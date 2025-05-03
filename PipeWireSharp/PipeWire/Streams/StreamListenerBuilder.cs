using System.Runtime.InteropServices;
using PipeWireSharp.Native;
using PipeWireSharp.Spa.Pods;

namespace PipeWireSharp.PipeWire.Streams;

public class StreamListenerBuilder
{
    private readonly Stream _stream;
    
    private StreamEvents.DestroyEvent? _destroy;
    private StreamEvents.StateChangedEvent? _stateChanged;
    private StreamEvents.ParamChangedEvent? _paramChanged;
    private StreamEvents.ProcessEvent? _processEvent;
    
    internal StreamListenerBuilder(Stream stream)
    {
        _stream = stream;
    }
    
    public StreamListenerBuilder OnDestroy(StreamEvents.DestroyEvent callback)
    {
        _destroy = callback;
        return this;
    }

    public StreamListenerBuilder OnStateChanged(StreamEvents.StateChangedEvent callback)
    {
        _stateChanged = callback;
        return this;
    }

    public StreamListenerBuilder OnParamChanged(StreamEvents.ParamChangedEvent callback)
    {
        _paramChanged = callback;
        return this;
    }

    public StreamListenerBuilder OnProcess(StreamEvents.ProcessEvent callback)
    {
        _processEvent = callback;
        return this;
    }

    public StreamListener Register()
    {
        unsafe
        {
            var eventsSize = sizeof(pw_stream_events);
            var eventsPtr = Marshal.AllocHGlobal(eventsSize);

            var destroyHandler = DestroyHandler;
            var controlInfoChangedHandler = ControlInfoChangedHandler;
            var stateChangedHandler = StateChangedHandler;
            var paramChangedHandler = ParamChangedHandler;
            var addBufferHandler = AddBufferHandler;
            var removeBufferHandler = RemoveBufferHandler;
            var processHandler = ProcessHandler;
            var drainedHandler = DrainedHandler;
            var commandHandler = CommandHandler;
            var triggerDoneHandler = TriggerDoneHandler;
            
            var events = new pw_stream_events
            {
                version = Bindings.PW_VERSION_STREAM_EVENTS,
                destroy = (delegate* unmanaged[Cdecl]<void*, void>)Marshal.GetFunctionPointerForDelegate(destroyHandler),
                control_info = (delegate* unmanaged[Cdecl]<void*, uint, pw_stream_control*, void>)Marshal.GetFunctionPointerForDelegate(controlInfoChangedHandler),
                state_changed = (delegate* unmanaged[Cdecl]<void*, int, int, byte*, void>)Marshal.GetFunctionPointerForDelegate(stateChangedHandler),
                param_changed = (delegate* unmanaged[Cdecl]<void*, uint, spa_pod*, void>)Marshal.GetFunctionPointerForDelegate(paramChangedHandler),
                add_buffer = (delegate* unmanaged[Cdecl]<void*, pw_buffer*, void>)Marshal.GetFunctionPointerForDelegate(addBufferHandler),
                remove_buffer = (delegate* unmanaged[Cdecl]<void*, pw_buffer*, void>)Marshal.GetFunctionPointerForDelegate(removeBufferHandler),
                process = (delegate* unmanaged[Cdecl]<void*, void>)Marshal.GetFunctionPointerForDelegate(processHandler),
                drained = (delegate* unmanaged[Cdecl]<void*, void>)Marshal.GetFunctionPointerForDelegate(drainedHandler),
                command = (delegate* unmanaged[Cdecl]<void*, spa_command*, void>)Marshal.GetFunctionPointerForDelegate(commandHandler),
                trigger_done = (delegate* unmanaged[Cdecl]<void*, void>)Marshal.GetFunctionPointerForDelegate(triggerDoneHandler)
            };

            Marshal.StructureToPtr(events, eventsPtr, true);
            
            var hooksSize = sizeof(spa_hook);
            var hooksPtr = Marshal.AllocHGlobal(hooksSize);
            
            var hooks = new spa_hook();
            
            Marshal.StructureToPtr(hooks, hooksPtr, true);
        
            Bindings.pw_stream_add_listener(_stream.RawHandle, (spa_hook*)hooksPtr, (pw_stream_events*)eventsPtr, null);

            return new StreamListener(hooksPtr, eventsPtr, destroyHandler, controlInfoChangedHandler, stateChangedHandler, paramChangedHandler, addBufferHandler, removeBufferHandler, processHandler, drainedHandler, commandHandler, triggerDoneHandler);
        }
    }

    private unsafe void DestroyHandler(void* data)
    {
        _destroy?.Invoke(_stream);
        Console.WriteLine("Destroy");
    }
    
    private unsafe void StateChangedHandler(void* data, int prevState, int newState, byte* errorMsg)
    {
        var msg = Marshal.PtrToStringAuto((IntPtr)errorMsg);
        _stateChanged?.Invoke(_stream, (StreamState)prevState, (StreamState)newState, msg ?? "");
    }

    private unsafe void ControlInfoChangedHandler(void* data, uint id, pw_stream_control* streamControl)
    {
        Console.WriteLine("Control Info Changed");
    }

    private unsafe void ParamChangedHandler(void* data, uint id, spa_pod* rawParam)
    {
        var param = new Pod((IntPtr)rawParam);
        _paramChanged?.Invoke(_stream, id, param.ParsePod());
    }

    private unsafe void AddBufferHandler(void* data, pw_buffer* buffer)
    {
        Console.WriteLine("Add Buffer");
    }
    
    private unsafe void RemoveBufferHandler(void* data, pw_buffer* buffer)
    {
        Console.WriteLine("Remove Buffer");
    }

    private unsafe void ProcessHandler(void* data)
    {
        _processEvent?.Invoke(_stream);
    }
    
    private unsafe void DrainedHandler(void* data)
    {
        Console.WriteLine("Drained");
    }
    
    private unsafe void CommandHandler(void* data, spa_command* command)
    {
        Console.WriteLine("Command");
    }
    
    private unsafe void TriggerDoneHandler(void* data)
    {
        Console.WriteLine("Trigger Done");
    }
}