using Luo_Painter.Brushes;
using Luo_Painter.Shaders;
using Microsoft.Graphics.Canvas;
using System.Threading.Tasks;

namespace Luo_Painter.Controls
{
    public sealed partial class PaintScrollViewer
    {

        byte[] BrushEdgeHardnessShaderCodeBytes;
        byte[] BrushEdgeHardnessWithTextureShaderCodeBytes;


        private async Task CreateResourcesAsync(ICanvasResourceCreatorWithDpi sender)
        {
            // Brush
            this.BrushEdgeHardnessShaderCodeBytes = await ShaderType.BrushEdgeHardness.LoadAsync();
            this.BrushEdgeHardnessWithTextureShaderCodeBytes = await ShaderType.BrushEdgeHardnessWithTexture.LoadAsync();

            this.InkRender = new CanvasRenderTarget(sender, InkPresenter.Width, InkPresenter.Height);
            this.Ink();
        }

    }
}