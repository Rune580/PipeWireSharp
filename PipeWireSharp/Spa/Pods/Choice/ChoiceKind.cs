using PipeWireSharp.Native;

namespace PipeWireSharp.Spa.Pods.Choice;

public enum ChoiceKind : uint
{
    None = Bindings.SPA_CHOICE_None,
    Range = Bindings.SPA_CHOICE_Range,
    Step = Bindings.SPA_CHOICE_Step,
    Enum = Bindings.SPA_CHOICE_Enum,
    Flags = Bindings.SPA_CHOICE_Flags
}