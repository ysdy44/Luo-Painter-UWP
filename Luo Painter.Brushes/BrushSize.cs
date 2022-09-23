namespace Luo_Painter.Brushes
{
    public sealed class BrushSize
    {
        public double Size { get; set; } //= 0.7f;
        public string Text { get; set; } //= "0.7";

        public int Number { get; set; } //= 7;
        public double Preview { get; set; } //= 1.0764939309056;

        //@Static
        public static double Convert(double size) => BrushSize.ConvertCore(size * 10);
        private static double ConvertCore(double number)
        {
            double a = (number + 100000.0) / 1020.0 - 97;
            double b = (1.0 - 0.09 / (number)) * 600 - 590;
            return (a + b) / 1.2 - 1.7;
        }
    }
}