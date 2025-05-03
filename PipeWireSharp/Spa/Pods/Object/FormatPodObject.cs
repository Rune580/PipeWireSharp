using PipeWireSharp.Spa.Enums;

namespace PipeWireSharp.Spa.Pods.Object;

public class FormatPodObject
{
    private readonly PodObject _pod;
    
    internal FormatPodObject(PodValue podValue)
    {
        _pod = podValue.GetValue<PodObject>();
    }

    public SpaMediaType? MediaType => (SpaMediaType?)_pod[(uint)SpaFormat.MediaType]?.GetChoiceValue<uint>(0);
    public SpaMediaSubType? MediaSubType => (SpaMediaSubType?)_pod[(uint)SpaFormat.MediaSubtype]?.GetChoiceValue<uint>(0);

    // Video Parameters
    public SpaRectangle? VideoSize => _pod[(uint)SpaFormat.VideoSize]?.GetChoiceValue<SpaRectangle>(0);
    public SpaFraction? VideoFramerate => _pod[(uint)SpaFormat.VideoFramerate]?.GetChoiceValue<SpaFraction>(0);
    public SpaFraction? VideoMaxFramerate => _pod[(uint)SpaFormat.VideoMaxFramerate]?.GetChoiceValue<SpaFraction>(0);
    public SpaVideoFormat? VideoFormat => (SpaVideoFormat?)_pod[(uint)SpaFormat.VideoFormat]?.GetChoiceValue<uint>(0);
    
    public static implicit operator FormatPodObject(PodValue podValue) => new(podValue);
}