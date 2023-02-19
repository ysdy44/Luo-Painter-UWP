using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Luo_Painter.Elements
{
    /// <summary>
    /// A panel for indicator index. <para/>
    /// 
    /// -4: LeftTop <para/>
    /// -3: Top <para/>
    /// -2: RightTop <para/>
    ///
    /// -1: Left <para/>
    /// 0: Center <para/>
    /// 1: Right <para/>
    ///
    /// 2: LeftBottom <para/>
    /// 3: Bottom <para/>
    /// 4: RightBottom <para/>
    /// </summary>
    public sealed partial class IndicatorPanel : Button
    {
        //@Converter
        private double Unit => 32;
        private double Unit2 => Unit * 2;
        private double Unit4 => Unit * 4;
        private double Unit5 => Unit * 5;

        //@Content
        /// <summary>
        /// Gets the indicator index.
        /// </summary>
        public int Index { get; private set; }

        double X;
        double Y;

        //@Construct     
        /// <summary>
        /// Initializes a IndicatorPanel. 
        /// </summary>
        public IndicatorPanel()
        {
            this.InitializeComponent();
            base.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
            base.ManipulationStarted += (s, e) =>
            {
                this.HideStoryboard.Begin(); // Storyboard
            };
            base.ManipulationDelta += (s, e) =>
            {
                this.X += e.Delta.Translation.X;
                this.Y += e.Delta.Translation.Y;

                this.TranslateTransform.X = System.Math.Clamp(this.X, -this.Unit, this.Unit);
                this.TranslateTransform.Y = System.Math.Clamp(this.Y, -this.Unit, this.Unit);
            };
            base.ManipulationCompleted += (s, e) =>
            {
                this.Begin();
                this.ShowStoryboard.Begin(); // Storyboard
            };
            base.KeyDown += (s, e) =>
            {
                switch (e.OriginalKey)
                {
                    case VirtualKey.Left:
                        this.X -= this.Unit;
                        this.Begin();
                        break;
                    case VirtualKey.Up:
                        this.Y -= this.Unit;
                        this.Begin();
                        break;
                    case VirtualKey.Right:
                        this.X += this.Unit;
                        this.Begin();
                        break;
                    case VirtualKey.Down:
                        this.Y += this.Unit;
                        this.Begin();
                        break;
                    default:
                        break;
                }
            };
        }

        private void Begin()
        {
            int x = System.Math.Clamp((int)System.Math.Round(this.X / this.Unit, System.MidpointRounding.ToEven), -1, 1);
            int y = System.Math.Clamp((int)System.Math.Round(this.Y / this.Unit, System.MidpointRounding.ToEven), -1, 1);
            this.Index = x + 3 * y;

            this.X = x * this.Unit;
            this.Y = y * this.Unit;

            this.XDoubleAnimation.To = this.X;
            this.YDoubleAnimation.To = this.Y;

            this.Storyboard.Begin(); // Storyboard
        }
    }
}