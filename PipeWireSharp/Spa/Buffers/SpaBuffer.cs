using PipeWireSharp.Native;

namespace PipeWireSharp.Spa.Buffers;

public class SpaBuffer
{
    internal readonly IntPtr Handle;
    internal unsafe spa_buffer* RawHandle => (spa_buffer*)Handle;

    public readonly SpaData[] Data;
    public readonly SpaMeta[] MetaData;

    internal SpaBuffer(IntPtr handle)
    {
        unsafe
        {
            Handle = handle;
            
            Data = new SpaData[RawHandle->n_datas];
            for (int i = 0; i < Data.Length; i++)
                Data[i] = new SpaData((IntPtr)(&RawHandle->datas[i]));
            
            MetaData = new SpaMeta[RawHandle->n_metas];
            for (int i = 0; i < MetaData.Length; i++)
                MetaData[i] = new SpaMeta((IntPtr)(&RawHandle->metas[i]));
        }
    }
}