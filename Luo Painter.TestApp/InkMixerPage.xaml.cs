using FanKit.Transformers;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
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
        public bool ReadyToDraw { get; set; }

        public Vector4 ColorHdrToDraw { get; private set; }

        Vector4 ColorHdr;
        Vector4 StartingColorHdr;

        Vector2 StartingPoint;

        public void Draw(Vector2 point, CanvasBitmap bitmap)
        {
            if (this.ReadyToDraw is false)
            {
                int x = (int)point.X;
                int y = (int)point.Y;

                if (x < 0) return;
                if (y < 0) return;
                if (x >= bitmap.SizeInPixels.Width) return;
                if (y >= bitmap.SizeInPixels.Height) return;

                Color color = bitmap.GetPixelColors(x, y, 1, 1).Single();
                if (color == Colors.Transparent) return;


                // 0. Initialize 
                this.ReadyToDraw = true;

                this.ColorHdrToDraw =
                this.StartingColorHdr =
                this.ColorHdr = new Vector4(color.R, color.G, color.B, color.A) / 255f;

                this.StartingPoint = point;
                return;
            }


            float length = Vector2.Distance(point, this.StartingPoint);
            float smooth = length / 122;
            if (smooth < 1)
            {
                // 1. Mix
                this.ColorHdrToDraw = Vector4.Lerp(this.StartingColorHdr, this.ColorHdr, smooth);
                return;
            }


            // 2. Fade
            this.ColorHdrToDraw = this.ColorHdr;
            this.StartingColorHdr = this.ColorHdr;
            this.ColorHdr.W = 0;

            this.StartingPoint = point;


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
                this.ColorHdr = new Vector4(color.R, color.G, color.B, color.A) / 255f;

                //this.SP = item;
            }
        }
    }

    public sealed partial class InkMixerPage : Page
    {

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
        float Amout = 0.5f;

        public InkMixerPage()
        {
            this.InitializeComponent();
            this.ConstructInkMixer();
            this.ConstructCanvas();
            this.ConstructOperator();
        }

        private void ConstructInkMixer()
        {
            this.Slider.ValueChanged += (s, e) =>
            {
                this.Amout = (float)(e.NewValue / 100);
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
                float scale = 256;
                this.BitmapLayer = new CanvasRenderTarget(sender, 1024, 1024, 96);
                this.GrayAndWhite = new CanvasRenderTarget(sender, 1024, 1024, 96);
                using (CanvasDrawingSession ds = this.GrayAndWhite.CreateDrawingSession())
                using (CanvasBitmap grayAndWhiteMesh = CanvasBitmap.CreateFromColors(sender, new Color[]
                {
                    Windows.UI.Colors.Black, Windows.UI.Colors.White,
                    Windows.UI.Colors.White, Windows.UI.Colors.Black,
                }, 2, 2))
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

                this.Mixer.ReadyToDraw = false;
                using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession())
                {
                    this.Mixer.Draw(this.Position, this.GrayAndWhite);

                    Vector4 co = Vector4.Lerp(this.Mixer.ColorHdrToDraw, Vector4.One, 0.5f);
                    Vector4 c = co * 255f;
                    ds.FillCircle(this.Position, 24, Windows.UI.Color.FromArgb((byte)c.W, (byte)c.X, (byte)c.Y, (byte)c.Z));
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

                        this.Mixer.Draw(item, this.GrayAndWhite);

                        Vector4 hdr = 255f * Vector4.Lerp(this.ColorHdr, this.Mixer.ColorHdrToDraw, this.Amout);
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