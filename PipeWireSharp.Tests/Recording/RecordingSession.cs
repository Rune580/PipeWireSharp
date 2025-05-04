using System.Collections.Concurrent;
using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Pipes;
using PipeWireSharp.Spa.Utils;
using PipeWireSharp.Tests.ScreenCast;

namespace PipeWireSharp.Tests.Recording;

public class RecordingSession
{
    private readonly ConcurrentQueue<RawVideoFrame> _frames = new();
    private readonly ScreenCastCaptureSession _captureSession;
    private bool _isRecording;

    public RecordingSession(ScreenCastCaptureSession captureSession)
    {
        _captureSession = captureSession;
        _captureSession.FrameDataReceived += OnFrameDataReceived;
    }

    public void StartRecording(string filePath)
    {
        _captureSession.StartCapturing();
        _isRecording = true;

        var videoSource = new RawVideoPipeSource(GetVideoFrames())
        {
            FrameRate = 60,
        };

        FFMpegArguments.FromPipeInput(videoSource)
            .OutputToFile(filePath, true, options =>
            {
                options.WithVideoCodec(VideoCodec.LibX264);
            })
            .ProcessSynchronously();
    }

    public void StopRecording()
    {
        _isRecording = false;
        _captureSession.StopCapturing();
    }

    private void OnFrameDataReceived(int streamId, VideoFrameDataAccessor accessor)
    {
        var croppedFrame = accessor.Crop(3072, 1612, 600, 600);
        
        var frameData = new Memory<byte>(new byte[croppedFrame.ByteSizeOfFrame]);
        var offset = 0;
        
        croppedFrame.AccessRows(span =>
        {
            span.CopyTo(frameData[offset..].Span);
            offset += span.Length;
        });
        
        _frames.Enqueue(new RawVideoFrame(frameData, croppedFrame.FrameWidth, croppedFrame.FrameHeight, croppedFrame.VideoFormat.ToString().ToLower()));
    }

    public IEnumerable<RawVideoFrame> GetVideoFrames()
    {
        while (_frames.IsEmpty)
        {
            Thread.Sleep(100);
        }

        var frames = 60*10;
        
        while ((_isRecording || !_frames.IsEmpty) && frames > 0)
        {
            if (!_frames.TryDequeue(out var frame))
            {
                Thread.Sleep(500);
                continue;
            }

            frames--;
            
            yield return frame;
        }
    }
}