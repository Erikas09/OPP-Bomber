using Bomber.Client.Interfaces;
using System.Drawing.Drawing2D;

namespace Bomber.Client.Contexts;

public sealed class DebugDrawingContext : IDrawingContext
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

        using Pen pen = new(color, 2f);

        pen.DashStyle = DashStyle.Dash;

        graphics.DrawRectangle(
            pen,
            rectangle.X,
            rectangle.Y,
            rectangle.Width,
            rectangle.Height);
    }

    public void DrawRectangle(
        Color color,
        RectangleF rectangle)
    {
        Graphics graphics = GetGraphics();

        using Pen pen = new(
            color,
            1f);

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

        using Pen pen = new(
            color,
            2f);

        pen.DashStyle = DashStyle.Dash;

        graphics.DrawEllipse(
            pen,
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
