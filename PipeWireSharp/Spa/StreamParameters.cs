using PipeWireSharp.Native;
using PipeWireSharp.Spa.Enums;
using PipeWireSharp.Spa.Pods;

namespace PipeWireSharp.Spa;

public class StreamParameters : IPodSerializable
{
    public Pod ToPod()
    {
        var builder = new PodBuilder(4096);

        builder.PushObject(SpaType.ObjectParamBuffers, SpaParamType.Buffers);

        const int flags = (1 << (int)Bindings.SPA_DATA_MemPtr) | (1 << (int)Bindings.SPA_DATA_DmaBuf);

        builder.AddProperty(Bindings.SPA_PARAM_BUFFERS_dataType, flags);
        
        return builder.Build();
    }
}