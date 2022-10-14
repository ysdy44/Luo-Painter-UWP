using Luo_Painter.Blends;
using Luo_Painter.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class BrushPage : Page
    {

        public void ConstructInk1()
        {
            this.ComboBox.SelectionChanged += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                if (this.ComboBox.SelectedItem is InkType type)
                {
                    if (this.InkPresenter.Type == type) return;
                    this.InkPresenter.Type = type;

                    this.Type = type;
                    this.InkType = this.InkPresenter.GetType();

                    if (this.ShaderCodeByteIsEnabled is false) return;
                    System.Threading.Tasks.Task.Run(this.InkAsync);
                }
            };


            this.SizeSlider.ValueChanged += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                double size = this.SizeRange.ConvertXToY(e.NewValue);
                this.InkPresenter.Size = (float)size;

                if (this.ShaderCodeByteIsEnabled is false) return;
                System.Threading.Tasks.Task.Run(this.InkAsync);
            };
            this.OpacitySlider.ValueChanged += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                double opacity = System.Math.Clamp(e.NewValue / 100, 0, 1);
                this.InkPresenter.Opacity = (float)opacity;
                this.InkType = this.InkPresenter.GetType();
            };
            this.SpacingSlider.ValueChanged += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                double spacing = this.SpacingRange.ConvertXToY(e.NewValue);
                double spacing2 = System.Math.Clamp(spacing / 100, 0.1, 4);
                this.InkPresenter.Spacing = (float)spacing2;

                if (this.ShaderCodeByteIsEnabled is false) return;
                System.Threading.Tasks.Task.Run(this.InkAsync);
            };
            this.FlowSlider.ValueChanged += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                double flow = System.Math.Clamp(e.NewValue / 100, 0, 1);
                this.InkPresenter.Flow = (float)flow;

                if (this.ShaderCodeByteIsEnabled is false) return;
                System.Threading.Tasks.Task.Run(this.InkAsync);
            };


            this.IgnoreSizePressureButton.Toggled += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                this.InkPresenter.IgnoreSizePressure = this.IgnoreSizePressureButton.IsOn;

                if (this.ShaderCodeByteIsEnabled is false) return;
                System.Threading.Tasks.Task.Run(this.InkAsync);
            };
            this.IgnoreFlowPressureButton.Toggled += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                this.InkPresenter.IgnoreFlowPressure = this.IgnoreFlowPressureButton.IsOn;

                if (this.ShaderCodeByteIsEnabled is false) return;
                System.Threading.Tasks.Task.Run(this.InkAsync);
            };

            this.ShapeListBox.SelectionChanged += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;

                switch (this.ShapeListBox.SelectedIndex)
                {
                    case 0:
                        this.InkPresenter.Shape = PenTipShape.Circle;
                        this.InkPresenter.IsStroke = false;

                        if (this.ShaderCodeByteIsEnabled is false) break;
                        lock (this.InkLocker) this.Ink();
                        break;
                    case 1:
                        this.InkPresenter.Shape = PenTipShape.Circle;
                        this.InkPresenter.IsStroke = true;

                        if (this.ShaderCodeByteIsEnabled is false) break;
                        lock (this.InkLocker) this.Ink();
                        break;
                    case 2:
                        this.InkPresenter.Shape = PenTipShape.Rectangle;
                        this.InkPresenter.IsStroke = false;

                        if (this.ShaderCodeByteIsEnabled is false) break;
                        lock (this.InkLocker) this.Ink();
                        break;
                    case 3:
                        this.InkPresenter.Shape = PenTipShape.Rectangle;
                        this.InkPresenter.IsStroke = true;

                        if (this.ShaderCodeByteIsEnabled is false) break;
                        lock (this.InkLocker) this.Ink();
                        break;
                    default:
                        break;
                }
            };


            this.NoneRadioButton.Checked += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                this.InkPresenter.Hardness = BrushEdgeHardness.None;

                if (this.ShaderCodeByteIsEnabled is false) return;
                lock (this.InkLocker) this.Ink();
            };
            this.CosineRadioButton.Checked += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                this.InkPresenter.Hardness = BrushEdgeHardness.Cosine;

                if (this.ShaderCodeByteIsEnabled is false) return;
                lock (this.InkLocker) this.Ink();
            };
            this.QuadraticRadioButton.Checked += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                this.InkPresenter.Hardness = BrushEdgeHardness.Quadratic;

                if (this.ShaderCodeByteIsEnabled is false) return;
                lock (this.InkLocker) this.Ink();
            };
            this.CubeRadioButton.Checked += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                this.InkPresenter.Hardness = BrushEdgeHardness.Cube;

                if (this.ShaderCodeByteIsEnabled is false) return;
                lock (this.InkLocker) this.Ink();
            };
            this.QuarticRadioButton.Checked += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                this.InkPresenter.Hardness = BrushEdgeHardness.Quartic;

                if (this.ShaderCodeByteIsEnabled is false) return;
                lock (this.InkLocker) this.Ink();
            };

            this.BasisRadioButton.Checked += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                this.InkPresenter.Mode = InkType.None;
                this.InkType = this.InkPresenter.GetType();

                if (this.ShaderCodeByteIsEnabled is false) return;
                lock (this.InkLocker) this.Ink();
            };
            this.MixRadioButton.Checked += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                this.InkPresenter.Mode = InkType.Mix;
                this.InkType = this.InkPresenter.GetType();

                if (this.ShaderCodeByteIsEnabled is false) return;
                lock (this.InkLocker) this.Ink();
            };
            this.BlendRadioButton.Checked += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;

                this.InkPresenter.Mode = InkType.Blend;
                this.InkType = this.InkPresenter.GetType();

                if (this.ShaderCodeByteIsEnabled is false) return;
                lock (this.InkLocker) this.Ink();
            };

            this.BlendModeComboBox.SelectionChanged += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;

                if (this.BlendModeComboBox.SelectedItem is BlendEffectMode item)
                {
                    this.InkPresenter.BlendMode = item;

                    if (this.ShaderCodeByteIsEnabled is false) return;
                    lock (this.InkLocker) this.Ink();
                }
            };
        }

    }
}