using Bomber.Client.Interfaces;

namespace Bomber.Client.Contexts;

public sealed class SystemDrawingContext : IDrawingContext
{
    private Graphics? _graphics;

    public void BeginFrame(Graphics graphics)
    {
        _graphics = graphics;
    }

    public void FillRectangle(
        Color color,
        RectangleF rectangle)
    {
        Graphics graphics = GetGraphics();

        using SolidBrush brush = new(color);

        graphics.FillRectangle(
            brush,
            rectangle);
    }

    public void DrawRectangle(
        Color color,
        RectangleF rectangle)
    {
        Graphics graphics = GetGraphics();

        using Pen pen = new(color);

        graphics.DrawRectangle(
            pen,
            rectangle.X,
            rectangle.Y,
            rectangle.Width,
            rectangle.Height);
    }

    public void FillEllipse(
        Color color,
        RectangleF rectangle)
    {
        Graphics graphics = GetGraphics();

        using SolidBrush brush = new(color);

        graphics.FillEllipse(
            brush,
            rectangle);
    }

    public void DrawText(
        string text,
        Color color,
        float x,
        float y)
    {
        Graphics graphics = GetGraphics();

        using SolidBrush brush = new(color);

        graphics.DrawString(
            text,
            SystemFonts.DefaultFont,
            brush,
            x,
            y);
    }

    private Graphics GetGraphics()
    {
        if (_graphics is null)
            throw new InvalidOperationException("BeginFrame must be called before drawing.");

        return _graphics;
    }
}
