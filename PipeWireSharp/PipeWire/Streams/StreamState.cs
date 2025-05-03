namespace PipeWireSharp.PipeWire.Streams;

public enum StreamState
{
    PW_STREAM_STATE_ERROR = -1,
    PW_STREAM_STATE_UNCONNECTED = 0,
    PW_STREAM_STATE_CONNECTING = 1,
    PW_STREAM_STATE_PAUSED = 2,
    PW_STREAM_STATE_STREAMING = 3
}