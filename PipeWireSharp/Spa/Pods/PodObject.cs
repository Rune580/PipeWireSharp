using System.Text;
using PipeWireSharp.Utils;

namespace PipeWireSharp.Spa.Pods;

public class PodObject
{
    public readonly uint Type;
    public readonly uint Id;
    
    public Dictionary<uint, PodValue> Properties { get; } = new();

    public PodObject(uint type, uint id)
    {
        Type = type;
        Id = id;
    }

    public PodValue? this[uint key] => Properties.GetValueOrDefault(key);

    public override string ToString()
    {
        var builder = new StringBuilder();

        foreach (var (key, value) in Properties)
            builder.Append($"{{ Key: {SpaEnumUtils.GetNameFromTypeKey(Type, key)}, Value: {value} }}");
        
        return $"{{ Type: {SpaEnumUtils.GetNameFromType(Type)}, Id: {SpaEnumUtils.GetNameFromParamType(Id)}, Properties: [ {builder} ] }}";
    }
}