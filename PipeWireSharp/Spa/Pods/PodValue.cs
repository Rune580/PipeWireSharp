using System.Text;
using PipeWireSharp.Spa.Pods.Choice;

namespace PipeWireSharp.Spa.Pods;

public class PodValue : IPodSerializable
{
    public readonly PodValueKind Kind;
    public readonly object BoxedValue;

    private PodValue(PodValueKind kind, object value)
    {
        Kind = kind;
        BoxedValue = value;
    }
    
    public T GetValue<T>() => (T)BoxedValue;

    public T GetChoiceValue<T>(int i)
    {
        var choice = GetValue<PodChoice>();
        return choice.Values[i].GetValue<T>();
    }

    public override string ToString()
    {
        return $"{Kind}({BoxedValue})";
    }

    internal int EstimatePodByteSize()
    {
        var size = Kind.PodHeaderByteSize();

        if (Kind == PodValueKind.Array)
        {
            var array = GetValue<PodArray>();
            size += array.Children.Sum(child => child.EstimatePodByteSize());
        }
        else if (Kind == PodValueKind.Object)
        {
            var podObject = GetValue<PodObject>();
            size += podObject.Properties.Sum(prop => prop.Value.EstimatePodByteSize());
        }
        else if (Kind == PodValueKind.Choice)
        {
            var choice = GetValue<PodChoice>();
            size += choice.Values.Sum(value => value.EstimatePodByteSize());
        }
        else if (Kind == PodValueKind.String)
        {
            size += Encoding.UTF8.GetBytes(GetValue<string>()).Length;
        }
        else if (Kind == PodValueKind.Bytes)
        {
            size += GetValue<byte[]>().Length;
        }
        // Todo: other container pod types
        else
        {
            size += Kind.PodBodyByteSize();
        }

        return size;
    }

    public Pod ToPod()
    {
        // Create builder with buffer size of estimated size for pods.
        var builder = new PodBuilder((uint)EstimatePodByteSize());

        builder.PushValue(this);

        return builder.Build();
    }

    public static PodValue None() => new(PodValueKind.None, null!);
    
    public static PodValue Id(uint id) => new(PodValueKind.Id, id);

    public static PodValue Pod(PodValue pod) => new(PodValueKind.Pod, pod);

    public static implicit operator PodValue(bool value) => new(PodValueKind.Bool, value);
    
    public static implicit operator PodValue(int value) => new(PodValueKind.Int, value);
    
    public static implicit operator PodValue(long value) => new(PodValueKind.Long, value);
    
    public static implicit operator PodValue(float value) => new(PodValueKind.Float, value);
    
    public static implicit operator PodValue(double value) => new(PodValueKind.Double, value);
    
    public static implicit operator PodValue(string value) => new(PodValueKind.String, value);
    
    public static implicit operator PodValue(byte[] value) => new(PodValueKind.Bytes, value);
    
    public static implicit operator PodValue(SpaRectangle value) => new(PodValueKind.Rectangle, value);
    
    public static implicit operator PodValue(SpaFraction value) => new(PodValueKind.Fraction, value);
    
    public static implicit operator PodValue(PodArray value) => new(PodValueKind.Array, value);
    
    public static implicit operator PodValue(PodObject value) => new(PodValueKind.Object, value);

    public static implicit operator PodValue(PodChoice value) => new(PodValueKind.Choice, value);
}