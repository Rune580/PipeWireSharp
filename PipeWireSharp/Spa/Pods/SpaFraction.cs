using PipeWireSharp.Native;

namespace PipeWireSharp.Spa.Pods;

public struct SpaFraction
{
    public uint Numerator;
    public uint Denominator;
    
    public SpaFraction() { }

    internal SpaFraction(spa_fraction fraction)
    {
        Numerator = fraction.num;
        Denominator = fraction.denom;
    }

    public override string ToString() =>
        $"{{ Value: {Numerator / Denominator}, Numerator: {Numerator}, Denominator: {Denominator} }}";
}