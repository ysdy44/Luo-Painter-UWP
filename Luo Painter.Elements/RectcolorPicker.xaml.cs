using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Luo_Painter.Elements
{
    public sealed partial class RectcolorPicker : UserControl, IColorPicker
    {
        //@Delegate
        public event EventHandler<Color> ColorChanged;

        Point Rectangle;
        Point Wheel;
        Vector4 HSV = Vector4.UnitW;

        #region DependencyProperty

        /// <summary> Gets or set the size for <see cref="RectcolorPicker"/>. </summary>
        private RectangleSize R
        {
            get => (RectangleSize)base.GetValue(RProperty);
            set => base.SetValue(RProperty, value);
        }
        /// <summary> Identifies the <see cref = "RectcolorPicker.R" /> dependency property. </summary>
        private static readonly DependencyProperty RProperty = DependencyProperty.Register(nameof(R), typeof(RectangleSize), typeof(RectcolorPicker), new PropertyMetadata(new RectangleSize(new WheelSize(320)), (sender, e) =>
        {
            RectcolorPicker control = (RectcolorPicker)sender;

            if (e.NewValue is RectangleSize value)
            {
                control.Reset(value);
            }
        }));

        #endregion

        //@Construct
        public RectcolorPicker()
        {
            this.InitializeComponent();
            base.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                this.R = new RectangleSize(new WheelSize(Math.Min(e.NewSize.Width, e.NewSize.Height)));
            };

            this.RectangleRectangle.ManipulationMode = ManipulationModes.All;
            this.RectangleRectangle.ManipulationStarted += (_, e) =>
            {
                this.Rectangle = e.Position;
                this.Move();

                //this.TextBlock.Text = ColorHelper.ToDisplayName(this.HSV.ToColor());
                //this.TextBlock.Visibility = Visibility.Visible;
            };
            this.RectangleRectangle.ManipulationDelta += (_, e) =>
            {
                this.Rectangle.X += e.Delta.Translation.X;
                this.Rectangle.Y += e.Delta.Translation.Y;
                this.Move();

                //this.TextBlock.Text = ColorHelper.ToDisplayName(this.HSV.ToColor());
            };
            this.RectangleRectangle.ManipulationCompleted += (_, e) =>
            {
                Color color = this.HSV.ToColor();
                this.Color(color);

                //this.TextBlock.Text = ColorHelper.ToDisplayName(this.HSV.ToColor());
                //this.TextBlock.Visibility = Visibility.Collapsed;
            };

            this.WheelPath.ManipulationMode = ManipulationModes.All;
            this.WheelPath.ManipulationStarted += (_, e) =>
            {
                this.Wheel = this.R.Size.Wheel(e.Position);
                this.Zoom();

                this.TextBlock.Text = $"{(int)this.HSV.Z} °";
                this.TextBlock.Visibility = Visibility.Visible;
            };
            this.WheelPath.ManipulationDelta += (_, e) =>
            {
                this.Wheel.X += e.Delta.Translation.X;
                this.Wheel.Y += e.Delta.Translation.Y;
                this.Zoom();

                this.TextBlock.Text = $"{(int)this.HSV.Z} °";
            };
            this.WheelPath.ManipulationCompleted += (_, e) =>
            {
                Color color = HSVExtensions.ToColor(this.HSV.Z);
                this.Stop(color);
                this.Color(this.HSV.ToColor());

                this.TextBlock.Text = $"{(int)this.HSV.Z} °";
                this.TextBlock.Visibility = Visibility.Collapsed;
            };
        }
    }

    public sealed partial class RectcolorPicker
    {
        public void Recolor(Color color)
        {
            this.HSV = color.ToHSV();

            this.Rectangle = this.R.Rectangle(this.HSV);

            this.Line(Math.PI * this.HSV.Z / 180f);
            this.Ellipse(this.HSV.X == 0f ? 0.5 : this.HSV.X, this.HSV.Y);
        }
        private void Reset(RectangleSize t)
        {
            this.Rectangle = t.Rectangle(this.HSV);

            this.Line(Math.PI * this.HSV.Z / 180f);
            this.Ellipse(this.HSV.X == 0f ? 0.5 : this.HSV.X, this.HSV.Y);
        }


        private void Move()
        {
            double v = this.R.V(this.Rectangle);
            double s = this.R.S(this.Rectangle);
            this.HSV.X = (float)s;
            this.HSV.Y = (float)v;

            this.Ellipse(s, v);

            Color color = this.HSV.ToColor();
            this.Color(color);
        }

        private void Zoom()
        {
            double h = WheelSize.VectorToH(this.Wheel);
            h += 360d;
            h %= 360d;
            this.HSV.Z = (float)h;

            Vector2 v = Vector2.Normalize(this.Wheel.ToVector2());
            this.Line(v.X, v.Y);

            Color color = HSVExtensions.ToColor(this.HSV.Z);
            this.Stop(color);
            this.Color(this.HSV.ToColor());
        }


        public void Left()
        {
            this.Rectangle.X -= 1;
            this.Move();
        }
        public void Right()
        {
            this.Rectangle.X += 1;
            this.Move();
        }

        public void Down()
        {
            this.Rectangle.Y += 1;
            this.Move();
        }
        public void Up()
        {
            this.Rectangle.Y -= 1;
            this.Move();
        }

        public void ZoomOut()
        {
            this.HSV.Z -= 1;
            this.HSV.Z += 360f;
            this.HSV.Z %= 360f;

            this.Line(this.HSV.Z * MathF.PI / 180d);

            Color color = HSVExtensions.ToColor(this.HSV.Z);
            this.Stop(color);
            this.Color(this.HSV.ToColor());
        }
        public void ZoomIn()
        {
            this.HSV.Z += 1;
            this.HSV.Z += 360f;
            this.HSV.Z %= 360f;

            this.Line(this.HSV.Z * MathF.PI / 180d);

            Color color = HSVExtensions.ToColor(this.HSV.Z);
            this.Stop(color);
            this.Color(this.HSV.ToColor());
        }
    }

    public sealed partial class RectcolorPicker
    {
        private void Color(Color color)
        {
            this.ColorChanged?.Invoke(this, color); // Delegate

            this.EllipseSolidColorBrush.Color = color;
        }

        private void Stop(Color color)
        {
            this.LineSolidColorBrush.Color = color;

            this.EndStop.Color = color;
        }

        private void Line(double h) => this.Line(Math.Cos(h), Math.Sin(h));
        private void Line(double cos, double sin)
        {
            this.BlackLine.X1 = this.WhiteLine.X1 = this.R.Size.XY1(cos);
            this.BlackLine.Y1 = this.WhiteLine.Y1 = this.R.Size.XY1(sin);

            this.BlackLine.X2 = this.WhiteLine.X2 = this.R.Size.XY2(cos);
            this.BlackLine.Y2 = this.WhiteLine.Y2 = this.R.Size.XY2(sin);
        }

        private void Ellipse(double s, double v)
        {
            double x = this.R.X(s);
            double y = this.R.Y(v);

            Canvas.SetLeft(this.BlackEllipse, x - 14);
            Canvas.SetTop(this.BlackEllipse, y - 14);
            Canvas.SetLeft(this.WhiteEllipse, x - 13);
            Canvas.SetTop(this.WhiteEllipse, y - 13);
        }
    }
}