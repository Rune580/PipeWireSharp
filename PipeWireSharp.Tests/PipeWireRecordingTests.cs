using PipeWireSharp.Tests.Recording;
using PipeWireSharp.Tests.ScreenCast;
using Xunit;

namespace PipeWireSharp.Tests;

public class PipeWireRecordingTests
{
    [Fact]
    public async Task RecordScreenTest()
    {
        using var screenCastSession = await ScreenCastSession.CreateAsync();
        var screenCaptureSession = await ScreenCastCaptureSession.CreateAsync("pipewiresharp_tests", screenCastSession);

        var recordingSession = new RecordingSession(screenCaptureSession);

        var videoPath = Path.Join(Path.GetTempPath(), "video_capture_test.mp4");

        recordingSession.StartRecording(videoPath);

        Console.WriteLine("Recording started!");
        
        // await Task.Delay(4000, TestContext.Current.CancellationToken);
        
        recordingSession.StopRecording();
        
        Console.WriteLine($"Recording stopped! File at: {Path.GetFullPath(videoPath)}");

        await screenCastSession.StopAsync();
    }
}