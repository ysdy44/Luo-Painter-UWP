using Microsoft.Graphics.Canvas;
using Windows.UI.Xaml.Navigation;

namespace Luo_Painter.Brushes
{
    /// <summary>
    /// Represents an interface which can be <see cref="NavigationEventArgs.Parameter"/> for Ink.
    /// </summary>
    public interface IInkParameter
    {
        CanvasDevice CanvasDevice { get; }
        InkPresenter InkPresenter { get; }
        InkType InkType { get; set; }
    }
}