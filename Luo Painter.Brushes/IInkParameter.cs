using Microsoft.Graphics.Canvas;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Luo_Painter.Brushes
{
    /// <summary>
    /// Represents an interface which can be <see cref="NavigationEventArgs.Parameter"/> for Ink.
    /// </summary>
    public interface IInkParameter
    {
        CanvasDevice CanvasDevice { get; }

        InkType InkType { get; set; }
        InkPresenter InkPresenter { get; }

        Color Color { get; }
        Vector4 ColorHdr { get; }

        string TextureSelectedItem { get; }
        void ConstructTexture(string path);
        IAsyncOperation<ContentDialogResult> ShowTextureAsync();

        void Construct(IInkParameter item);
    }
}