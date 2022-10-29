using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Shaders;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class InkMixerPage : Page
    {
        //@Converter
        private string RoundConverter(double value) => $"{value:0}";

        //@Converter
        private Vector2 ToPosition(Vector2 point) => Vector2.Transform(this.CanvasControl.Dpi.ConvertDipsToPixels(point), this.Transformer.GetInverseMatrix());
        private Vector2 ToPoint(Vector2 position) => this.CanvasControl.Dpi.ConvertPixelsToDips(Vector2.Transform(position, this.Transformer.GetMatrix()));

        byte[] BrushEdgeHardnessShaderCodeBytes;

        BitmapLayer BitmapLayer;

        Vector2 StartingPosition;
        Vector2 Position;

        Vector4 ColorHdr = Vector4.One;

        public float Mix { get; set; } = 1;
        public float Wet { get; set; } = 12;
        public float Persistence { get; set; } = 0;

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
                this.Mix = (float)(e.NewValue / 100);

                Vector3 n = this.BitmapLayer.MixNormalize(this.Mix, this.Persistence);
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
                this.Wet = (float)(e.NewValue);
            };
            this.PersistenceSlider.ValueChanged += (s, e) =>
            {
                this.Persistence = (float)(e.NewValue / 100);

                Vector3 n = this.BitmapLayer.MixNormalize(this.Mix, this.Persistence);
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
                this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Temp);

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

                float scale = 128;
                using (CanvasBitmap grayAndWhiteMesh = CanvasBitmap.CreateFromColors(this.CanvasControl, new Color[]
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
                    this.BitmapLayer = new BitmapLayer(sender, borderEffect, 1024, 1024);
                }

                args.TrackAsyncAction(this.CreateResourcesAsync().AsAsyncAction());
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                args.DrawingSession.Transform = this.Transformer.GetMatrix();

                args.DrawingSession.DrawImage(this.BitmapLayer[BitmapType.Source]);
                args.DrawingSession.DrawImage(this.BitmapLayer[BitmapType.Temp]);
            };
        }

        private async Task CreateResourcesAsync()
        {
            // Brush
            this.BrushEdgeHardnessShaderCodeBytes = await ShaderType.BrushEdgeHardness.LoadAsync();
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, properties) =>
            {
                this.StartingPosition = this.Position = this.ToPosition(point);

                StrokeCap cap = new StrokeCap(this.StartingPosition, 1, 24);
                this.BitmapLayer.CapDrawShaderBrushEdgeHardness(cap, this.BrushEdgeHardnessShaderCodeBytes, this.ColorHdr, this.Mix, this.Wet, this.Persistence, 0, 1, true, true);

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                this.Position = this.ToPosition(point);

                StrokeSegment segment = new StrokeSegment(this.StartingPosition, this.Position, 1, 1, 24, 0.25f);

                if (segment.InRadius) return;
                this.BitmapLayer.SegmentDrawShaderBrushEdgeHardness(segment, this.BrushEdgeHardnessShaderCodeBytes, this.ColorHdr, this.Mix, this.Wet, this.Persistence, 0, 1, true, true);

                this.CanvasControl.Invalidate(); // Invalidate

                this.StartingPosition = this.Position;
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