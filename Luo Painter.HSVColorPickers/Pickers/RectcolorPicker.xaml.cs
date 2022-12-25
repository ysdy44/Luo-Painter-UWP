using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Luo_Painter.HSVColorPickers
{
    public sealed partial class RectcolorPicker : UserControl, IColorPicker
    {
        //@Delegate
        public event EventHandler<Color> ColorChanged;
        public event EventHandler<Color> ColorChangedCompleted;

        public ColorType Type => ColorType.Rectcolor;

        Point Rectangle;
        Point Wheel;
        Vector4 HSV = Vector4.UnitW;

        #region DependencyProperty

        /// <summary> Gets or set the size for <see cref="RectcolorPicker"/>. </summary>
        private RectangleTemplateSettings RectangleSize
        {
            get => (RectangleTemplateSettings)base.GetValue(RectangleSizeProperty);
            set => base.SetValue(RectangleSizeProperty, value);
        }
        /// <summary> Identifies the <see cref = "RectcolorPicker.RectangleSize" /> dependency property. </summary>
        private static readonly DependencyProperty RectangleSizeProperty = DependencyProperty.Register(nameof(RectangleSize), typeof(RectangleTemplateSettings), typeof(RectcolorPicker), new PropertyMetadata(new RectangleTemplateSettings(new WheelTemplateSettings(new CircleTemplateSettings(320))), (sender, e) =>
        {
            RectcolorPicker control = (RectcolorPicker)sender;

            if (e.NewValue is RectangleTemplateSettings value)
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

                this.RectangleSize = new RectangleTemplateSettings(new WheelTemplateSettings(new CircleTemplateSettings(Math.Min(e.NewSize.Width, e.NewSize.Height))));
            };

            this.RectangleRectangle.ManipulationMode = ManipulationModes.All;
            this.RectangleRectangle.ManipulationStarted += (_, e) =>
            {
                if (e.PointerDeviceType == default)
                {
                    this.Transform.ScaleX = 1.8;
                    this.Transform.ScaleY = 1.8;
                }

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
                this.ColorChangedCompleted?.Invoke(this, color); // Delegate

                if (e.PointerDeviceType == default)
                {
                    this.Transform.ScaleX = 1;
                    this.Transform.ScaleY = 1;
                }

                //this.TextBlock.Text = ColorHelper.ToDisplayName(this.HSV.ToColor());
                //this.TextBlock.Visibility = Visibility.Collapsed;
            };

            this.WheelPath.ManipulationMode = ManipulationModes.All;
            this.WheelPath.ManipulationStarted += (_, e) =>
            {
                this.Wheel = this.RectangleSize.WheelSize.CircleSize.Offset(e.Position);
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
                this.Stop(HSVExtensions.ToColor(this.HSV.Z));

                Color color = this.HSV.ToColor();
                this.ColorChangedCompleted?.Invoke(this, color); // Delegate

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

            this.Rectangle = this.RectangleSize.Offset(this.HSV);

            this.Line(Math.PI * this.HSV.Z / 180f);
            this.Ellipse(this.HSV.X == 0f ? 0.5 : this.HSV.X, this.HSV.Y);

            this.Stop(HSVExtensions.ToColor(this.HSV.Z));
            this.EllipseSolidColorBrush.Color = color;
        }
        private void Reset(RectangleTemplateSettings t)
        {
            this.Rectangle = t.Offset(this.HSV);

            this.Line(Math.PI * this.HSV.Z / 180f);
            this.Ellipse(this.HSV.X == 0f ? 0.5 : this.HSV.X, this.HSV.Y);
        }


        private void Move()
        {
            double v = this.RectangleSize.Value(this.Rectangle);
            double s = this.RectangleSize.Saturation(this.Rectangle);
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

            this.Stop(HSVExtensions.ToColor(this.HSV.Z));
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

            this.Stop(HSVExtensions.ToColor(this.HSV.Z));
            this.Color(this.HSV.ToColor());
        }
        public void ZoomIn()
        {
            this.HSV.Z += 1;
            this.HSV.Z += 360f;
            this.HSV.Z %= 360f;
            this.Line(this.HSV.Z * MathF.PI / 180d);

            this.Stop(HSVExtensions.ToColor(this.HSV.Z));
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

        private void Line(double h)
        {
            Point xy1 = this.RectangleSize.WheelSize.CircleSize.XY(h, 1);
            this.BlackLine.X1 = this.WhiteLine.X1 = xy1.X;
            this.BlackLine.Y1 = this.WhiteLine.Y1 = xy1.Y;

            Point xy2 = this.RectangleSize.WheelSize.XY(h, 1);
            this.BlackLine.X2 = this.WhiteLine.X2 = xy2.X;
            this.BlackLine.Y2 = this.WhiteLine.Y2 = xy2.Y;
        }

        private void Ellipse(double s, double v)
        {
            double x = this.RectangleSize.X(s);
            double y = this.RectangleSize.Y(v);

            Canvas.SetLeft(this.BlackEllipse, x - 14);
            Canvas.SetTop(this.BlackEllipse, y - 14);
            Canvas.SetLeft(this.WhiteEllipse, x - 13);
            Canvas.SetTop(this.WhiteEllipse, y - 13);
        }
    }
}