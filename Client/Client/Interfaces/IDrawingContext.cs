namespace Bomber.Client.Interfaces;

public interface IDrawingContext
{
    void BeginFrame(Graphics graphics);

    void FillRectangle(
        Color color,
        RectangleF rectangle);

    void DrawRectangle(
        Color color,
        RectangleF rectangle);

    void FillEllipse(
        Color color,
        RectangleF rectangle);

    void DrawText(
        string text,
        Color color,
        float x,
        float y);
}
