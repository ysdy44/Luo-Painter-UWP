using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Luo_Painter.Shaders
{
    public static class ShaderExtensions
    {

        public static async Task<byte[]> LoadAsync(this ShaderType type)
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(type.GetPath()));
            IBuffer buffer = await FileIO.ReadBufferAsync(file);
            return buffer.ToArray();
        }

        private static string GetPath(this ShaderType type)
        {
            switch (type)
            {
                case ShaderType.Liquefaction: return "ms-appx:///Luo Painter.Shaders/Liquefaction.bin";
                case ShaderType.GeneralBrush: return "ms-appx:///Luo Painter.Shaders/GeneralBrush.bin";
                case ShaderType.SprayGun: return "ms-appx:///Luo Painter.Shaders/SprayGun.txt";
                case ShaderType.GradientMapping: return "ms-appx:///Luo Painter.Shaders/GradientMapping.bin";
                case ShaderType.FreeTransform: return "ms-appx:///Luo Painter.Shaders/FreeTransform.bin";
                case ShaderType.DottedLine: return "ms-appx:///Luo Painter.Shaders/DottedLine.txt";
                case ShaderType.RippleEffect: return "ms-appx:///Luo Painter.Shaders/RippleEffect.bin";
                case ShaderType.Difference: return "ms-appx:///Luo Painter.Shaders/Difference.bin";
                default: return null;
            }
        }

    }
}