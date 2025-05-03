using System.Text;

namespace PipeWireSharp.Spa.Pods.Choice;

public class PodChoice
{
    public readonly ChoiceKind Kind;
    public readonly uint ExtraFlags;
    public readonly PodValue[] Values;

    private PodChoice(ChoiceKind kind, uint flags, params PodValue[] values)
    {
        Kind = kind;
        ExtraFlags = flags;
        Values = values;
    }
    
    private PodChoice(ChoiceKind kind, params PodValue[] values) : this(kind, 0, values) { }

    public override string ToString()
    {
        var builder = new StringBuilder();

        foreach (var value in Values)
            builder.Append($"{value}, ");

        return $"{Kind}([ {builder.ToString().Trim(',', ' ')} ])";
    }

    public static PodChoice None(PodValue value) => new(ChoiceKind.None, value);

    public static PodChoice Range(PodValue defaultValue, PodValue minValue, PodValue maxValue) =>
        new(ChoiceKind.Range, defaultValue, minValue, maxValue);

    public static PodChoice Step(PodValue defaultValue, PodValue minValue, PodValue maxValue, PodValue step) =>
        new(ChoiceKind.Step, defaultValue, minValue, maxValue, step);

    public static PodChoice Enum(PodValue defaultValue, params PodValue[] alternatives) =>
        new(ChoiceKind.Enum, [defaultValue, ..alternatives]);

    public static PodChoice Flags(PodValue defaultValue, params PodValue[] possibleFlags) =>
        new(ChoiceKind.Flags, [defaultValue, ..possibleFlags]);
}