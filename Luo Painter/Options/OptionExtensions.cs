namespace Luo_Painter.Options
{
    public static class OptionExtensions
    {
        public static string GetResource(this OptionType type) => $"ms-appx:///Assets/Options/{type}.jpg";
    }
}