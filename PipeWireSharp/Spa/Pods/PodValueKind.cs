using PipeWireSharp.Native;

namespace PipeWireSharp.Spa.Pods;

public enum PodValueKind : uint
{
    None = Bindings.SPA_TYPE_None,
    Bool = Bindings.SPA_TYPE_Bool,
    Id = Bindings.SPA_TYPE_Id,
    Int = Bindings.SPA_TYPE_Int,
    Long = Bindings.SPA_TYPE_Long,
    Float = Bindings.SPA_TYPE_Float,
    Double = Bindings.SPA_TYPE_Double,
    String = Bindings.SPA_TYPE_String,
    Bytes = Bindings.SPA_TYPE_Bytes,
    Rectangle = Bindings.SPA_TYPE_Rectangle,
    Fraction = Bindings.SPA_TYPE_Fraction,
    Bitmap = Bindings.SPA_TYPE_Bitmap,
    Array = Bindings.SPA_TYPE_Array,
    Struct = Bindings.SPA_TYPE_Struct,
    Object = Bindings.SPA_TYPE_Object,
    Sequence = Bindings.SPA_TYPE_Sequence,
    Pointer = Bindings.SPA_TYPE_Pointer,
    Fd = Bindings.SPA_TYPE_Fd,
    Choice = Bindings.SPA_TYPE_Choice,
    Pod = Bindings.SPA_TYPE_Pod
}

internal static class PodValueKindExtensions
{
    public static int PodHeaderByteSize(this PodValueKind kind)
    {
        return kind switch
        {
            PodValueKind.None => sizeof(int) * 2,
            PodValueKind.Bool => sizeof(int) * 2,
            PodValueKind.Id => sizeof(int) * 2,
            PodValueKind.Int => sizeof(int) * 2,
            PodValueKind.Long => sizeof(int) * 2,
            PodValueKind.Float => sizeof(int) * 2,
            PodValueKind.Double => sizeof(int) * 2,
            PodValueKind.String => sizeof(int) * 2,
            PodValueKind.Bytes => sizeof(int) * 2,
            PodValueKind.Rectangle => sizeof(int) * 2,
            PodValueKind.Fraction => sizeof(int) * 2,
            PodValueKind.Bitmap => sizeof(int) * 2,
            PodValueKind.Array => sizeof(int) * 4,
            PodValueKind.Struct => sizeof(int) * 2,
            PodValueKind.Object => sizeof(int) * 4,
            PodValueKind.Sequence => sizeof(int) * 4,
            PodValueKind.Pointer => sizeof(int) * 4,
            PodValueKind.Fd => sizeof(int) * 2,
            PodValueKind.Choice => sizeof(int) * 6,
            PodValueKind.Pod => 0,
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
        };
    }
    
    public static int PodBodyByteSize(this PodValueKind kind)
    {
        return kind switch
        {
            PodValueKind.None => sizeof(int) * 0,
            PodValueKind.Bool => sizeof(int) * 2,
            PodValueKind.Id => sizeof(int) * 2,
            PodValueKind.Int => sizeof(int) * 2,
            PodValueKind.Long => sizeof(int) * 2,
            PodValueKind.Float => sizeof(int) * 2,
            PodValueKind.Double => sizeof(int) * 2,
            PodValueKind.String => 0,
            PodValueKind.Bytes => 0,
            PodValueKind.Rectangle => sizeof(int) * 2,
            PodValueKind.Fraction => sizeof(int) * 2,
            PodValueKind.Bitmap => 0,
            PodValueKind.Array => 0,
            PodValueKind.Struct => 0,
            PodValueKind.Object => 0,
            PodValueKind.Sequence => 0,
            PodValueKind.Pointer => 0,
            PodValueKind.Fd => 0,
            PodValueKind.Choice => 0,
            PodValueKind.Pod => 0,
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
        };
    }
}