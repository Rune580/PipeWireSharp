namespace PipeWireSharp.PipeWire.Streams;

[Flags]
public enum StreamFlags
{
    None = 0,
    AutoConnect = 1 << 0,
    Inactive = 1 << 1,
    MapBuffers = 1 << 2,
    Driver = 1 << 3,
    RtProcess = 1 << 4,
    NoConvert = 1 << 5,
    Exclusive = 1 << 6,
    DontReconnect = 1 << 7,
    AllocBuffers = 1 << 8,
    Trigger = 1 << 9,
    Async = 1 << 10,
    EarlyProcess = 1 << 11,
    RtTriggerDone = 1 << 12
}