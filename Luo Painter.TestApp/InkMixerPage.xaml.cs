using Luo_Painter.Elements;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Linq;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed class InkMixer
    {
        
        public float Mix { get; set; } = 1;
        public float Wet { get; set; } = 10;
        public float Persistence { get; set; } = 0;
        public Vector3 Normalize()
        {
            float x = 1 - this.Mix;
            float y = this.Mix * (1 - this.Persistence);
            float z = this.Mix * this.Persistence;
            return new Vector3(x, y, z);
        }

        public Vector4 MixHdr { get; private set; } = Vector4.Zero;
        public Vector4 PersistenceHdr { get; private set; } = Vector4.Zero;
        public Vector4 GetMix(Vector4 colorHdr)
        {
            float x = 1 - this.Mix;
            float y = this.Mix * (1 - this.Persistence);
            float z = this.Mix * this.Persistence;
            return colorHdr * x + this.MixHdr * y + this.PersistenceHdr * z;
        }

        Vector4 MixWetHdr;
        Vector4 StartingMixWetHdr;

        Vector2 StartingMixPoint;

        public void ConstructMix(Vector2 point, CanvasBitmap bitmap)
        {
            this.MixWetHdr = Vector4.Zero;
            this.StartingMixWetHdr = Vector4.Zero;
            this.MixHdr = Vector4.Zero;
            this.PersistenceHdr = Vector4.Zero;

            this.StartingMixPoint = point;

            int x = (int)point.X;
            int y = (int)point.Y;

            if (x < 0) return;
            if (y < 0) return;
            if (x >= bitmap.SizeInPixels.Width) return;
            if (y >= bitmap.SizeInPixels.Height) return;

            Color color = bitmap.GetPixelColors(x, y, 1, 1).Single();
            if (color == Colors.Transparent) return;

            this.MixWetHdr =
            this.StartingMixWetHdr =
            this.MixHdr =
            this.PersistenceHdr = new Vector4(color.R, color.G, color.B, color.A) / 255f;
        }

        public void MixY(Vector2 point, float size, CanvasBitmap bitmap)
        {
            float length = Vector2.Distance(point, this.StartingMixPoint);
            float smooth = length / (size * this.Wet);
            if (smooth < 1)
            {
                // 1. Mix
                this.MixHdr = Vector4.Lerp(this.StartingMixWetHdr, this.MixWetHdr, smooth);
                return;
            }


            // 2. Fade
            this.MixHdr = this.MixWetHdr;
            this.StartingMixWetHdr = this.MixWetHdr;
            this.MixWetHdr.W = 0;

            this.StartingMixPoint = point;


            {
                int x = (int)point.X;
                int y = (int)point.Y;

                if (x < 0) return;
                if (y < 0) return;
                if (x >= bitmap.SizeInPixels.Width) return;
                if (y >= bitmap.SizeInPixels.Height) return;

                Color color = bitmap.GetPixelColors(x, y, 1, 1).Single();
                if (color == Colors.Transparent) return;


                // 3. Recolor
                //this.Color = this.C;
                //this.SC = this.C;
                this.MixWetHdr = new Vector4(color.R, color.G, color.B, color.A) / 255f;

                //this.SP = item;
            }
        }
    }

    public sealed partial class InkMixerPage : Page
    {
        //@Converter
        private string RoundConverter(double value) => $"{value:0}";

        //@Converter
        private Vector2 ToPosition(Vector2 point) => Vector2.Transform(this.CanvasControl.Dpi.ConvertDipsToPixels(point), this.Transformer.GetInverseMatrix());
        private Vector2 ToPoint(Vector2 position) => this.CanvasControl.Dpi.ConvertPixelsToDips(Vector2.Transform(position, this.Transformer.GetMatrix()));

        CanvasRenderTarget GrayAndWhite;
        CanvasRenderTarget BitmapLayer;

        readonly InkMixer Mixer = new InkMixer();

        Vector2 StartingPosition;
        Vector2 Position;
        float StartingPressure;
        float Pressure;

        Vector4 ColorHdr = Vector4.One;

        public InkMixerPage()
        {
            this.InitializeComponent();
            this.ConstructInkMixer();
            this.ConstructCanvas();
            this.ConstructOperator();
        }

        private void ConstructInkMixer()
        {
            this.MixSlider.ValueChanged += (s, e) =>
            {
                this.Mixer.Mix = (float)(e.NewValue / 100);

                Vector3 n = this.Mixer.Normalize();
                float x = n.X * 200;
                float y = n.Y * 200;
                float z = n.Z * 200;

                this.XRectangle.Width = x;
                Canvas.SetLeft(this.YRectangle, x);
                this.YRectangle.Width = y;
                Canvas.SetLeft(this.ZRectangle, x + y);
                this.ZRectangle.Width = z;
            };
            this.Wetlider.ValueChanged += (s, e) =>
            {
                this.Mixer.Wet = (float)e.NewValue;

                this.Canvas.Height =
                this.XRectangle.Height =
                this.YRectangle.Height =
                this.ZRectangle.Height =
                this.Mixer.Wet * 10;
            };
            this.MixSlider.ValueChanged += (s, e) =>
            {
                this.Mixer.Mix = (float)(e.NewValue / 100);

                Vector3 n = this.Mixer.Normalize();
                float x = n.X * 200;
                float y = n.Y * 200;
                float z = n.Z * 200;

                this.XRectangle.Width = x;
                Canvas.SetLeft(this.YRectangle, x);
                this.YRectangle.Width = y;
                Canvas.SetLeft(this.ZRectangle, x + y);
                this.ZRectangle.Width = z;
            };
            this.PersistenceSlider.ValueChanged += (s, e) =>
            {
                this.Mixer.Persistence = (float)(e.NewValue / 100);

                Vector3 n = this.Mixer.Normalize();
                float x = n.X * 200;
                float y = n.Y * 200;
                float z = n.Z * 200;

                this.XRectangle.Width = x;
                Canvas.SetLeft(this.YRectangle, x);
                this.YRectangle.Width = y;
                Canvas.SetLeft(this.ZRectangle, x + y);
                this.ZRectangle.Width = z;
            };

            this.ColorPicker.ColorChanged += (s, e) =>
            {
                Color color = e.NewColor;
                this.ColorHdr = new Vector4(color.R, color.G, color.B, color.A) / 255f;
            };
            this.ClearButton.Click += (s, e) =>
            {
                using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession())
                {
                    ds.Clear(Colors.Transparent);
                }

                this.CanvasControl.Invalidate(); // Invalidate
            };
        }

        private void ConstructCanvas()
        {
            this.CanvasControl.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                Vector2 size = this.CanvasControl.Dpi.ConvertDipsToPixels(e.NewSize.ToVector2());
                this.Transformer.ControlWidth = size.X;
                this.Transformer.ControlHeight = size.Y;
            };
            this.CanvasControl.CreateResources += (sender, args) =>
            {
                this.Transformer.Fit();

                //@DPI
                float scale = 128;
                this.BitmapLayer = new CanvasRenderTarget(sender, 1024, 1024, 96);
                this.GrayAndWhite = new CanvasRenderTarget(sender, 1024, 1024, 96);
                using (CanvasDrawingSession ds = this.GrayAndWhite.CreateDrawingSession())
                using (CanvasBitmap grayAndWhiteMesh = CanvasBitmap.CreateFromColors(sender, new Color[]
                {
                    Windows.UI.Color.FromArgb(255,240,127,33 ), Windows.UI.Color.FromArgb(255,255,203,19), Windows.UI.Color.FromArgb(255,149,60,52),
                    Windows.UI.Color.FromArgb(255,189,176,157), Windows.UI.Color.FromArgb(255,127,90,63), Windows.UI.Color.FromArgb(255,165,111,52),
                    Windows.UI.Color.FromArgb(255,104,39,19), Windows.UI.Color.FromArgb(255,255,255,255), Windows.UI.Color.FromArgb(255,84,84,76),
                }, 3, 3))
                using (ScaleEffect scaleEffect = new ScaleEffect
                {
                    Scale = new Vector2(scale),
                    InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                    Source = grayAndWhiteMesh
                })
                using (BorderEffect borderEffect = new BorderEffect
                {
                    ExtendX = CanvasEdgeBehavior.Wrap,
                    ExtendY = CanvasEdgeBehavior.Wrap,
                    Source = scaleEffect
                })
                {
                    ds.DrawImage(borderEffect);
                }
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                args.DrawingSession.Transform = this.Transformer.GetMatrix();

                args.DrawingSession.DrawImage(this.GrayAndWhite);
                args.DrawingSession.DrawImage(this.BitmapLayer);
            };
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, properties) =>
            {
                this.StartingPosition = this.Position = this.ToPosition(point);
                this.StartingPressure = this.Pressure = properties.Pressure;

                this.Mixer.ConstructMix(this.Position, this.GrayAndWhite);

                using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession())
                {
                    Vector4 hdr = 255f * this.Mixer.GetMix(this.ColorHdr);
                    ds.FillCircle(this.Position, 24, Windows.UI.Color.FromArgb((byte)hdr.W, (byte)hdr.X, (byte)hdr.Y, (byte)hdr.Z));
                }

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                this.Position = this.ToPosition(point);
                this.Pressure = properties.Pressure;

                float length = Vector2.Distance(this.StartingPosition, this.Position);
                using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession())
                {
                    for (int i = 0; i < length; i += 12)
                    {
                        float smooth = i / length;
                        Vector2 item = Vector2.Lerp(this.StartingPosition, this.Position, smooth);

                        this.Mixer.MixY(item, 24, this.GrayAndWhite);

                        Vector4 hdr = 255f * this.Mixer.GetMix(this.ColorHdr);
                        ds.FillCircle(item, 24, Windows.UI.Color.FromArgb((byte)hdr.W, (byte)hdr.X, (byte)hdr.Y, (byte)hdr.Z));
                    }
                }

                this.CanvasControl.Invalidate(); // Invalidate

                this.StartingPosition = this.Position;
                this.StartingPressure = this.Pressure;
            };
            this.Operator.Single_Complete += (point, properties) =>
            {
                this.CanvasControl.Invalidate(); // Invalidate
            };


            // Right
            this.Operator.Right_Start += (point) =>
            {
                this.Transformer.CacheMove(this.CanvasControl.Dpi.ConvertDipsToPixels(point));
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Right_Delta += (point) =>
            {
                this.Transformer.Move(this.CanvasControl.Dpi.ConvertDipsToPixels(point));
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Right_Complete += (point) =>
            {
                this.Transformer.Move(this.CanvasControl.Dpi.ConvertDipsToPixels(point));
                this.CanvasControl.Invalidate(); // Invalidate
            };


            // Double
            this.Operator.Double_Start += (center, space) =>
            {
                this.Transformer.CachePinch(this.CanvasControl.Dpi.ConvertDipsToPixels(center), this.CanvasControl.Dpi.ConvertDipsToPixels(space));

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Double_Delta += (center, space) =>
            {
                this.Transformer.Pinch(this.CanvasControl.Dpi.ConvertDipsToPixels(center), this.CanvasControl.Dpi.ConvertDipsToPixels(space));

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Double_Complete += (center, space) =>
            {
                this.CanvasControl.Invalidate(); // Invalidate
            };

            // Wheel
            this.Operator.Wheel_Changed += (point, space) =>
            {
                if (space > 0)
                    this.Transformer.ZoomIn(this.CanvasControl.Dpi.ConvertDipsToPixels(point), 1.05f);
                else
                    this.Transformer.ZoomOut(this.CanvasControl.Dpi.ConvertDipsToPixels(point), 1.05f);

                this.CanvasControl.Invalidate(); // Invalidate
            };
        }

    }
}