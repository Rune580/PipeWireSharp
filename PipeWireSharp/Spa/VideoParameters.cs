using PipeWireSharp.Native;
using PipeWireSharp.Spa.Enums;
using PipeWireSharp.Spa.Pods;
using PipeWireSharp.Spa.Pods.Choice;

namespace PipeWireSharp.Spa;

public class VideoParameters : IPodSerializable
{
    public List<SpaVideoFormat> Formats { get; } = [];
    
    public SpaRectangle PreferredSize { get; set; }
    public SpaRectangle MinSize { get; set; }
    public SpaRectangle MaxSize { get; set; }
    
    public SpaFraction PreferredFrameRate { get; set; }
    public SpaFraction MinFrameRate { get; set; }
    public SpaFraction MaxFrameRate { get; set; }
    
    public Pod ToPod()
    {
        var builder = new PodBuilder(4096);
        
        builder.PushObject(Bindings.SPA_TYPE_OBJECT_Format, Bindings.SPA_PARAM_EnumFormat);

        builder.AddProperty(Bindings.SPA_FORMAT_mediaType, PodValue.Id(Bindings.SPA_MEDIA_TYPE_video))
            .AddProperty(Bindings.SPA_FORMAT_mediaSubtype, PodValue.Id(Bindings.SPA_MEDIA_SUBTYPE_raw));

        if (Formats.Count < 1)
            throw new Exception("There must be at least one video format!");

        var formats = Formats.Select(fmt => PodValue.Id((uint)fmt))
            .ToArray();
        
        var videoFormatsChoice = PodChoice.Enum(formats[0], formats.Skip(1).ToArray());
        builder.AddProperty(Bindings.SPA_FORMAT_VIDEO_format, videoFormatsChoice);
        
        var videoSizeChoice = PodChoice.Range(PreferredSize, MinSize, MaxSize);
        builder.AddProperty(Bindings.SPA_FORMAT_VIDEO_size, videoSizeChoice);
        
        var videoFrameRateChoice = PodChoice.Range(PreferredFrameRate, MinFrameRate, MaxFrameRate);
        builder.AddProperty(Bindings.SPA_FORMAT_VIDEO_framerate, videoFrameRateChoice);
        
        return builder.Build();
    }
}