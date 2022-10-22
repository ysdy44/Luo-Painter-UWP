using Luo_Painter.Brushes;
using Luo_Painter.Shaders;
using Microsoft.Graphics.Canvas;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class PaintScrollViewer : UserControl, IInkParameter
    {

        bool ShaderCodeByteIsEnabled;

        byte[] BrushEdgeHardnessShaderCodeBytes;
        byte[] BrushEdgeHardnessWithTextureShaderCodeBytes;


        private async Task CreateResourcesAsync(ICanvasResourceCreatorWithDpi sender)
        {
            // Brush
            this.BrushEdgeHardnessShaderCodeBytes = await ShaderType.BrushEdgeHardness.LoadAsync();
            this.BrushEdgeHardnessWithTextureShaderCodeBytes = await ShaderType.BrushEdgeHardnessWithTexture.LoadAsync();

            this.InkRender = new CanvasRenderTarget(sender, InkPresenter.Width, InkPresenter.Height);
            this.Ink();

            this.ShaderCodeByteIsEnabled = true;
        }

    }
}