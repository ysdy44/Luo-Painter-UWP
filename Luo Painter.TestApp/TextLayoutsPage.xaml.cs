using FanKit.Transformers;
using Luo_Painter.Elements;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Text;
using System.Numerics;
using Windows.Foundation;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Text.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class TextLayoutsPage : Page
    {
        //@Key
        private bool IsKeyDown(VirtualKey key) => Window.Current.CoreWindow.GetKeyState(key).HasFlag(CoreVirtualKeyStates.Down);
        private bool IsCtrl => this.IsKeyDown(VirtualKey.Control);
        private bool IsShift => this.IsKeyDown(VirtualKey.Shift);
        private bool IsAlt => this.IsKeyDown(VirtualKey.Menu);
        private bool IsSpace => this.IsKeyDown(VirtualKey.Space);

        //@Converter
        private Vector2 ToPosition(Vector2 point) => Vector2.Transform(this.CanvasControl.Dpi.ConvertDipsToPixels(point), this.Transformer.GetInverseMatrix());

        string Text = "The quick brown fox jumps over the lazy dog.";
        CanvasTextFormat TextFormat;
        CanvasTextLayout TextLayout;

        CanvasTextLayoutRegion TextLayoutRegion;

        CoreTextRange Selection;

        public bool HasSelection => this.Start != this.End;
        public int Start { get => this.Selection.StartCaretPosition; private set => this.Selection.StartCaretPosition = value; }
        public int End { get => this.Selection.EndCaretPosition; private set => this.Selection.EndCaretPosition = value; }

        public TextLayoutsPage()
        {
            this.InitializeComponent();
            this.ConstructCanvas();
            this.ConstructOperator();
            this.ConstructCustomEditor();
        }

        private void ConstructCustomEditor()
        {
            this.AllButton.Click += (s, e) => this.Caret(0, this.Text.Length);

            this.UpButton.Click += (s, e) => this.Caret(0, 0);
            this.DownButton.Click += (s, e) => this.Caret(this.Text.Length, this.Text.Length);

            this.LeftButton.Click += (s, e) => this.Move(true, false);
            this.RightButton.Click += (s, e) => this.Move(false, false);

            this.ShiftLeftButton.Click += (s, e) => this.Move(true, true);
            this.ShiftRightButton.Click += (s, e) => this.Move(false, true);

            this.BackButton.Click += (s, e) => this.Delete(true);
            this.DeleteButton.Click += (s, e) => this.Delete(false);
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

                this.TextFormat = new CanvasTextFormat
                {
                    FontSize = 150,
                    HorizontalAlignment = CanvasHorizontalAlignment.Center,
                    VerticalAlignment = CanvasVerticalAlignment.Center
                };
                this.TextLayout = new CanvasTextLayout(sender, this.Text, this.TextFormat, this.Transformer.Width, this.Transformer.Height);
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
                    int first = System.Math.Min(this.Start, this.End);
                    int length = System.Math.Abs(this.End - this.Start);

                    foreach (CanvasTextLayoutRegion description in this.TextLayout.GetCharacterRegions(first, length))
                    {
                        args.DrawingSession.FillRectangle(description.LayoutBounds, Colors.DodgerBlue);
                    }
                }
                else
                {
                    foreach (CanvasTextLayoutRegion description in this.TextLayout.GetCharacterRegions(this.Start, 1))
                    {
                        Rect r = description.LayoutBounds;
                        float l = (float)r.Left;
                        float t = (float)r.Top;
                        float b = (float)r.Bottom;
                        args.DrawingSession.DrawLine(l, t, l, b, Colors.Black, 2 * sender.Dpi.ConvertPixels());
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

                this.TextLayout.HitTest(position.X, position.Y, out this.TextLayoutRegion);

                if (position.X > this.TextLayoutRegion.LayoutBounds.X + this.TextLayoutRegion.LayoutBounds.Width / 2)
                    this.Start = this.End = 1 + this.TextLayoutRegion.CharacterIndex;
                else
                    this.Start = this.End = this.TextLayoutRegion.CharacterIndex;

                this.UpdateTextUI();
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                Vector2 position = this.ToPosition(point);

                this.TextLayout.HitTest(position, out this.TextLayoutRegion);

                if (position.X > this.TextLayoutRegion.LayoutBounds.X + this.TextLayoutRegion.LayoutBounds.Width / 2)
                    this.End = 1 + this.TextLayoutRegion.CharacterIndex;
                else
                    this.End = this.TextLayoutRegion.CharacterIndex;

                this.UpdateTextUI();
            };
            this.Operator.Single_Complete += (point, properties) =>
            {
                this.UpdateTextUI();
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

        private void Caret(int start, int end)
        {
            if (this.Start == start && this.End == end) return;

            this.Start = start;
            this.End = end;

            this.UpdateTextUI();
        }

        private void Move(bool isLeft, bool isShift)
        {
            if (isLeft)
            {
                if (this.End <= 0) return;
                this.End--;
            }
            else
            {
                if (this.End >= this.Text.Length) return;
                this.End++;
            }

            if (isShift is false)
            {
                this.Start = this.End;
            }

            this.UpdateTextUI();
        }

        private void Delete(bool isLeft)
        {
            if (this.HasSelection)
            {
                int first = System.Math.Min(this.Start, this.End);
                int last = System.Math.Max(this.Start, this.End);

                if (first <= 0)
                {
                    if (last >= this.Text.Length)
                        this.Text = string.Empty;
                    else
                        this.Text = this.Text.Substring(last + 1);

                    this.End = this.Start = 0;
                }
                else
                {
                    if (last >= this.Text.Length)
                        this.Text = this.Text.Substring(0, first);
                    else
                        this.Text = this.Text.Substring(0, first) + this.Text.Substring(last + 1);

                    this.End = this.Start = first;
                }
            }
            else
            {
                int index = System.Math.Min(this.Start, this.End);

                if (isLeft)
                {
                    if (index <= 0) return;
                    else if (index >= this.Text.Length)
                        this.Text = this.Text.Substring(0, index - 1);
                    else
                        this.Text = this.Text.Substring(0, index - 1) + this.Text.Substring(index);
                    this.Start = this.End = index - 1;
                }
                else
                {
                    if (index >= this.Text.Length) return;
                    else if (index <= 0)
                        this.Text = this.Text.Substring(index + 1);
                    else
                        this.Text = this.Text.Substring(0, index) + this.Text.Substring(index + 1);
                }
            }

            this.UpdateTextUI(true);
        }

        private void UpdateTextUI(bool isRelayout = false)
        {
            this.TextBlock.Text = $"Range: {this.Start}~{this.End}";
            if (isRelayout)
            {
                this.TextLayout = new CanvasTextLayout(this.CanvasControl, this.Text, this.TextFormat, this.Transformer.Width, this.Transformer.Height);
            }
            this.CanvasControl.Invalidate();
        }

    }
}