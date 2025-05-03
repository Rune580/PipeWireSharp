using PipeWireSharp.Native;

namespace PipeWireSharp.Spa.Enums;

public enum SpaMediaType : uint
{
    Unknown = Bindings.SPA_MEDIA_TYPE_unknown,
    Audio = Bindings.SPA_MEDIA_TYPE_audio,
    Video = Bindings.SPA_MEDIA_TYPE_video,
    Image = Bindings.SPA_MEDIA_TYPE_image,
    Binary = Bindings.SPA_MEDIA_TYPE_binary,
    Stream = Bindings.SPA_MEDIA_TYPE_stream,
    Application = Bindings.SPA_MEDIA_TYPE_application,
}