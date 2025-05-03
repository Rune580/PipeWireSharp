using PipeWireSharp.Spa.Pods;

namespace PipeWireSharp.PipeWire.Streams;

public static class StreamEvents
{
    public delegate void DestroyEvent(Stream stream);
    public delegate void DestroyEvent<T>(Stream stream, T data);

    public delegate void StateChangedEvent(Stream stream, StreamState prevState, StreamState newState, string errorMsg);
    public delegate void StateChangedEvent<T>(Stream stream, T data, StreamState prevState, StreamState newState, string errorMsg);

    public delegate void ControlInfoEvent(Stream stream, uint id, StreamControl control);
    public delegate void ControlInfoEvent<T>(Stream stream, T data, uint id, StreamControl control);
    
    public delegate void ParamChangedEvent(Stream stream, uint id, PodValue param);
    public delegate void ParamChangedEvent<T>(Stream stream, T data, uint id, object param);

    public delegate void ProcessEvent(Stream stream);
    public delegate void ProcessEvent<T>(Stream stream, T data);
}