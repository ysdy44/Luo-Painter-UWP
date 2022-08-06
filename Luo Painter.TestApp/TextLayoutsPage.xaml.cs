using FanKit.Transformers;
using Luo_Painter.Elements;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Text;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class TextLayoutsPage : Page
    {

        //@Converter
        private Vector2 ToPosition(Vector2 point) => Vector2.Transform(this.CanvasControl.Dpi.ConvertDipsToPixels(point), this.Transformer.GetInverseMatrix());

        const string TestString = "The quick brown fox jumps over the lazy dog.";
        CanvasTextLayout TextLayout;

        bool HasHit;
        bool HasSelection;
        int StartIndex = 0;
        int EndIndex = 0;
        CanvasTextLayoutRegion TextLayoutRegion;

        public TextLayoutsPage()
        {
            this.InitializeComponent();
            this.ConstructCanvas();
            this.ConstructOperator();

            this.ClearButton.Click += (s, e) =>
            {
                this.HasHit = false;
                this.HasSelection = false;
                this.StartIndex = 0;
                this.EndIndex = 0;

                this.TextBlock.Text = $"Range: 0~0  Hit: false";

                this.CanvasControl.Invalidate();
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

                this.TextLayout = new CanvasTextLayout(sender, TextLayoutsPage.TestString, new CanvasTextFormat
                {
                    FontSize = 150,
                    HorizontalAlignment = CanvasHorizontalAlignment.Center,
                    VerticalAlignment = CanvasVerticalAlignment.Center
                }, this.Transformer.Width, this.Transformer.Height);
                TextLayout.SetBrush(0, TestString.Length, null);
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                args.DrawingSession.DrawCard(new ColorSourceEffect
                {
                    Color = Colors.White
                }, this.Transformer, Colors.Black);
                args.DrawingSession.Transform = this.Transformer.GetMatrix();

                if (this.HasSelection)
                {
                    int firstIndex = System.Math.Min(this.StartIndex, this.EndIndex);
                    int length = System.Math.Abs(this.EndIndex - this.StartIndex) + 1;
                    foreach (CanvasTextLayoutRegion description in this.TextLayout.GetCharacterRegions(firstIndex, length))
                    {
                        args.DrawingSession.FillRectangle(description.LayoutBounds, Colors.DodgerBlue);
                    }
                }
                else if (this.HasHit)
                {
                    foreach (CanvasTextLayoutRegion description in this.TextLayout.GetCharacterRegions(this.StartIndex, 1))
                    {
                        Rect r = description.LayoutBounds;
                        float l = (float)r.Left;
                        float t = (float)r.Top;
                        float b = (float)r.Bottom;
                        args.DrawingSession.DrawLine(l, t, l, b, Colors.Black, sender.Dpi.ConvertPixels());
                    }
                }

                args.DrawingSession.DrawTextLayout(this.TextLayout, 0, 0, Colors.Black);
                args.DrawingSession.DrawRectangle(this.TextLayout.DrawBounds, Colors.Orange, sender.Dpi.ConvertPixels());
                args.DrawingSession.DrawRectangle(this.TextLayout.LayoutBounds, Colors.DodgerBlue, sender.Dpi.ConvertPixels());
                //args.DrawingSession.DrawRectangle(textLayout.LayoutBoundsIncludingTrailingWhitespace, Colors.White, sender.Dpi.ConvertPixels());
            };
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, properties) =>
            {
                Vector2 position = this.ToPosition(point);

                this.HasHit = this.TextLayout.HitTest(position.X, position.Y, out this.TextLayoutRegion);
                this.StartIndex = this.EndIndex = this.TextLayoutRegion.CharacterIndex;
                this.HasSelection = false;

                this.CanvasControl.Invalidate();
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                Vector2 position = this.ToPosition(point);

                bool hasHit = this.TextLayout.HitTest(position.X, position.Y, out this.TextLayoutRegion);
                int index = this.TextLayoutRegion.CharacterIndex;
                bool hasSelection = this.StartIndex != this.EndIndex;

                if (this.HasHit != hasHit || this.EndIndex != index || this.HasSelection != hasSelection)
                {
                    this.TextBlock.Text = $"Range: {this.StartIndex}~{this.EndIndex}  Hit: {this.HasHit}";
                    this.CanvasControl.Invalidate();
                }

                this.HasHit = hasHit;
                this.EndIndex = index;
                this.HasSelection = hasSelection;
            };
            this.Operator.Single_Complete += (point, properties) =>
            {
                this.CanvasControl.Invalidate();
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