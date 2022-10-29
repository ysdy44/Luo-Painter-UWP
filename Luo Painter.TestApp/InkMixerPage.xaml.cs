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

        public float X { get; set; } = 0;
        public float Y { get; set; } = 1;
        public float Z { get; set; } = 0;
        public Vector3 Normalize()
        {
            float length = this.X + this.Y + this.Z;
            if (length == 0d) return Vector3.Zero;
            return new Vector3(this.X, this.Y, this.Z) / length;
        }


        public Vector4 XHdr { get; set; } = Vector4.Zero;
        public Vector4 YHdr { get; private set; } = Vector4.Zero;
        public Vector4 ZHdr { get; private set; } = Vector4.Zero;
        public Vector4 Mix()
        {
            float length = this.X + this.Y + this.Z;
            if (length == 0d) return Vector4.Zero;
            return (this.XHdr * this.X + this.YHdr * this.Y + this.ZHdr * this.Z) / length;
        }


        Vector4 WetYHdr;
        Vector4 StartingWetYHdr;

        Vector2 StartingMixPoint;

        public void ConstructYZ(Vector2 point, CanvasBitmap bitmap)
        {
            this.WetYHdr = Vector4.Zero;
            this.StartingWetYHdr = Vector4.Zero;
            this.YHdr = Vector4.Zero;
            this.ZHdr = Vector4.Zero;

            this.StartingMixPoint = point;

            int x = (int)point.X;
            int y = (int)point.Y;

            if (x < 0) return;
            if (y < 0) return;
            if (x >= bitmap.SizeInPixels.Width) return;
            if (y >= bitmap.SizeInPixels.Height) return;

            Color color = bitmap.GetPixelColors(x, y, 1, 1).Single();
            if (color == Colors.Transparent) return;

            this.WetYHdr =
            this.StartingWetYHdr =
            this.YHdr =
            this.ZHdr = new Vector4(color.R, color.G, color.B, color.A) / 255f;
        }

        public void MixY(Vector2 point, CanvasBitmap bitmap)
        {
            float length = Vector2.Distance(point, this.StartingMixPoint);
            float smooth = length / 122;
            if (smooth < 1)
            {
                // 1. Mix
                this.YHdr = Vector4.Lerp(this.StartingWetYHdr, this.WetYHdr, smooth);
                return;
            }


            // 2. Fade
            this.YHdr = this.WetYHdr;
            this.StartingWetYHdr = this.WetYHdr;
            this.WetYHdr.W = 0;

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
                this.WetYHdr = new Vector4(color.R, color.G, color.B, color.A) / 255f;

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
            this.ZSlider.ValueChanged += (s, e) =>
            {
                this.Mixer.Z = (float)(e.NewValue / 100);
                Vector3 normalize = this.Mixer.Normalize();
                float x = normalize.X * 200;
                float y = normalize.Y * 200;
                float z = normalize.Z * 200;

                this.XRectangle.Width = x;
                Canvas.SetLeft(this.YRectangle, x);
                this.YRectangle.Width = y;
                Canvas.SetLeft(this.ZRectangle, x + y);
                this.ZRectangle.Width = z;
            };
            this.YSlider.ValueChanged += (s, e) =>
            {
                this.Mixer.Y = (float)(e.NewValue / 100);
                Vector3 normalize = this.Mixer.Normalize();
                float x = normalize.X * 200;
                float y = normalize.Y * 200;
                float z = normalize.Z * 200;

                this.XRectangle.Width = x;
                Canvas.SetLeft(this.YRectangle, x);
                this.YRectangle.Width = y;
                Canvas.SetLeft(this.ZRectangle, x + y);
                this.ZRectangle.Width = z;
            };
            this.XSlider.ValueChanged += (s, e) =>
            {
                this.Mixer.X = (float)(e.NewValue / 100);
                Vector3 normalize = this.Mixer.Normalize();
                float x = normalize.X * 200;
                float y = normalize.Y * 200;
                float z = normalize.Z * 200;

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

                Vector2 item = this.Position;
                this.Mixer.XHdr = this.ColorHdr;
                this.Mixer.ConstructYZ(item, this.GrayAndWhite);

                using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession())
                {
                    Vector4 hdr = 255f * this.Mixer.Mix();
                    ds.FillCircle(item, 24, Windows.UI.Color.FromArgb((byte)hdr.W, (byte)hdr.X, (byte)hdr.Y, (byte)hdr.Z));
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

                        this.Mixer.MixY(item, this.GrayAndWhite);

                        Vector4 hdr = 255f * this.Mixer.Mix();
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