using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Luo_Painter.Shaders
{
    public sealed class ShaderUri : Uri
    {
        public ShaderUri(ShaderType type) : base(ShaderUri.GetPath(type))
        {
        }

        public async Task<byte[]> LoadAsync()
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(this);
            IBuffer buffer = await FileIO.ReadBufferAsync(file);
            return buffer.ToArray();
        }

        //@Static
        public static string GetPath(ShaderType type)
        {
            switch (type)
            {
                case ShaderType.Liquefaction: return "ms-appx:///Luo Painter.Shaders/Liquefaction.bin";
                case ShaderType.GeneralBrush: return "ms-appx:///Luo Painter.Shaders/GeneralBrush.bin";
                case ShaderType.SprayGun: return "ms-appx:///Luo Painter.Shaders/SprayGun.txt";
                case ShaderType.GradientMapping: return "ms-appx:///Luo Painter.Shaders/GradientMapping.bin";
                case ShaderType.DottedLine: return "ms-appx:///Luo Painter.Shaders/DottedLine.txt";
                case ShaderType.RippleEffect: return "ms-appx:///Luo Painter.Shaders/RippleEffect.bin";
                case ShaderType.Difference: return "ms-appx:///Luo Painter.Shaders/Difference.bin";
                case ShaderType.ColorMatch: return "ms-appx:///Luo Painter.Shaders/ColorMatch.txt";
                case ShaderType.DottedLineTransform: return "ms-appx:///Luo Painter.Shaders/DottedLineTransform.bin";
                case ShaderType.BlackOrWhite: return "ms-appx:///Luo Painter.Shaders/BlackOrWhite.txt";
                case ShaderType.SDFMorph: return "ms-appx:///Luo Painter.Shaders/SDFMorph.txt";
                case ShaderType.FilmicEffect: return "ms-appx:///Luo Painter.Shaders/FilmicEffect.bin";
                case ShaderType.LuminanceHeatmapEffect: return "ms-appx:///Luo Painter.Shaders/LuminanceHeatmapEffect.bin";
                case ShaderType.ReinhardEffect: return "ms-appx:///Luo Painter.Shaders/ReinhardEffect.bin";
                case ShaderType.SdrOverlayEffect: return "ms-appx:///Luo Painter.Shaders/SdrOverlayEffect.bin";
                case ShaderType.LalphaMask: return "ms-appx:///Luo Painter.Shaders/LalphaMask.txt";
                case ShaderType.RalphaMask: return "ms-appx:///Luo Painter.Shaders/RalphaMask.bin";
                case ShaderType.DisplacementLiquefaction: return "ms-appx:///Luo Painter.Shaders/DisplacementLiquefaction.bin";
                case ShaderType.BrushEdgeHardness: return "ms-appx:///Luo Painter.Shaders/BrushEdgeHardness.bin";
                case ShaderType.BrushEdgeHardnessWithTexture: return "ms-appx:///Luo Painter.Shaders/BrushEdgeHardnessWithTexture.bin";
                case ShaderType.LalphaMaskEffect: return "ms-appx:///Luo Painter.Shaders/LalphaMaskEffect.bin";
                case ShaderType.RalphaMaskEffect: return "ms-appx:///Luo Painter.Shaders/RalphaMaskEffect.bin";
                case ShaderType.Threshold: return "ms-appx:///Luo Painter.Shaders/Threshold.bin";
                default: throw new NullReferenceException($"The shader for {type} is null.");
            }
        }
    }
}