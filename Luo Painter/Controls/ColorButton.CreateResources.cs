﻿using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers.Models;
using Luo_Painter.Shaders;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class ColorButton : Button, IInkParameter
    {

        byte[] LiquefactionShaderCodeBytes;

        byte[] BrushEdgeHardnessShaderCodeBytes;
        byte[] BrushEdgeHardnessWithTextureShaderCodeBytes;


        private void CreateResources(int width, int height)
        {
            this.BitmapLayer = new BitmapLayer
            (
                this.CanvasDevice,
                System.Math.Min(width, FileUtil.MaxImageSize),
                System.Math.Min(height, FileUtil.MaxImageSize)
            );
        }


        private async Task CreateResourcesAsync()
        {
            this.LiquefactionShaderCodeBytes = await ShaderType.Liquefaction.LoadAsync();

            // Brush
            this.BrushEdgeHardnessShaderCodeBytes = await ShaderType.BrushEdgeHardness.LoadAsync();
            this.BrushEdgeHardnessWithTextureShaderCodeBytes = await ShaderType.BrushEdgeHardnessWithTexture.LoadAsync();
        }

    }
}