using FanKit.Transformers;
using Luo_Painter.Elements;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Text;
using System.Numerics;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Text.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class TextLayoutsPage : Page
    {
        //@Converter
        private Vector2 ToPosition(Vector2 point) => Vector2.Transform(this.CanvasControl.Dpi.ConvertDipsToPixels(point), this.Transformer.GetInverseMatrix());
        private Vector2 ToPoint(Vector2 position) => this.CanvasControl.Dpi.ConvertPixelsToDips(Vector2.Transform(position, this.Transformer.GetMatrix()));

        public int Start { get => this.CustomEditor.Start; private set => this.CustomEditor.Start = value; }
        public int End { get => this.CustomEditor.End; private set => this.CustomEditor.End = value; }

        readonly CustomEditor CustomEditor = new CustomEditor
        {
            Text = "The quick brown fox jumps over the lazy dog."
        };

        readonly CanvasTextFormat TextFormat = new CanvasTextFormat
        {
            FontSize = 150,
            HorizontalAlignment = CanvasHorizontalAlignment.Center,
            VerticalAlignment = CanvasVerticalAlignment.Center
        };
        CanvasTextLayout TextLayout;

        CanvasTextLayoutRegion TextLayoutRegion;

        Rect TextBounds;
        Rect ControlBounds;

        public TextLayoutsPage()
        {
            this.InitializeComponent();
            this.ConstructCanvas();
            this.ConstructOperator();
            this.ConstructCustomEditor();

            base.Unloaded += (s, e) => this.CustomEditor.RemoveInternalFocus();
            base.Loaded += (s, e) => this.CustomEditor.SetInternalFocus();
        }

        private void ConstructCustomEditor()
        {
            this.AllButton.Click += (s, e) => this.CustomEditor.SelectAll();
            this.UpButton.Click += (s, e) => this.CustomEditor.SelectToStart();
            this.DownButton.Click += (s, e) => this.CustomEditor.SelectToEnd();

            this.LeftButton.Click += (s, e) => this.CustomEditor.KeyLeft(false);
            this.RightButton.Click += (s, e) => this.CustomEditor.KeyRight(false);
            this.ShiftLeftButton.Click += (s, e) => this.CustomEditor.KeyLeft(true);
            this.ShiftRightButton.Click += (s, e) => this.CustomEditor.KeyRight(true);

            this.BackButton.Click += (s, e) => this.CustomEditor.KeyBack();
            this.DeleteButton.Click += (s, e) => this.CustomEditor.KeyDelete();

            this.CustomEditor.UpdateTextUIAction += () =>
            {
                this.TextBlock.Text = this.CustomEditor.ToString();
                this.CanvasControl.Invalidate();
            };
            this.CustomEditor.UpdateTextLayoutAction += () =>
            {
                this.TextLayout = new CanvasTextLayout(this.CanvasControl, this.CustomEditor.Text, this.TextFormat, this.Transformer.Width, this.Transformer.Height);
            };

            this.CustomEditor.LayoutRequested += (sender, args) =>
            {
                if (this.TextLayout is null) return;

                double scale = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
                this.ControlBounds = Window.Current.CoreWindow.Bounds;
                this.ControlBounds.X *= scale;
                this.ControlBounds.Y *= scale;
                this.ControlBounds.Width *= scale;
                this.ControlBounds.Height *= scale;
                args.Request.LayoutBounds.ControlBounds = this.ControlBounds;

                Vector2 p = this.ToPoint(this.TextLayout.GetCaretPosition(this.Start, false));
                this.TextBounds.X = this.ControlBounds.X + this.CanvasControl.Dpi.ConvertDipsToPixels(p.X);
                this.TextBounds.Y = this.ControlBounds.Y + this.CanvasControl.Dpi.ConvertDipsToPixels(p.Y);
                this.TextBounds.Width = this.TextBounds.Height = this.Transformer.Scale * (150 + 75);
                args.Request.LayoutBounds.TextBounds = this.TextBounds;
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

                this.TextLayout = new CanvasTextLayout(sender, this.CustomEditor.Text, this.TextFormat, this.Transformer.Width, this.Transformer.Height);
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

                if (this.CustomEditor.HasSelection())
                {
                    foreach (CanvasTextLayoutRegion description in this.TextLayout.GetCharacterRegions(this.CustomEditor.First(), this.CustomEditor.Length()))
                    {
                        args.DrawingSession.FillRectangle(description.LayoutBounds, Colors.DodgerBlue);
                    }
                }
                else
                {
                    Vector2 caret = this.TextLayout.GetCaretPosition(this.Start, false);
                    args.DrawingSession.DrawLine(caret.X, caret.Y, caret.X, caret.Y + 75 + 150, Colors.Black, 2 * sender.Dpi.ConvertPixels());
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

                this.TextLayout.HitTest(position, out this.TextLayoutRegion);

                if (position.X > this.TextLayoutRegion.LayoutBounds.X + this.TextLayoutRegion.LayoutBounds.Width / 2)
                    this.Start = this.End = 1 + this.TextLayoutRegion.CharacterIndex;
                else
                    this.Start = this.End = this.TextLayoutRegion.CharacterIndex;

                this.TextBlock.Text = this.CustomEditor.ToString();
                this.CanvasControl.Invalidate();
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                Vector2 position = this.ToPosition(point);

                this.TextLayout.HitTest(position, out this.TextLayoutRegion);

                if (position.X > this.TextLayoutRegion.LayoutBounds.X + this.TextLayoutRegion.LayoutBounds.Width / 2)
                    this.End = 1 + this.TextLayoutRegion.CharacterIndex;
                else
                    this.End = this.TextLayoutRegion.CharacterIndex;

                this.TextBlock.Text = this.CustomEditor.ToString();
                this.CanvasControl.Invalidate();
            };
            this.Operator.Single_Complete += (point, properties) =>
            {
                this.TextBlock.Text = this.CustomEditor.ToString();
                this.CustomEditor.NotifySelection();
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