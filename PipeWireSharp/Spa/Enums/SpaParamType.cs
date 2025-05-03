using PipeWireSharp.Native;

namespace PipeWireSharp.Spa.Enums;

public enum SpaParamType : uint
{
    Invalid = Bindings.SPA_PARAM_Invalid,              
    PropInfo = Bindings.SPA_PARAM_PropInfo,             
    Props = Bindings.SPA_PARAM_Props,                
    EnumFormat = Bindings.SPA_PARAM_EnumFormat,           
    Format = Bindings.SPA_PARAM_Format,               
    Buffers = Bindings.SPA_PARAM_Buffers,              
    Meta = Bindings.SPA_PARAM_Meta,                 
    Io = Bindings.SPA_PARAM_IO,                   
    EnumProfile = Bindings.SPA_PARAM_EnumProfile,          
    Profile = Bindings.SPA_PARAM_Profile,              
    EnumPortConfig = Bindings.SPA_PARAM_EnumPortConfig,       
    PortConfig = Bindings.SPA_PARAM_PortConfig,           
    EnumRoute = Bindings.SPA_PARAM_EnumRoute,            
    Route = Bindings.SPA_PARAM_Route,                
    Control = Bindings.SPA_PARAM_Control,              
    Latency = Bindings.SPA_PARAM_Latency,              
    ProcessLatency = Bindings.SPA_PARAM_ProcessLatency,       
    Tag = Bindings.SPA_PARAM_Tag
}