using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Luo_Painter.HSVColorPickers
{
    public sealed partial class TricolorPicker : UserControl, IColorPicker
    {
        //@Delegate
        public event EventHandler<Color> ColorChanged;

        Point Triangle;
        Point Wheel;
        Vector4 HSV = Vector4.UnitW;

        #region DependencyProperty

        /// <summary> Gets or set the size for <see cref="TricolorPicker"/>. </summary>
        private TriangleTemplateSettings TriangleSize
        {
            get => (TriangleTemplateSettings)base.GetValue(TriangleSizeProperty);
            set => base.SetValue(TriangleSizeProperty, value);
        }
        /// <summary> Identifies the <see cref = "TricolorPicker.TriangleSize" /> dependency property. </summary>
        private static readonly DependencyProperty TriangleSizeProperty = DependencyProperty.Register(nameof(TriangleSize), typeof(TriangleTemplateSettings), typeof(TricolorPicker), new PropertyMetadata(new TriangleTemplateSettings(new WheelTemplateSettings(new CircleTemplateSettings(320))), (sender, e) =>
        {
            TricolorPicker control = (TricolorPicker)sender;

            if (e.NewValue is TriangleTemplateSettings value)
            {
                control.Reset(value);
            }
        }));

        #endregion

        //@Construct
        public TricolorPicker()
        {
            this.InitializeComponent();
            base.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                this.TriangleSize = new TriangleTemplateSettings(new WheelTemplateSettings(new CircleTemplateSettings(Math.Min(e.NewSize.Width, e.NewSize.Height))));
            };

            this.TrianglePath.ManipulationMode = ManipulationModes.All;
            this.TrianglePath.ManipulationStarted += (_, e) =>
            {
                this.Triangle = this.TriangleSize.Offset(e.Position);
                this.Move();

                //this.TextBlock.Text = ColorHelper.ToDisplayName(this.HSV.ToColor());
                //this.TextBlock.Visibility = Visibility.Visible;
            };
            this.TrianglePath.ManipulationDelta += (_, e) =>
            {
                this.Triangle.X += e.Delta.Translation.X;
                this.Triangle.Y += e.Delta.Translation.Y;
                this.Move();

                //this.TextBlock.Text = ColorHelper.ToDisplayName(this.HSV.ToColor());
            };
            this.TrianglePath.ManipulationCompleted += (_, e) =>
            {
                Color color = this.HSV.ToColor();
                this.Color(color);

                //this.TextBlock.Text = ColorHelper.ToDisplayName(this.HSV.ToColor());
                //this.TextBlock.Visibility = Visibility.Collapsed;
            };

            this.WheelPath.ManipulationMode = ManipulationModes.All;
            this.WheelPath.ManipulationStarted += (_, e) =>
            {
                this.Wheel = this.TriangleSize.WheelSize.CircleSize.Offset(e.Position);
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

    public sealed partial class TricolorPicker
    {
        public void Recolor(Color color)
        {
            this.HSV = color.ToHSV();

            this.Triangle = this.TriangleSize.Offset(this.HSV);

            this.Line(Math.PI * this.HSV.Z / 180f);
            this.Ellipse(this.HSV.X == 0f ? 0.5 : this.HSV.X, this.HSV.Y);
        }
        private void Reset(TriangleTemplateSettings size)
        {
            this.Triangle = size.Offset(this.HSV);

            this.Line(Math.PI * this.HSV.Z / 180f);
            this.Ellipse(this.HSV.X == 0f ? 0.5 : this.HSV.X, this.HSV.Y);
        }


        private void Move()
        {
            double v = this.TriangleSize.Value(this.Triangle);
            double s = this.TriangleSize.Saturation(this.Triangle, v);
            this.HSV.X = (float)s;
            this.HSV.Y = (float)v;

            this.Ellipse(s, v);

            Color color = this.HSV.ToColor();
            this.Color(color);
        }

        private void Zoom()
        {
            double h = WheelTemplateSettings.Atan2(this.Wheel);
            this.HSV.Z = (float)((h * 180d / Math.PI + 360d) % 360d);
            this.Line(h);

            Color color = HSVExtensions.ToColor(this.HSV.Z);
            this.Stop(color);
            this.Color(this.HSV.ToColor());
        }


        public void Left()
        {
            this.Triangle.X -= 1;
            this.Move();
        }
        public void Right()
        {
            this.Triangle.X += 1;
            this.Move();
        }

        public void Down()
        {
            this.Triangle.Y += 1;
            this.Move();
        }
        public void Up()
        {
            this.Triangle.Y -= 1;
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

    public sealed partial class TricolorPicker
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
            color.A = 0;
            this.StartStop.Color = color;
        }

        private void Line(double h)
        {
            Point xy1 = this.TriangleSize.WheelSize.CircleSize.XY(h, 1);
            this.BlackLine.X1 = this.WhiteLine.X1 = xy1.X;
            this.BlackLine.Y1 = this.WhiteLine.Y1 = xy1.Y;

            Point xy2 = this.TriangleSize.WheelSize.XY(h, 1);
            this.BlackLine.X2 = this.WhiteLine.X2 = xy2.X;
            this.BlackLine.Y2 = this.WhiteLine.Y2 = xy2.Y;
        }

        private void Ellipse(double s, double v)
        {
            double x = this.TriangleSize.X(s);
            double y = this.TriangleSize.Y(v);

            Canvas.SetLeft(this.BlackEllipse, x - 14);
            Canvas.SetTop(this.BlackEllipse, y - 14);
            Canvas.SetLeft(this.WhiteEllipse, x - 13);
            Canvas.SetTop(this.WhiteEllipse, y - 13);
        }
    }
}