using System;

namespace Luo_Painter.Brushes
{
    public sealed class BrushTextureUri : Uri
    {
        public BrushTextureUri(string name) : base($@"ms-appx:///Luo Painter.Brushes/Textures/{name}Texture.png")
        {
        }
    }

    public static class BrushExtensions
    {
        public static string GetTile(this uint tile) => $"ms-appx:///Luo Painter.Brushes/Tiles/{tile}.jpg";
        public const string DeaultTile = "ms-appx:///Luo Painter.Brushes/Tiles/0000000000.jpg";

        public static string GetTexture(this string path) => $@"ms-appx:///Luo Painter.Brushes/Textures/{path}Texture.png";
        public static string GetTextureSource(this string path) => $@"Luo Painter.Brushes/Textures/{path}.png";
    }
}