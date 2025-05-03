namespace PipeWireSharp.Spa.Enums;

public enum SpaType : uint
{
    /* Basic types */
    None = 0x00001,
    Bool,
    Id,
    Int,
    Long,
    Float,
    Double,
    String,
    Bytes,
    Rectangle,
    Fraction,
    Bitmap,
    Array,
    Struct,
    Object,
    Sequence,
    Pointer,
    Fd,
    Choice,
    Pod,
    /* Pointers */
    PointerBuffer = 0x10001,
    PointerMeta,
    PointerDict,
    /* Events */
    EventDevice = 0x20001,
    EventNode,
    /* Commands */
    CommandDevice = 0x30001,
    CommandNode,
    /* Objects */
    ObjectPropInfo = 0x40001,
    ObjectProps,
    ObjectFormat,
    ObjectParamBuffers,
    ObjectParamMeta,
    ObjectParamIo,
    ObjectParamProfile,
    ObjectParamPortConfig,
    ObjectParamRoute,
    ObjectProfiler,
    ObjectParamLatency,
    ObjectParamProcessLatency,
    ObjectParamTag,
    /* vendor extensions */
    VendorPipeWire = 0x02000000,
    VendorOther = 0x7f000000,
}