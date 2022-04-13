namespace Luo_Painter.Options
{
    public static class OptionExtensions
    {

        public static bool HasPreview(this OptionType type)
        {
            switch (type)
            {
                case OptionType.None:
                case OptionType.Gray:
                case OptionType.Invert:
                    return false;

                case OptionType.Fog:
                case OptionType.Sepia:
                case OptionType.Posterize:
                    return false;

                default:
                    return true;
            }
        }
        public static string GetResource(this OptionType type) => $"ms-appx:///Assets/Options/{type}.jpg";
    }
}