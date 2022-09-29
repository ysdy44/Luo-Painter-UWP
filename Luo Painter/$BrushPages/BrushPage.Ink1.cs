using Luo_Painter.Blends;
using Luo_Painter.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class BrushPage : Page
    {

        public void ConstructInk1()
        {
            this.SizeSlider.ValueChanged += (s, e) =>
            {
                double size = this.SizeRange.ConvertXToY(e.NewValue);
                this.InkPresenter.Size = (float)size;
                this.InkCanvasControl.Invalidate();
            };
            this.OpacitySlider.ValueChanged += (s, e) =>
            {
                double opacity = System.Math.Clamp(e.NewValue / 100, 0, 1);
                this.InkPresenter.Opacity = (float)opacity;
                this.InkCanvasControl.Opacity = opacity;
                this.InkType = this.InkPresenter.GetType();
            };
            this.SpacingSlider.ValueChanged += (s, e) =>
            {
                double spacing = this.SpacingRange.ConvertXToY(e.NewValue);
                double spacing2 = System.Math.Clamp(spacing / 100, 0.1, 4);
                this.InkPresenter.Spacing = (float)spacing2;
                this.InkCanvasControl.Invalidate();
            };

            this.NoneRadioButton.Checked += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                this.InkPresenter.Hardness = BrushEdgeHardness.None;
                this.InkCanvasControl.Invalidate();
            };
            this.CosineRadioButton.Checked += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                this.InkPresenter.Hardness = BrushEdgeHardness.Cosine;
                this.InkCanvasControl.Invalidate();
            };
            this.QuadraticRadioButton.Checked += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                this.InkPresenter.Hardness = BrushEdgeHardness.Quadratic;
                this.InkCanvasControl.Invalidate();
            };
            this.CubeRadioButton.Checked += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                this.InkPresenter.Hardness = BrushEdgeHardness.Cube;
                this.InkCanvasControl.Invalidate();
            };
            this.QuarticRadioButton.Checked += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                this.InkPresenter.Hardness = BrushEdgeHardness.Quartic;
                this.InkCanvasControl.Invalidate();
            };

            this.BlendRadioButton.Checked += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;

                switch (this.BlendModeComboBox.SelectedIndex)
                {
                    case 0:
                        this.InkPresenter.Mode = InkType.None;
                        this.InkType = this.InkPresenter.GetType();
                        this.InkCanvasControl.Invalidate();
                        break;
                    default:
                        this.InkPresenter.Mode = InkType.Blend;
                        this.InkType = this.InkPresenter.GetType();
                        this.InkCanvasControl.Invalidate();
                        break;
                }
            };
            this.BlurRadioButton.Checked += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                this.InkPresenter.Mode = InkType.Blur;
                this.InkType = this.InkPresenter.GetType();
                this.InkCanvasControl.Invalidate();
            };
            this.MixRadioButton.Checked += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                this.InkPresenter.Mode = InkType.Mix;
                this.InkType = this.InkPresenter.GetType();
                this.InkCanvasControl.Invalidate();
            };
            this.MosaicRadioButton.Checked += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                this.InkPresenter.Mode = InkType.Mosaic;
                this.InkType = this.InkPresenter.GetType();
                this.InkCanvasControl.Invalidate();
            };

            this.BlendModeComboBox.SelectionChanged += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;

                if (this.BlendModeComboBox.SelectedItem is int item)
                {
                    BlendEffectMode blendMode = (BlendEffectMode)item;
                    if (blendMode.IsDefined())
                    {
                        this.InkPresenter.Mode = InkType.Blend;
                        this.InkPresenter.BlendMode = blendMode;
                    }
                    else
                    {
                        this.InkPresenter.Mode = InkType.None;
                    }

                    this.InkType = this.InkPresenter.GetType();
                    this.InkCanvasControl.Invalidate();
                }
            };
        }

    }
}