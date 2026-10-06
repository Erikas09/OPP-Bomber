using Bomber.Client.Interfaces;

namespace Bomber.Client.Rendering;

public abstract class Renderer
{
    protected readonly IDrawingContext DrawingContext;

    protected Renderer(IDrawingContext drawingContext)
    {
        DrawingContext = drawingContext;
    }
}
