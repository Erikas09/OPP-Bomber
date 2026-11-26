using Bomber.Client.Interfaces;
namespace Bomber.Client.Rendering;

public sealed class SpriteSheet : IDisposable
{
    private readonly Bitmap _image;

    public int FrameWidth { get; }
    public int FrameHeight { get; }
    public int OffsetX { get; }
    public int OffsetY { get; }

    public SpriteSheet(string path, int frameWidth, int frameHeight, int offsetX = 0, int offsetY = 0)
    {
        _image = new Bitmap(path);

        FrameWidth = frameWidth;
        FrameHeight = frameHeight;

        OffsetX = offsetX;
        OffsetY = offsetY;
    }

    public void Draw(IDrawingContext context, int column, int row, RectangleF destination)
    {
        destination.X += OffsetX;
        destination.Y += OffsetY;

        Rectangle source = new(column * FrameWidth, row * FrameHeight, FrameWidth, FrameHeight);

        context.DrawImage(_image, destination, source);
    }

    public void Dispose()
    {
        _image.Dispose();
    }
}