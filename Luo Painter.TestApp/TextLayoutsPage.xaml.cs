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
    internal class TextFormatWithSelection2 : TextFormatWithSelection
    {
        public void Create(CanvasTextLayout textLayout)
        {
            foreach (var item in this.FontFamily)
            {
                foreach (var item2 in item.Value)
                {
                    textLayout.SetFontFamily(item2, 1, item.Key);
                }
            }
            foreach (var item in this.FontWeight) textLayout.SetFontWeight(item, 1, FontWeights.Bold);
            foreach (var item in this.FontStyle) textLayout.SetFontStyle(item, 1, Windows.UI.Text.FontStyle.Italic);
            foreach (var item in this.Underline) textLayout.SetUnderline(item, 1, true);
            foreach (var item in this.Strikethrough) textLayout.SetStrikethrough(item, 1, true);
        }


        public void SetFontFamily(string value, CoreTextRange range, CanvasTextLayout textLayout)
        {
            int first = range.First();
            if (this.FontFamily.ContainsKey(value) is false)
            {
                this.FontFamily.Add(value, new TextFormatItem());
            }
            this.FontFamily[value].Add(first, range.Last());

            textLayout.SetFontFamily(first, range.Length(), value);
        }


        public bool ToggleFontWeight(CoreTextRange range, CanvasTextLayout textLayout)
        {
            int first = range.First();
            if (textLayout.GetFontWeight(first).Weight == FontWeights.Bold.Weight)
            {
                this.FontWeight.Remove(first, range.Last());
                textLayout.SetFontWeight(first, range.Length(), FontWeights.Normal);
                return false;
            }
            else
            {
                this.FontWeight.Add(first, range.Last());
                textLayout.SetFontWeight(first, range.Length(), FontWeights.Bold);
                return true;
            }
        }

        public bool ToggleFontStyle(CoreTextRange range, CanvasTextLayout textLayout)
        {
            int first = range.First();
            if (textLayout.GetFontStyle(first) == Windows.UI.Text.FontStyle.Italic)
            {
                this.FontStyle.Remove(first, range.Last());
                textLayout.SetFontStyle(first, range.Length(), Windows.UI.Text.FontStyle.Normal);
                return false;
            }
            else
            {
                this.FontStyle.Add(first, range.Last());
                textLayout.SetFontStyle(first, range.Length(), Windows.UI.Text.FontStyle.Italic);
                return true;
            }
        }

        public bool ToggleUnderline(CoreTextRange range, CanvasTextLayout textLayout)
        {
            int first = range.First();
            if (textLayout.GetUnderline(first))
            {
                this.Underline.Remove(first, range.Last());
                textLayout.SetUnderline(first, range.Length(), false);
                return false;
            }
            else
            {
                this.Underline.Add(first, range.Last());
                textLayout.SetUnderline(first, range.Length(), true);
                return true;
            }
        }

        public bool ToggleStrikethrough(CoreTextRange range, CanvasTextLayout textLayout)
        {
            int first = range.First();
            if (textLayout.GetStrikethrough(first))
            {
                this.Strikethrough.Remove(first, range.Last());
                textLayout.SetStrikethrough(first, range.Length(), false);
                return false;
            }
            else
            {
                this.Strikethrough.Add(first, range.Last());
                textLayout.SetStrikethrough(first, range.Length(), true);
                return true;
            }
        }

    }

    public sealed partial class TextLayoutsPage : Page
    {
        //@Converter
        private Vector2 ToPosition(Vector2 point) => Vector2.Transform(this.CanvasControl.Dpi.ConvertDipsToPixels(point), this.Transformer.GetInverseMatrix());
        private Vector2 ToPoint(Vector2 position) => this.CanvasControl.Dpi.ConvertPixelsToDips(Vector2.Transform(position, this.Transformer.GetMatrix()));

        readonly InputPane InputPane = InputPane.GetForCurrentView();
        readonly CoreTextEditContext EditContext = CoreTextServicesManager.GetForCurrentView().CreateEditContext();
        bool InternalFocus;

        readonly TextFormatWithSelection2 Text = new TextFormatWithSelection2
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

        private void Create()
        {
            this.TextLayout?.Dispose();
            this.TextLayout = new CanvasTextLayout(this.CanvasControl, this.Text.Text, this.TextFormat, this.Transformer.Width, this.Transformer.Height);
            this.Text.Create(this.TextLayout);
        }

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
            this.ComboBox.ItemsSource = CanvasTextFormat.GetSystemFontFamilies(new string[] { base.Language });
            this.ComboBox.SelectionChanged += (s, e) =>
            {
                if (this.ComboBox.IsEnabled is false) return;
                if (this.ComboBox.SelectedItem is string item)
                {
                    this.Text.SetFontFamily(item, this.Text.Selection, this.TextLayout);
                    this.CanvasControl.Invalidate();
                }
            };
            this.BoldButton.Click += (s, e) =>
            {
                this.BoldButton.IsChecked = this.Text.ToggleFontWeight(this.Text.Selection, this.TextLayout);
                this.CanvasControl.Invalidate();
            };
            this.ItalicButton.Click += (s, e) =>
            {
                this.ItalicButton.IsChecked = this.Text.ToggleFontStyle(this.Text.Selection, this.TextLayout);
                this.CanvasControl.Invalidate();
            };
            this.UnderlineButton.Click += (s, e) =>
            {
                this.UnderlineButton.IsChecked = this.Text.ToggleUnderline(this.Text.Selection, this.TextLayout);
                this.CanvasControl.Invalidate();
            };
            this.StrikethroughButton.Click += (s, e) =>
            {
                this.StrikethroughButton.IsChecked = this.Text.ToggleStrikethrough(this.Text.Selection, this.TextLayout);
                this.CanvasControl.Invalidate();
            };
        }

        private void ConstructKey()
        {
            this.CutButton.Click += (s, e) =>
            {
                if (this.Text.Selection.HasSelection() is false) return;

                string text = this.Text.Text.Substring(this.Text.Selection.First(), this.Text.Selection.Length());
                if (string.IsNullOrEmpty(text)) return;

                DataPackage dataPackage = new DataPackage();
                dataPackage.SetText(text);
                Clipboard.SetContent(dataPackage);

                this.Text.DeleteSelection();
                this.NotifySelection();
                this.Create();
                this.CanvasControl.Invalidate();
            };
            this.CopyButton.Click += (s, e) =>
            {
                if (this.Text.Selection.HasSelection() is false) return;

                string text = this.Text.Text.Substring(this.Text.Selection.First(), this.Text.Selection.Length());
                if (string.IsNullOrEmpty(text)) return;

                DataPackage dataPackage = new DataPackage();
                dataPackage.SetText(text);
                Clipboard.SetContent(dataPackage);
            };
            this.PasteButton.Click += async (s, e) =>
            {
                DataPackageView dataPackageView = Clipboard.GetContent();
                if (dataPackageView.Contains(StandardDataFormats.Text) is false) return;

                string text = await dataPackageView.GetTextAsync();
                if (string.IsNullOrEmpty(text)) return;

                if (this.Text.Selection.HasSelection())
                {
                    this.Text.DeleteSelection();
                    this.NotifySelection();
                    this.Create();
                    this.CanvasControl.Invalidate();
                }

                if (this.Text.Selection.HasSelection())
                    this.Text.InsertSelection(text);
                else
                    this.Text.Insert(text);

                this.NotifySelection();
                this.Create();
                this.CanvasControl.Invalidate();
            };

            this.AllButton.Click += (s, e) =>
            {
                if (this.Text.SelectAll())
                {
                    this.NotifySelection();
                    this.CanvasControl.Invalidate();
                }
            };
            this.UpButton.Click += (s, e) =>
            {
                if (this.Text.SelectToStart())
                {
                    this.NotifySelection();
                    this.CanvasControl.Invalidate();
                }
            };
            this.DownButton.Click += (s, e) =>
            {
                if (this.Text.SelectToEnd())
                {
                    this.NotifySelection();
                    this.CanvasControl.Invalidate();
                }
            };

            this.LeftButton.Click += (s, e) =>
            {
                if (this.Text.Left())
                {
                    this.NotifySelection();
                    this.CanvasControl.Invalidate();
                }
            };
            this.RightButton.Click += (s, e) =>
            {
                if (this.Text.Right())
                {
                    this.NotifySelection();
                    this.CanvasControl.Invalidate();
                }
            };
            this.ShiftLeftButton.Click += (s, e) =>
            {
                if (this.Text.ShiftLeft())
                {
                    this.NotifySelection();
                    this.CanvasControl.Invalidate();
                }
            };
            this.ShiftRightButton.Click += (s, e) =>
            {
                if (this.Text.ShiftRight())
                {
                    this.NotifySelection();
                    this.CanvasControl.Invalidate();
                }
            };

            this.BackButton.Click += (s, e) =>
            {
                if (this.Text.Selection.HasSelection())
                {
                    this.Text.DeleteSelection();
                    this.NotifySelection();
                    this.Create();
                    this.CanvasControl.Invalidate();
                }
                else if (this.Text.Delete(true))
                {
                    this.NotifySelection();
                    this.Create();
                    this.CanvasControl.Invalidate();
                }
            };
            this.DeleteButton.Click += (s, e) =>
            {
                if (this.Text.Selection.HasSelection())
                {
                    this.Text.DeleteSelection();
                    this.NotifySelection();
                    this.Create();
                    this.CanvasControl.Invalidate();
                }
                else if (this.Text.Delete(false))
                {
                    this.NotifySelection();
                    this.Create();
                    this.CanvasControl.Invalidate();
                }
            };
            this.EnterButton.Click += (s, e) =>
            {
                if (this.Text.Selection.HasSelection())
                    this.Text.InsertSelection("\r\n");
                else
                    this.Text.Insert("\r\n");

                this.NotifySelection();
                this.Create();
                this.CanvasControl.Invalidate();
            };
        }

        private void ConstructCustomEditor()
        {
            this.EditContext.InputPaneDisplayPolicy = CoreTextInputPaneDisplayPolicy.Manual;
            this.EditContext.InputScope = CoreTextInputScope.Text;

            this.EditContext.FocusRemoved += (sender, args) => this.RemoveInternalFocusWorker();

            this.EditContext.SelectionRequested += (sender, args) => args.Request.Selection = this.Text.Selection;
            this.EditContext.SelectionUpdating += (sender, args) => this.Text.Selection = args.Selection;

            this.EditContext.TextRequested += (sender, args) => args.Request.Text = this.Text.Text.Substring(args.Request.Range.First(), args.Request.Range.Length(this.Text.Text.Length));
            this.EditContext.TextUpdating += (sender, args) =>
            {
                if (this.Text.Selection.HasSelection())
                {
                    this.Text.Delete(true);
                }

                this.Text.TextUpdating(args);

                this.NotifySelection();
                this.Create();
                this.CanvasControl.Invalidate();
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

                Vector2 p = this.ToPoint(this.TextLayout.GetCaretPosition(this.Text.Start, false));
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

                this.Create();
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

                if (this.Text.Selection.HasSelection())
                {
                    foreach (CanvasTextLayoutRegion description in this.TextLayout.GetCharacterRegions(this.Text.Selection.First(), this.Text.Selection.Length()))
                    {
                        args.DrawingSession.FillRectangle(description.LayoutBounds, Colors.DodgerBlue);
                    }
                }
                else
                {
                    Vector2 caret = this.TextLayout.GetCaretPosition(this.Text.Start, false);
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

                if (this.TextLayoutRegion.LayoutBounds.HitTest(position))
                    this.Text.Start = this.Text.End = 1 + this.TextLayoutRegion.CharacterIndex;
                else
                    this.Text.Start = this.Text.End = this.TextLayoutRegion.CharacterIndex;

                this.NotifySelection();
                this.CanvasControl.Invalidate();
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                Vector2 position = this.ToPosition(point);

                this.TextLayout.HitTest(position, out this.TextLayoutRegion);

                if (this.TextLayoutRegion.LayoutBounds.HitTest(position))
                    this.Text.End = 1 + this.TextLayoutRegion.CharacterIndex;
                else
                    this.Text.End = this.TextLayoutRegion.CharacterIndex;

                this.NotifySelection();
                this.CanvasControl.Invalidate();
            };
            this.Operator.Single_Complete += (point, properties) =>
            {
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


        public void NotifySelection(bool isNotify = true)
        {
            if (isNotify)
            {
                this.EditContext.NotifySelectionChanged(this.Text.Selection);

                this.AppBar.IsEnabled = this.Text.Selection.HasSelection();
                this.TextBlock.Text = this.Text.ToString();
            }

            int first = this.Text.Selection.First();
            this.ComboBox.IsEnabled = false;
            this.ComboBox.SelectedItem = this.TextLayout.GetFontFamily(first);
            this.ComboBox.IsEnabled = true;
            this.BoldButton.IsChecked = this.TextLayout.GetFontWeight(first).Weight == FontWeights.Bold.Weight;
            this.ItalicButton.IsChecked = this.TextLayout.GetFontStyle(first) == FontStyle.Italic;
            this.UnderlineButton.IsChecked = this.TextLayout.GetUnderline(first);
            this.StrikethroughButton.IsChecked = this.TextLayout.GetStrikethrough(first);
        }

        public void SetInternalFocus()
        {
            if (!this.InternalFocus)
            {
                this.InternalFocus = true;

                this.CanvasControl.Invalidate();

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

            this.CanvasControl.Invalidate();
        }

    }
}