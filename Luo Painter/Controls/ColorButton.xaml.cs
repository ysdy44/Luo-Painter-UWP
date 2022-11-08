using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.Controls
{
    internal class ColorCommand : RelayCommand<Color> { }

    public sealed partial class ColorButton : Button, IInkParameter
    {
        //@Converter
        private Symbol ColorSpectrumShapeSymbolConverter(bool? value) => value == true ? Symbol.Target : Symbol.Stop;
        private ColorSpectrumShape ColorSpectrumShapeConverter(int value) => value is 0 ? ColorSpectrumShape.Box : ColorSpectrumShape.Ring;
        private ColorSpectrumComponents ColorSpectrumComponentsConverter(int value)
        {
            switch (value)
            {
                case 0: return ColorSpectrumComponents.SaturationValue;
                case 1: return ColorSpectrumComponents.HueSaturation;
                default: return ColorSpectrumComponents.HueValue;
            }
        }

        //@Delegate
        public event TypedEventHandler<ColorPicker, ColorChangedEventArgs> ColorChanged;

        //@Content
        public Eyedropper Eyedropper { get; set; }
        public ClickEyedropper ClickEyedropper { get; set; }

        public CanvasDevice CanvasDevice => this.InkParameter.CanvasDevice;

        //@Task
        readonly object Locker = new object();
        BitmapLayer BitmapLayer { get; set; }

        Vector2 StartingPosition;
        Vector2 Position;
        float StartingPressure;
        float Pressure;

        ObservableCollection<Color> ObservableCollection { get; } = new ObservableCollection<Color>();
        readonly DispatcherTimer Timer = new DispatcherTimer
        {
            Interval = System.TimeSpan.FromSeconds(1)
        };

        #region IInkParameter

        public InkType InkType { get => this.InkParameter.InkType; set => this.InkParameter.InkType = value; }
        public InkPresenter InkPresenter => this.InkParameter.InkPresenter;

        public Color Color { get; private set; }
        public Vector4 ColorHdr { get; private set; }

        public string TextureSelectedItem => this.InkParameter.TextureSelectedItem;
        public void ConstructTexture(string path) => this.InkParameter.ConstructTexture(path);
        public Task<ContentDialogResult> ShowTextureAsync() => this.InkParameter.ShowTextureAsync();

        IInkParameter InkParameter;
        public void Construct(IInkParameter item)
        {
            this.InkParameter = item;
        }

        #endregion


        Point StartingStraw;

        //@Construct
        public ColorButton()
        {
            this.InitializeComponent();
            this.ConstructColor();
            this.ConstructStraw();
            this.SetColorHdr(this.ColorPicker.Color);
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

        public void Show(Color color)
        {
            this.ColorPicker.ColorChanged -= this.ColorChanged;
            this.ColorPicker.ColorChanged -= this.ColorPicker_ColorChanged;
            {
                this.ColorPicker.Color = color;
            }
            this.ColorPicker.ColorChanged += this.ColorChanged;
            this.ColorPicker.ColorChanged += this.ColorPicker_ColorChanged;
        }
        public void ShowAt(Color color, FrameworkElement placementTarget)
        {
            this.ColorPicker.ColorChanged -= this.ColorChanged;
            this.ColorPicker.ColorChanged -= this.ColorPicker_ColorChanged;
            {
                this.ColorPicker.Color = color;
                base.Flyout.ShowAt(placementTarget);
            }
            this.ColorPicker.ColorChanged += this.ColorChanged;
            this.ColorPicker.ColorChanged += this.ColorPicker_ColorChanged;
        }

        public UIElement GetTarget()
        {
            if (Window.Current.Content is FrameworkElement frame)
            {
                if (frame.Parent is FrameworkElement border)
                {
                    if (border.Parent is FrameworkElement rootScrollViewer)
                        return rootScrollViewer;
                    else
                        return border;
                }
                else
                    return frame;
            }
            else return Window.Current.Content;
        }

    }
}