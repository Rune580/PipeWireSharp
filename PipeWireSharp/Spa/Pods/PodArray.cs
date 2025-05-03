using System.Text;

namespace PipeWireSharp.Spa.Pods;

public class PodArray
{
    public readonly PodValue[] Children;

    public PodArray(PodValue[] children)
    {
        Children = children;
    }

    public override string ToString()
    {
        var builder = new StringBuilder();

        foreach (var child in Children)
            builder.Append($"{{ {child.ToString()} }}, ");

        return $"[ {builder.ToString().Trim(',', ' ')} ]";
    }
}