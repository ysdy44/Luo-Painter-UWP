using Luo_Painter.Elements;
using Luo_Painter.HSVColorPickers;
using Luo_Painter.Models;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Luo_Painter
{
    internal enum ColorPickerMode
    {
        None = 0,

        Case0 = 1,
        Case1 = 2,
        Case2 = 3,
        Case3 = 4,
    }

    public sealed partial class DrawPage
    {

        ColorPickerMode ColorPickerMode;
        Point StartingStraw;

        private void ColorShowAt(IColorBase color, ColorPickerMode mode = default)
        {
            this.ColorPickerMode = mode;
            this.ColorPicker.Recolor(color.Color);
            this.ColorFlyout.ShowAt(color.PlacementTarget);
        }

        public void ConstructColorPicker()
        {
            this.ColorPicker.StrawClick += async (s, e) =>
            {
                if (this.ClickEyedropper is null) return;

                this.StartingStraw.X = -200;
                this.StartingStraw.Y = -200;

                FrameworkElement placementTarget = this.ColorFlyout.Target;
                this.ColorFlyout.Hide();
                {
                    bool result = await this.ClickEyedropper.RenderAsync();
                    if (result is false) return;

                    this.ClickEyedropper.Move(this.StartingStraw);

                    Window.Current.CoreWindow.PointerCursor = null;
                    this.ClickEyedropper.Visibility = Visibility.Visible;
                    {
                        Color color = await this.ClickEyedropper.OpenAsync();
                        this.ColorPicker.Recolor(color);
                        this.ColorPicker.OnColorChanged(color);
                    }
                    Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
                    this.ClickEyedropper.Visibility = Visibility.Collapsed;
                }
                this.ColorFlyout.ShowAt(placementTarget);
            };
            this.ColorPicker.ColorChanged += (s, e) =>
            {
                if (this.ClickEyedropper.Visibility != default)
                {
                    if (this.ColorFlyout.IsOpen is false) return;
                }

                switch (this.OptionType)
                {
                    case OptionType.GradientMapping:
                        this.GradientMappingSelector.SetCurrentColor(e);
                        this.ResetGradientMapping();
                        this.CanvasVirtualControl.Invalidate(); // Invalidate
                        break;

                    case OptionType.ColorMatch:
                        switch (this.ColorPickerMode)
                        {
                            case ColorPickerMode.Case0:
                                this.ColorMatchSourceButton.SetColor(e);
                                this.ColorMatchSourceButton.SetColorHdr(e);

                                this.CanvasVirtualControl.Invalidate(); // Invalidate
                                break;
                            case ColorPickerMode.Case1:
                                this.ColorMatchDestinationButton.SetColor(e);
                                this.ColorMatchDestinationButton.SetColorHdr(e);

                                this.CanvasVirtualControl.Invalidate(); // Invalidate
                                break;
                            default:
                                break;
                        }
                        break;

                    case OptionType.Shadow:
                        this.ShadowColorButton.SetColor(e);
                        this.ShadowColorButton.SetColorHdr(e);

                        this.CanvasVirtualControl.Invalidate(); // Invalidate
                        break;

                    case OptionType.ChromaKey:
                        this.ChromaKeyColorButton.SetColor(e);
                        this.ChromaKeyColorButton.SetColorHdr(e);

                        this.CanvasVirtualControl.Invalidate(); // Invalidate
                        break;
                    case OptionType.Tint:
                        this.TintColorButton.SetColor(e);
                        this.TintColorButton.SetColorHdr(e);

                        this.CanvasVirtualControl.Invalidate(); // Invalidate
                        break;

                    case OptionType.Threshold:
                        switch (this.ColorPickerMode)
                        {
                            case ColorPickerMode.Case0:
                                this.ThresholdColor0Button.SetColor(e);
                                this.ThresholdColor0Button.SetColorHdr(e);

                                this.CanvasVirtualControl.Invalidate(); // Invalidate
                                break;
                            case ColorPickerMode.Case1:
                                this.ThresholdColor1Button.SetColor(e);
                                this.ThresholdColor1Button.SetColorHdr(e);

                                this.CanvasVirtualControl.Invalidate(); // Invalidate
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            };
        }

    }
}