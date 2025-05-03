use std::env;
use std::path::PathBuf;

fn main() {
    gen_spa_bindings();
    gen_pipewire_bindings();
    gen_csharp_bindings();
}

fn gen_spa_bindings() {
    let libs = system_deps::Config::new()
        .probe()
        .expect("Cannot find libraries");

    // Write bindings files to the $OUT_DIR/ directory.
    let out_path = PathBuf::from(env::var("OUT_DIR").unwrap());

    let builder = bindgen::builder()
        .header("spa-wrapper.h")
        // Use `usize` for `size_t`. This behavior of bindgen changed because it is not
        // *technically* correct, but is the case in all architectures supported by Rust.
        .size_t_is_usize(true)
        .allowlist_function("spa_.*")
        .allowlist_type("spa_.*")
        .allowlist_var("SPA_.*")
        .blocklist_function("spa_handle_factory_enum") // Not included in library, but included in header file.
        .prepend_enum_name(false)
        .derive_eq(true)
        // Create callable wrapper functions around SPAs `static inline` functions so they
        // can be called via FFI
        .wrap_static_fns(true)
        .wrap_static_fns_suffix("_libspa_rs")
        .wrap_static_fns_path(out_path.join("static_fns"));

    let builder = libs
        .iter()
        .iter()
        .flat_map(|(_, lib)| lib.include_paths.iter())
        .fold(builder, |builder, l| {
            let arg = format!("-I{}", l.to_string_lossy());
            builder.clang_arg(arg)
        });

    let bindings = builder.generate().expect("Unable to generate bindings");

    bindings
        .write_to_file("src/spa_bindings.rs")
        .expect("Couldn't write bindings!");
    const FILES: &[&str] = &["src/type-info.c"];
    let cc_files = &[PathBuf::from(FILES[0]), out_path.join("static_fns.c")];

    for file in FILES {
        println!("cargo:rerun-if-changed={file}");
    }

    let mut cc = cc::Build::new();
    cc.files(cc_files);
    cc.include(env!("CARGO_MANIFEST_DIR"));
    cc.includes(libs.all_include_paths());

    cc.compile("libspa-rs-reexports");
}

fn gen_pipewire_bindings() {
    let libs = system_deps::Config::new()
        .probe()
        .expect("Cannot find libpipewire");
    let libpipewire = libs.get_by_name("libpipewire").unwrap();

    let builder = bindgen::Builder::default()
        .header("pipewire-wrapper.h")
        // Tell cargo to invalidate the built crate whenever any of the
        // included header files changed.
        .size_t_is_usize(true)
        .allowlist_function("pw_.*")
        .allowlist_type("pw_.*")
        .allowlist_var("pw_.*")
        .allowlist_var("PW_.*")
        .blocklist_function("spa_.*")
        .blocklist_type("spa_.*")
        .blocklist_item("spa_.*")
        .blocklist_type("__va_list_tag")
        .blocklist_type("timespec")
        .blocklist_function("pw_impl_link_find") // Not included in library, but included in header file.
        .blocklist_function("pw_impl_core_get_info") // Not included in library, but included in header file.
        .blocklist_function("pw_impl_port_state_as_string") // Not included in library, but included in header file.
        .blocklist_function("pw_work_queue_destroy") // Not included in library, but included in header file.
        .blocklist_function("pw_work_queue_new") // Not included in library, but included in header file.
        .blocklist_function("spa_handle_factory_enum") // Not included in library, but included in header file.
        .raw_line("use super::spa_bindings::*;");

    let builder = libpipewire
        .include_paths
        .iter()
        .fold(builder, |builder, l| {
            let arg = format!("-I{}", l.to_string_lossy());
            builder.clang_arg(arg)
        });

    let bindings = builder.generate().expect("Unable to generate bindings");

    // Write the bindings to the $OUT_DIR/bindings.rs file.
    let out_path = PathBuf::from(env::var("OUT_DIR").unwrap());
    bindings
        .write_to_file("src/pipewire_bindings.rs")
        .expect("Couldn't write bindings!");
}

fn gen_csharp_bindings() {
    csbindgen::Builder::default()
        .input_bindgen_file("src/spa_bindings.rs")
        .input_bindgen_file("src/pipewire_bindings.rs")
        .rust_file_header("use super::spa_bindings::*;\nuse super::pipewire_bindings::*;\nuse std::ptr;\n\n#[no_mangle]\npub unsafe extern \"C\" fn csbindgen_pw_init() {\n\tpw_init(ptr::null_mut(), ptr::null_mut())\n}")
        .csharp_entry_point_prefix("csbindgen_")
        .csharp_dll_name("libpipewire_bindings")
        .csharp_namespace("PipeWireSharp.Native")
        .csharp_class_name("Bindings")
        .csharp_generate_const_filter(|name| name.starts_with("SPA_") || name.starts_with("PW_"))
        .generate_to_file("src/ffi.rs", "../PipeWireSharp/Native/Bindings.g.cs")
        .unwrap();
}