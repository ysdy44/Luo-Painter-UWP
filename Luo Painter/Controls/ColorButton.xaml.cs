using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.HSVColorPickers;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.Controls
{
    internal class ColorCommand : RelayCommand<Color> { }

    public sealed partial class ColorButton : EyedropperButton, IInkParameter, IColorHdrBase, IColorBase
    {
        //@Converter
        private HarmonyMode ModeConverter(int value)
        {
            switch (value)
            {
                case 0: return HarmonyMode.None;
                case 1: return HarmonyMode.Complementary;
                case 2: return HarmonyMode.SplitComplementary;
                case 3: return HarmonyMode.Analogous;
                case 4: return HarmonyMode.Triadic;
                case 5: return HarmonyMode.Tetradic;
                default: return default;
            }
        }
        private int IndexConverter(int value)
        {
            switch (value)
            {
                case 0: case 1: return 0;
                case 2: return 1;
                case 3: case 4: return 2;
                case 5: return 3;
                case 6: return 4;
                default: return default;
            }
        }

        //@Delegate
        public event EventHandler<Color> ColorChanged;

        //@Content
        public FrameworkElement PlacementTarget => this;
        public ICommand OpenCommand => this;

        public CanvasDevice CanvasDevice => this.InkParameter.CanvasDevice;
        BitmapLayer BitmapLayer { get; set; }
        ObservableCollection<Color> ObservableCollection { get; } = new ObservableCollection<Color>();

        //@Task
        readonly object Locker = new object();
        //@ Paint
        readonly PaintTaskCollection Tasks = new PaintTaskCollection();

        OpacityImageSource OpacityImageSource;
        AlphaImageSource AlphaImageSource;
        WheelImageSource WheelImageSource;

        Vector2 StartingPosition;
        Vector2 Position;
        float StartingPressure;
        float Pressure;

        #region IInkParameter

        public InkType InkType { get => this.InkParameter.InkType; set => this.InkParameter.InkType = value; }
        public InkPresenter InkPresenter => this.InkParameter.InkPresenter;

        public Color Color { get; private set; } = Colors.Black;
        public Vector4 ColorHdr { get; private set; } = Vector4.UnitW;

        public string TextureSelectedItem => this.InkParameter.TextureSelectedItem;
        public void ConstructTexture(string path) => this.InkParameter.ConstructTexture(path);
        public Task<ContentDialogResult> ShowTextureAsync() => this.InkParameter.ShowTextureAsync();

        IInkParameter InkParameter;
        public void Construct(IInkParameter item)
        {
            this.InkParameter = item;

            float dpi = DisplayInformation.GetForCurrentView().LogicalDpi;
            this.OpacityImageSource = new OpacityImageSource(this.CanvasDevice, 90, 15, dpi);
            this.AlphaImageSource = new AlphaImageSource(this.CanvasDevice, 300, 4, dpi);
            this.WheelImageSource = new WheelImageSource(this.CanvasDevice, new CircleTemplateSettingsF(300), dpi);
        }
        private void SurfaceContentsLost(object sender, object e)
        {
            this.OpacityImageSource.Redraw();
            this.AlphaImageSource.Redraw();
            this.WheelImageSource.Redraw();
        }

        #endregion

        //@Override
        public override void OnColorChanged(Color color) => this.Color3(color);

        //@Construct
        public ColorButton()
        {
            this.InitializeComponent();
            base.Unloaded += (s, e) => CompositionTarget.SurfaceContentsLost -= this.SurfaceContentsLost;
            base.Loaded += (s, e) => CompositionTarget.SurfaceContentsLost += this.SurfaceContentsLost;
            this.ComboBox.SelectionChanged += (s, e) => this.Show(this.Color);

            this.ConstructCanvas();
            this.ConstructOperator();

            this.ConstructPicker();

            this.ConstructColor();
            this.ConstructColorHarmony();
            this.ConstructColorValue();

            this.ConstructInk();
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

        public void SetColor(Color color) => this.Color = color;
        public void SetColorHdr(Vector4 colorHdr) => this.ColorHdr = colorHdr;

        public void SetColor(Vector4 colorHdr) => this.Color = Color.FromArgb((byte)(colorHdr.W * 255f), (byte)(colorHdr.X * 255f), (byte)(colorHdr.Y * 255f), (byte)(colorHdr.Z * 255f));
        public void SetColorHdr(Color color) => this.ColorHdr = new Vector4(color.R, color.G, color.B, color.A) / 255f; // 0~1

        public void Show(Color color)
        {
            if (this.TricolorPicker.Visibility == default) this.TricolorPicker.Recolor(color);
            if (this.HuePicker.Visibility == default) this.HuePicker.Recolor(color);

            if (this.RGBPicker.Visibility == default) this.RGBPicker.Recolor(color);
            if (this.HSVPicker.Visibility == default) this.HSVPicker.Recolor(color);

            this.HexPicker.Recolor(color);
        }
        public void ShowAt(Color color, FrameworkElement placementTarget)
        {
            this.Show(color);
            base.Flyout.ShowAt(placementTarget);
        }

    }
}