using FanKit.Transformers;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Models;
using Luo_Painter.Strings;
using Luo_Painter.UI;
using Microsoft.Graphics.Canvas;
using Windows.UI;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        public void ConstructPattern()
        {
            // PatternGrid
            this.GridComboBox.SelectionChanged += (s, e) => this.PatternInvalidate();

            this.GridStrokeWidthSlider.ValueChanged += (s, e) => this.PatternInvalidate();
            this.GridStrokeWidthSlider.Click += (s, e) => this.NumberShowAt(this.GridStrokeWidthSlider, NumberPickerMode.GridStrokeWidthSlider);

            this.GridColumnSpanSlider.ValueChanged += (s, e) => this.PatternInvalidate();
            this.GridColumnSpanSlider.Click += (s, e) => this.NumberShowAt(this.GridColumnSpanSlider, NumberPickerMode.GridColumnSpanSlider);

            this.GridRowSpanSlider.ValueChanged += (s, e) => this.PatternInvalidate();
            this.GridRowSpanSlider.Click += (s, e) => this.NumberShowAt(this.GridRowSpanSlider, NumberPickerMode.GridRowSpanSlider);

            // Spotted
            this.SpottedRadiusSlider.ValueChanged += (s, e) => this.PatternInvalidate();
            this.SpottedRadiusSlider.Click += (s, e) => this.NumberShowAt(this.SpottedRadiusSlider, NumberPickerMode.SpottedRadiusSlider);

            this.SpottedSpanSlider.ValueChanged += (s, e) => this.PatternInvalidate();
            this.SpottedSpanSlider.Click += (s, e) => this.NumberShowAt(this.SpottedSpanSlider, NumberPickerMode.SpottedSpanSlider);

            this.SpottedFadeSlider.ValueChanged += (s, e) => this.PatternInvalidate();
            this.SpottedFadeSlider.Click += (s, e) => this.NumberShowAt(this.SpottedFadeSlider, NumberPickerMode.SpottedFadeSlider);
        }

        private void PatternInvalidate()
        {
            if (this.BitmapLayer is null) return;

            using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession(BitmapType.Temp))
            {
                ds.Clear(Colors.Transparent);
                this.DrawPattern(ds, this.OptionType, this.CreateTransform.Transformer, this.Color);
            }

            this.CanvasVirtualControl.Invalidate(); // Invalidate
        }

        private void CancelPatternTransform()
        {
            this.BitmapLayer = null;
            this.OptionType = this.ToolListView.SelectedType;
            this.ConstructAppBar(this.OptionType);

            this.CanvasAnimatedControl.Invalidate(false); // Invalidate
            this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.CanvasControl.Invalidate(); // Invalidate

            this.RaiseHistoryCanExecuteChanged();
        }
        private void PrimaryPatternTransform()
        {
            using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession())
            {
                this.DrawPattern(ds, this.OptionType, this.CreateTransform.Transformer, this.Color);
            }
            this.BitmapLayer.Hit(this.CreateTransform.Transformer);

            // History
            IHistory history = this.BitmapLayer.GetBitmapHistory();
            history.Title = this.OptionType.GetString();
            int removes = this.History.Push(history);

            this.BitmapLayer.Flush();
            this.BitmapLayer.RenderThumbnail();


            this.BitmapLayer = null;
            this.OptionType = this.ToolListView.SelectedType;
            this.ConstructAppBar(this.OptionType);

            this.CanvasAnimatedControl.Invalidate(false); // Invalidate
            this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.CanvasControl.Invalidate(); // Invalidate

            this.RaiseHistoryCanExecuteChanged();
        }


        private void PatternTransform_Delta()
        {
            if (this.BitmapLayer is null) return;

            if (this.CreateTransform.IsMove)
                this.CreateTransform.Transformer = this.CreateTransform.StartingTransformer + (this.Position - this.StartingPosition);
            else if (this.CreateTransform.Mode == default)
                this.CreateTransform.Transformer = new Transformer(this.StartingPosition, this.Position, this.IsCtrl, this.IsShift);
            else
                this.CreateTransform.Transformer = FanKit.Transformers.Transformer.Controller(this.CreateTransform.Mode, this.StartingPosition, this.Position, this.CreateTransform.StartingTransformer, this.IsShift, this.IsCtrl);

            using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession(BitmapType.Temp))
            {
                ds.Clear(Colors.Transparent);
                this.DrawPattern(ds, this.OptionType, this.CreateTransform.Transformer, this.Color);
            }

            this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.CanvasControl.Invalidate(); // Invalidate
        }


        private void Pattern_Start()
        {
            if (this.LayerSelectedItem is null)
            {
                this.ToastTip.Tip(TipType.NoLayer.GetString(), TipType.NoLayer.GetString(true));
                return;
            }

            this.BitmapLayer = this.LayerSelectedItem as BitmapLayer;
            if (this.BitmapLayer is null)
            {
                this.ToastTip.Tip(TipType.NotBitmapLayer.GetString(), TipType.NotBitmapLayer.GetString(true));
                return;
            }

            if (this.OptionType.HasPreview())
            {
                // PatternTransform_Start
                this.Transform_Start();
                return;
            }

            this.CreateTransform.IsMove = false;
            this.CreateTransform.Mode = default;
            this.CreateTransform.Transformer = new Transformer(this.StartingPosition, this.Position, this.IsCtrl, this.IsShift);

            this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Temp);


            this.OptionType = this.OptionType.ToPatternTransform();
            this.ConstructAppBar(this.OptionType);

            this.CanvasAnimatedControl.Invalidate(true); // Invalidate
            this.CanvasVirtualControl.Invalidate(); // Invalidate
        }

        public void DrawPattern(CanvasDrawingSession ds, OptionType type, ITransformerLTRB transformerLTRB, Color color)
        {
            switch (type)
            {
                case OptionType.PatternGrid:
                case OptionType.PatternGridTransform:
                    {
                        float strokeWidth = (float)this.GridStrokeWidthSlider.Value;

                        TransformerBorder border = new TransformerBorder(transformerLTRB);
                        float l = border.Left;
                        float t = border.Top;
                        float r = border.Right;
                        float b = border.Bottom;

                        if (this.GridColumnItem.IsSelected is false)
                        {
                            float span = (float)this.GridRowSpanSlider.Value;
                            float half = (span + strokeWidth) / 2;
                            for (float y = t + half; y < b - half; y += span)
                            {
                                ds.DrawLine(l, y, r, y, color, strokeWidth);
                            }
                        }

                        if (this.GridRowItem.IsSelected is false)
                        {
                            float span = (float)this.GridColumnSpanSlider.Value;
                            float half = (span + strokeWidth) / 2;
                            for (float x = l + half; x < r - half; x += span)
                            {
                                ds.DrawLine(x, t, x, b, color, strokeWidth);
                            }
                        }
                    }
                    break;
                case OptionType.PatternSpotted:
                case OptionType.PatternSpottedTransform:
                    {
                        float radius = (float)this.SpottedRadiusSlider.Value;
                        float span = (float)this.SpottedSpanSlider.Value;
                        float fade = (float)this.SpottedFadeSlider.Value / 100;

                        TransformerBorder border = new TransformerBorder(transformerLTRB);
                        float l = border.Left + radius;
                        float t = border.Top + radius;
                        float r = border.Right - radius;
                        float b = border.Bottom - radius;
                        float length = b - t;

                        if (fade == 0)
                        {
                            for (float y = t; y < b; y += span)
                            {
                                for (float x = l; x < r; x += span)
                                {
                                    ds.FillCircle(x, y, radius, color);
                                }
                            }
                        }
                        else
                        {
                            for (float y = t; y < b; y += span)
                            {
                                //@Debug
                                //float pect = (y - t) / length;
                                //float scale = 1 - fade * pect;
                                //float rs = radius * scale;
                                //@Release
                                float rs = radius - radius * fade * (y - t) / length;

                                for (float x = l; x < r; x += span)
                                {
                                    ds.FillCircle(x, y, rs, color);
                                }
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }

    }
}