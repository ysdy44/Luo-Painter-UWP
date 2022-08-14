using FanKit.Transformers;
using Luo_Painter.Elements;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Text;
using System;
using System.Numerics;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Text.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class TextLayoutsPage : Page
    {
        //@Converter
        private Vector2 ToPosition(Vector2 point) => Vector2.Transform(this.CanvasControl.Dpi.ConvertDipsToPixels(point), this.Transformer.GetInverseMatrix());
        private Vector2 ToPoint(Vector2 position) => this.CanvasControl.Dpi.ConvertPixelsToDips(Vector2.Transform(position, this.Transformer.GetMatrix()));


        readonly InputPane InputPane = InputPane.GetForCurrentView();
        readonly CoreTextEditContext EditContext = CoreTextServicesManager.GetForCurrentView().CreateEditContext();


        public bool InternalFocus { get; private set; }
        public string Text { get; set; } = "The quick brown fox jumps over the lazy dog.";
        public string TextRange(CoreTextRange range) => this.Text.Substring(range.StartCaretPosition, range.EndCaretPosition - range.StartCaretPosition);


        readonly CanvasTextFormat TextFormat = new CanvasTextFormat
        {
            FontSize = 150,
            HorizontalAlignment = CanvasHorizontalAlignment.Center,
            VerticalAlignment = CanvasVerticalAlignment.Center
        };
        CanvasTextLayout TextLayout;

        CanvasTextLayoutRegion TextLayoutRegion;


        CoreTextRange Selection;

        public int Start { get => this.Selection.StartCaretPosition; set => this.Selection.StartCaretPosition = value; }
        public int End { get => this.Selection.EndCaretPosition; set => this.Selection.EndCaretPosition = value; }

        public int First() => System.Math.Min(this.Start, this.End);
        public int Last() => System.Math.Max(this.Start, this.End);
        public int Length() => System.Math.Abs(this.End - this.Start);
        public bool HasSelection() => this.Start != this.End;


        Rect TextBounds;
        Rect ControlBounds;

        public TextLayoutsPage()
        {
            this.InitializeComponent();
            this.ConstructCanvas();
            this.ConstructOperator();
            this.ConstructCustomEditor();
            this.ConstructKey();
            this.ConstructFont();

            base.Unloaded += (s, e) => this.RemoveInternalFocus();
            base.Loaded += (s, e) => this.SetInternalFocus();
        }

        private void ConstructFont()
        {
            this.ComboBox.ItemsSource = CanvasTextFormat.GetSystemFontFamilies();
            this.ComboBox.SelectionChanged += (s, e) =>
            {
                if (this.ComboBox.SelectedItem is string item)
                {
                    if (this.HasSelection()) this.TextLayout.SetFontFamily(this.First(), this.Length(), item);
                    else this.TextLayout.SetFontFamily(0, this.Text.Length, item);
                    this.CanvasControl.Invalidate();
                }
            };

            this.BoldButton.Click += (s, e) =>
            {
                FontWeight w = (this.TextLayout.GetFontWeight(this.First()).Weight == FontWeights.Bold.Weight) ? FontWeights.Normal : FontWeights.Bold;
                if (this.HasSelection()) this.TextLayout.SetFontWeight(this.First(), this.Length(), w);
                else this.TextLayout.SetFontWeight(0, this.Text.Length, w);
                this.CanvasControl.Invalidate();
            };
            this.ItalicButton.Click += (s, e) =>
            {
                FontStyle f = (this.TextLayout.GetFontStyle(this.First()) == FontStyle.Italic) ? FontStyle.Normal : FontStyle.Italic;
                if (this.HasSelection()) this.TextLayout.SetFontStyle(this.First(), this.Length(), f);
                else this.TextLayout.SetFontStyle(0, this.Text.Length, f);
                this.CanvasControl.Invalidate();
            };
            this.UnderlineButton.Click += (s, e) =>
            {
                bool u = !this.TextLayout.GetUnderline(this.First());
                if (this.HasSelection()) this.TextLayout.SetUnderline(this.First(), this.Length(), u);
                else this.TextLayout.SetUnderline(0, this.Text.Length, u);
                this.CanvasControl.Invalidate();
            };
            this.StrikethroughButton.Click += (s, e) =>
            {
                bool u = !this.TextLayout.GetStrikethrough(this.First());
                if (this.HasSelection()) this.TextLayout.SetStrikethrough(this.First(), this.Length(), u);
                else this.TextLayout.SetStrikethrough(0, this.Text.Length, u);
                this.CanvasControl.Invalidate();
            };
        }

        private void ConstructKey()
        {
            this.CutButton.Click += (s, e) => this.Copy(true);
            this.CopyButton.Click += (s, e) => this.Copy(false);
            this.PasteButton.Click += (s, e) => this.Paste();

            this.AllButton.Click += (s, e) => this.Caret(0, this.Text.Length);
            this.UpButton.Click += (s, e) => this.Caret(0, 0);
            this.DownButton.Click += (s, e) => this.Caret(this.Text.Length, this.Text.Length);

            this.LeftButton.Click += (s, e) => this.Move(true, false);
            this.RightButton.Click += (s, e) => this.Move(false, false);
            this.ShiftLeftButton.Click += (s, e) => this.Move(true, true);
            this.ShiftRightButton.Click += (s, e) => this.Move(false, true);

            this.BackButton.Click += (s, e) =>
            {
                if (this.HasSelection())
                    this.DeleteSelection();
                else
                    this.Delete(true);
            };
            this.DeleteButton.Click += (s, e) =>
            {
                if (this.HasSelection())
                    this.DeleteSelection();
                else
                    this.Delete(false);
            };
        }

        private void ConstructCustomEditor()
        {
            this.EditContext.InputPaneDisplayPolicy = CoreTextInputPaneDisplayPolicy.Manual;
            this.EditContext.InputScope = CoreTextInputScope.Text;

            this.EditContext.FocusRemoved += (sender, args) => this.RemoveInternalFocusWorker();

            this.EditContext.SelectionRequested += (sender, args) => args.Request.Selection = this.Selection;
            this.EditContext.SelectionUpdating += (sender, args) => this.Selection = args.Selection;

            this.EditContext.TextRequested += (sender, args) =>
            {
                int startIndex = args.Request.Range.StartCaretPosition;
                int endIndex = System.Math.Min(args.Request.Range.EndCaretPosition, this.Text.Length);
                args.Request.Text = this.Text.Substring(startIndex, endIndex - startIndex);
            };
            this.EditContext.TextUpdating += (sender, args) =>
            {
                if (this.HasSelection()) this.Delete(true);

                this.Text =
                this.Text.Substring(0, args.Range.StartCaretPosition) +
                args.Text +
                this.Text.Substring(System.Math.Min(this.Text.Length, args.Range.EndCaretPosition));
                this.TextLayout = new CanvasTextLayout(this.CanvasControl, this.Text, this.TextFormat, this.Transformer.Width, this.Transformer.Height);

                this.Start = this.End = args.NewSelection.StartCaretPosition;
                this.TextBlock.Text = this.ToString(); this.CanvasControl.Invalidate();
            };

            this.EditContext.FormatUpdating += (sender, args) =>
            {
                if (args.TextColor is null) { }

                if (args.BackgroundColor is null) { }

                if (args.UnderlineType is null) { }
            };

            this.EditContext.CompositionStarted += (s, e) => { };
            this.EditContext.CompositionCompleted += (s, e) => { };

            this.EditContext.LayoutRequested += (sender, args) =>
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

                if (this.HasSelection())
                {
                    foreach (CanvasTextLayoutRegion description in this.TextLayout.GetCharacterRegions(this.First(), this.Length()))
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

                this.TextBlock.Text = this.ToString();
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

                this.TextBlock.Text = this.ToString();
                this.CanvasControl.Invalidate();
            };
            this.Operator.Single_Complete += (point, properties) =>
            {
                this.TextBlock.Text = this.ToString();
                this.NotifySelection();
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


        public void NotifySelection() => this.EditContext.NotifySelectionChanged(this.Selection);


        public void SetInternalFocus()
        {
            if (!this.InternalFocus)
            {
                this.InternalFocus = true;

                this.TextBlock.Text = this.ToString(); this.CanvasControl.Invalidate();

                this.EditContext.NotifyFocusEnter();
            }

            this.InputPane.TryShow();
        }

        public void RemoveInternalFocus()
        {
            if (this.InternalFocus)
            {
                this.EditContext.NotifyFocusLeave();

                this.RemoveInternalFocusWorker();
            }
        }

        private void RemoveInternalFocusWorker()
        {
            this.InternalFocus = false;

            this.InputPane.TryHide();

            this.TextBlock.Text = this.ToString(); this.CanvasControl.Invalidate();
        }


        private void Copy(bool isDeleteSelection)
        {
            if (this.HasSelection() is false) return;

            string text = this.Text.Substring(this.First(), this.Length());
            if (string.IsNullOrEmpty(text)) return;

            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(text);
            Clipboard.SetContent(dataPackage);

            if (isDeleteSelection) this.DeleteSelection();
        }

        public async void Paste()
        {
            DataPackageView dataPackageView = Clipboard.GetContent();
            if (dataPackageView.Contains(StandardDataFormats.Text) is false) return;

            string text = await dataPackageView.GetTextAsync();
            if (string.IsNullOrEmpty(text)) return;

            this.Text =
            this.Text.Substring(0, this.Start) +
            text +
            this.Text.Substring(System.Math.Min(this.Text.Length, this.End));
            this.TextLayout = new CanvasTextLayout(this.CanvasControl, this.Text, this.TextFormat, this.Transformer.Width, this.Transformer.Height);

            this.Start = this.End = this.Last() + text.Length;
            this.TextBlock.Text = this.ToString();
            this.CanvasControl.Invalidate();
        }

        private void Caret(int start, int end)
        {
            if (this.Start == start && this.End == end) return;

            this.Start = start;
            this.End = end;

            this.NotifySelection();
            this.TextBlock.Text = this.ToString();
            this.CanvasControl.Invalidate();
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

            this.NotifySelection();
            this.TextBlock.Text = this.ToString();
            this.CanvasControl.Invalidate();
        }

        private void DeleteSelection()
        {
            int first = this.First();
            int last = this.Last();

            if (first <= 0)
            {
                if (last >= this.Text.Length)
                    this.Text = string.Empty;
                else
                    this.Text = this.Text.Substring(last);

                this.End = this.Start = 0;
            }
            else
            {
                if (last >= this.Text.Length)
                    this.Text = this.Text.Substring(0, first);
                else
                    this.Text = this.Text.Substring(0, first) + this.Text.Substring(last);

                this.End = this.Start = first;
            }

            this.NotifySelection();
            this.TextLayout = new CanvasTextLayout(this.CanvasControl, this.Text, this.TextFormat, this.Transformer.Width, this.Transformer.Height);

            this.TextBlock.Text = this.ToString();
            this.CanvasControl.Invalidate();
        }

        private void Delete(bool isLeft)
        {
            int index = this.First();

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

            this.NotifySelection();
            this.TextLayout = new CanvasTextLayout(this.CanvasControl, this.Text, this.TextFormat, this.Transformer.Width, this.Transformer.Height);

            this.TextBlock.Text = this.ToString();
            this.CanvasControl.Invalidate();
        }

    }
}