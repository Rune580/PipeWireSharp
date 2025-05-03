using System.Runtime.InteropServices;
using PipeWireSharp.PipeWire;
using PipeWireSharp.PipeWire.Buffers;
using PipeWireSharp.PipeWire.Streams;
using PipeWireSharp.Spa;
using PipeWireSharp.Spa.Enums;
using PipeWireSharp.Spa.Pods;
using PipeWireSharp.Spa.Pods.Object;
using PipeWireSharp.Spa.Utils;
using Stream = PipeWireSharp.PipeWire.Streams.Stream;

namespace PipeWireSharp.Tests.ScreenCast;

public class ScreenCastCaptureSession
{
    private MainLoop _mainLoop;
    private Context _context;
    private Core _core;
    private Stream _stream;
    private StreamListener _listener;
    private FormatPodObject? _videoFormat;

    public Action<int, VideoFrameDataAccessor>? FrameDataReceived;
    
    private ScreenCastCaptureSession(string sessionName, SafeHandle remoteFd, uint targetNodeId)
    {
        PipeWireSharpLib.Init();
        
        _mainLoop = new MainLoop();
        _context = new Context(_mainLoop);
        _core = _context.ConnectFd(remoteFd);
        
        var streamProperties = new PwProperties();
        streamProperties.Insert(PwPropertyKey.MEDIA_TYPE, "Video");
        streamProperties.Insert(PwPropertyKey.MEDIA_CATEGORY, "Capture");
        streamProperties.Insert(PwPropertyKey.MEDIA_ROLE, "Screen");
        
        _stream = new Stream(_core, sessionName, streamProperties);

        _listener = _stream.AddListener()
            .OnParamChanged(StreamParamsChanged)
            .OnProcess(ProcessStream)
            .Register();
        
        var videoParams = new VideoParameters
        {
            Formats = { SpaVideoFormat.Rgb, SpaVideoFormat.Rgba, SpaVideoFormat.Bgr, SpaVideoFormat.Bgra },
            PreferredSize = new SpaRectangle
            {
                Width = 1920,
                Height = 1080
            },
            MinSize = new SpaRectangle
            {
                Width = 1,
                Height = 1
            },
            MaxSize = new SpaRectangle
            {
                Width = 10240,
                Height = 5760
            },
            PreferredFrameRate = new SpaFraction
            {
                Numerator = 30,
                Denominator = 1
            },
            MinFrameRate = new SpaFraction
            {
                Numerator = 0,
                Denominator = 1
            },
            MaxFrameRate = new SpaFraction
            {
                Numerator = 480,
                Denominator = 1
            }
        };

        _stream.Connect(Direction.Input, targetNodeId, StreamFlags.AutoConnect | StreamFlags.MapBuffers, videoParams);
    }

    public void StartCapturing()
    {
        ThreadPool.QueueUserWorkItem(_ => _mainLoop.Run());
    }

    public void StopCapturing()
    {
        ThreadPool.QueueUserWorkItem(_ => _mainLoop.Quit());
    }

    private void StreamParamsChanged(Stream stream, uint id, PodValue param)
    {
        if (id == (uint)SpaParamType.Format)
        {
            _videoFormat = param;
            stream.UpdateParams(new StreamParameters());
        }
    }

    private void ProcessStream(Stream stream)
    {
        if (FrameDataReceived is null || _videoFormat is null)
            return;
        
        var pwBuffer = stream.DequeueBuffer();
        var size = _videoFormat.VideoSize!.Value;

        for (int i = 0; i < pwBuffer.Buffer.Data.Length; i++)
        {
            var frameDataAccessor = pwBuffer.Buffer.Data[i].AccessVideoFrameData(_videoFormat.VideoFormat!.Value, (int)size.Width, (int)size.Height);
            FrameDataReceived(i, frameDataAccessor);
        }
        
        stream.QueueBuffer(pwBuffer);
    }

    public static async Task<ScreenCastCaptureSession> CreateAsync(string sessionName, ScreenCastSession screenCastSession)
    {
        var streams = await screenCastSession.StartAsync();
        var remoteHandle = await screenCastSession.OpenPipeWireRemoteAsync();
        
        return new ScreenCastCaptureSession(sessionName, remoteHandle, streams[0].PipeWireNodeId);
    }
}