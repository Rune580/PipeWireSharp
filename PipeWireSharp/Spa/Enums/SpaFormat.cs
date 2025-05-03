namespace PipeWireSharp.Spa.Enums;

public enum SpaFormat : uint
{
    MediaType = 0x00001,
    MediaSubtype,

    /* Audio format keys */
    AudioFormat = 0x10001,
    AudioFlags,
    AudioRate,
    AudioChannels,
    AudioPosition,
    AudioIec958Codec,
    AudioBitorder,
    AudioInterleave,
    AudioBitrate,
    AudioBlockAlign,
    AudioAacStreamFormat,
    AudioWmaProfile,
    AudioAmrBandMode,

    /* Video Format keys */
    VideoFormat = 0x20001,
    VideoModifier,
    VideoSize,
    VideoFramerate,
    VideoMaxFramerate,
    VideoViews,
    VideoInterlaceMode,
    VideoPixelAspectRatio,
    VideoMultiviewMode,
    VideoMultiviewFlags,
    VideoChromaSite,
    VideoColorRange,
    VideoColorMatrix,
    VideoTransferFunction,
    VideoColorPrimaries,
    VideoProfile,
    VideoLevel,
    VideoH264StreamFormat,
    VideoH264Alignment,

    /* Image Format keys */
    // START_Image = 0x30000,

    /* Binary Format keys */
    // START_Binary = 0x40000,

    /* Stream Format keys */
    // START_Stream = 0x50000,

    /* Application Format keys */
    ControlTypes = 0x60001,
}