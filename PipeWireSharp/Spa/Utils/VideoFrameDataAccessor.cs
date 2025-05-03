using System.Diagnostics.Contracts;
using PipeWireSharp.Spa.Enums;

namespace PipeWireSharp.Spa.Utils;

public ref struct VideoFrameDataAccessor
{
    private readonly ReadOnlySpan<byte> _data;
    private readonly int _srcWidth;
    private readonly int _srcHeight;
    private readonly int _bytesPerPixel;

    private int _frameBoundsMinX;
    private int _frameBoundsMinY;
    private int _frameBoundsMaxX;
    private int _frameBoundsMaxY;
    
    public SpaVideoFormat VideoFormat { get; }
    public int FrameWidth => _frameBoundsMaxX - _frameBoundsMinX;
    public int FrameHeight => _frameBoundsMaxY - _frameBoundsMinY;

    public int ByteSizeOfFrame
    {
        get
        {
            var stride = (_frameBoundsMaxX - _frameBoundsMinX) * _bytesPerPixel;
            var rowCount = _frameBoundsMaxY - _frameBoundsMinY;
            return stride * rowCount;
        }
    }
    
    internal VideoFrameDataAccessor(ReadOnlySpan<byte> data, SpaVideoFormat videoFormat, int stride, int width, int height)
    {
        _data = data;
        VideoFormat = videoFormat;
        _srcWidth = width;
        _srcHeight = height;
        _bytesPerPixel = stride / _srcWidth;
        _frameBoundsMinX = 0;
        _frameBoundsMinY = 0;
        _frameBoundsMaxX = width;
        _frameBoundsMaxY = height;
    }
    
    [Pure]
    public VideoFrameDataAccessor Crop(int x, int y, int width, int height)
    {
        if (x < 0 || y < 0 || x + width >= _srcWidth || y + height >= _srcHeight)
            throw new Exception("Invalid crop bounds!");

        return this with
        {
            _frameBoundsMinX = x,
            _frameBoundsMinY = y,
            _frameBoundsMaxX = x + width,
            _frameBoundsMaxY = y + height
        };
    }

    public void AccessRows(Action<ReadOnlySpan<byte>> rowAccessor)
    {
        var srcStride = _srcWidth * _bytesPerPixel;
        var stride = (_frameBoundsMaxX - _frameBoundsMinX) * _bytesPerPixel;
        
        var rowCount = _frameBoundsMaxY - _frameBoundsMinY;

        for (int i = 0; i < rowCount; i++)
        {
            var offset = (_frameBoundsMinY + i) * srcStride + _frameBoundsMinX * _bytesPerPixel;
            var row = _data.Slice(offset, stride);
            
            rowAccessor.Invoke(row);
        }
    }

    public void AccessRows(Action<ReadOnlySpan<byte>, SpaVideoFormat> rowAccessor)
    {
        var srcStride = _srcWidth * _bytesPerPixel;
        var stride = (_frameBoundsMaxX - _frameBoundsMinX) * _bytesPerPixel;
        
        var rowCount = _frameBoundsMaxY - _frameBoundsMinY;

        for (int i = 0; i < rowCount; i++)
        {
            var offset = (_frameBoundsMinY + i) * srcStride + _frameBoundsMinX * _bytesPerPixel;
            var row = _data.Slice(offset, stride);
            
            rowAccessor.Invoke(row, VideoFormat);
        }
    }
}