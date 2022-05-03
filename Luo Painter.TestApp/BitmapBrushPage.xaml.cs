using Luo_Painter.Elements;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class BitmapBrushPage : Page
    {
        readonly CanvasDevice Device = new CanvasDevice();
        BitmapLayer BitmapLayer;

        Vector2 Position;
        float Pressure;

        InkMode InkMode = InkMode.Dry;
        float InkOpacity = 0.4f;
        BlendEffectMode? InkBlendMode = null;

        public BitmapBrushPage()
        {
            this.InitializeComponent();
            this.ConstructInk();
            this.ConstructCanvas();
            this.ConstructOperator();
        }


        private void ConstructInk()
        {
            this.InkModeComboBox.SelectionChanged += (s, e) =>
            {
                switch (this.InkModeComboBox.SelectedIndex)
                {
                    case 0: this.InkMode = InkMode.Dry; break;
                    case 1: this.InkMode = InkMode.WetWithOpacity; break;
                    case 2: this.InkMode = InkMode.WetWithBlendMode; break;
                    case 3: this.InkMode = InkMode.WetWithOpacityAndBlendMode; break;
                    default: break;
                }
            };
            this.OpacitySlider.ValueChanged += (s, e) =>
            {
                this.InkOpacity = (float)e.NewValue;
            };
            this.BlendModeListView.ItemsSource = System.Enum.GetValues(typeof(BlendEffectMode));
            this.BlendModeListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is BlendEffectMode item)
                {
                    this.InkBlendMode = item;
                }
            };
            this.ClearButton.Click += (s, e) =>
            {
                this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Origin);
                this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Source);

                this.CanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
                this.SourceCanvasControl.Invalidate(); // Invalidate
                this.TempCanvasControl.Invalidate(); // Invalidate
            };
        }

        private void ConstructCanvas()
        {
            this.CanvasControl.UseSharedDevice = true;
            this.CanvasControl.CustomDevice = this.Device;
            this.CanvasControl.CreateResources += (sender, args) =>
            {
                this.BitmapLayer = new BitmapLayer(this.Device, 512, 512);
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                args.DrawingSession.FillRectangle(0, 0, this.BitmapLayer.Width, this.BitmapLayer.Height, Colors.White);

                args.DrawingSession.DrawImage(this.GetInk());
            };


            this.OriginCanvasControl.UseSharedDevice = true;
            this.OriginCanvasControl.CustomDevice = this.Device;
            this.OriginCanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                args.DrawingSession.Clear(Colors.White);
                args.DrawingSession.DrawImage(new ScaleEffect
                {
                    Source = this.BitmapLayer.Origin,
                    Scale = new Vector2(this.CanvasControl.Dpi.ConvertDipsToPixels(100) / 512)
                });
            };

            this.SourceCanvasControl.UseSharedDevice = true;
            this.SourceCanvasControl.CustomDevice = this.Device;
            this.SourceCanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                args.DrawingSession.Clear(Colors.White);
                args.DrawingSession.DrawImage(new ScaleEffect
                {
                    Source = this.BitmapLayer.Source,
                    Scale = new Vector2(this.CanvasControl.Dpi.ConvertDipsToPixels(100) / 512)
                });
            };

            this.TempCanvasControl.UseSharedDevice = true;
            this.TempCanvasControl.CustomDevice = this.Device;
            this.TempCanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                args.DrawingSession.Clear(Colors.White);
                args.DrawingSession.DrawImage(new ScaleEffect
                {
                    Source = this.BitmapLayer.Temp,
                    Scale = new Vector2(this.CanvasControl.Dpi.ConvertDipsToPixels(100) / 512)
                });
            };
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, properties) =>
            {
                this.Position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);
                this.Pressure = properties.Pressure;

                this.BitmapLayer.InkMode = this.GetInkMode();
                this.BitmapLayer.InkMode = this.InkMode;
                this.CanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
                this.SourceCanvasControl.Invalidate(); // Invalidate
                this.TempCanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                Vector2 position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);
                float pressure = properties.Pressure;

                switch (this.BitmapLayer.InkMode)
                {
                    case InkMode.None:
                        break;
                    case InkMode.Dry:
                        this.BitmapLayer.FillCircleDry(this.Position, position, this.Pressure, properties.Pressure, 12, this.ColorPicker.Color);
                        break;
                    case InkMode.WetWithOpacity:
                    case InkMode.WetWithBlendMode:
                    case InkMode.WetWithOpacityAndBlendMode:
                        this.BitmapLayer.FillCircleWet(this.Position, position, this.Pressure, properties.Pressure, 12, this.ColorPicker.Color);
                        break;
                    default:
                        break;
                }
                this.Position = position;
                this.Pressure = pressure;

                this.CanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
                this.SourceCanvasControl.Invalidate(); // Invalidate
                this.TempCanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Complete += async (point, properties) =>
            {
                switch (this.BitmapLayer.InkMode)
                {
                    case InkMode.None:
                        break;
                    case InkMode.Dry:
                        // History
                        this.BitmapLayer.Flush();
                        this.OriginCanvasControl.Invalidate(); // Invalidate
                        this.SourceCanvasControl.Invalidate(); // Invalidate
                        this.TempCanvasControl.Invalidate(); // Invalidate
                        break;
                    case InkMode.WetWithOpacity:
                    case InkMode.WetWithBlendMode:
                    case InkMode.WetWithOpacityAndBlendMode:
                        this.IsEnabled = false;

                        // 1.  Origin + Temp => Source
                        await Task.Delay(400);
                        this.BitmapLayer.DrawCopy(this.GetInk());
                        this.OriginCanvasControl.Invalidate(); // Invalidate
                        this.SourceCanvasControl.Invalidate(); // Invalidate
                        this.TempCanvasControl.Invalidate(); // Invalidate

                        // 2. Temp => 0
                        await Task.Delay(400);
                        this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Temp);
                        this.OriginCanvasControl.Invalidate(); // Invalidate
                        this.SourceCanvasControl.Invalidate(); // Invalidate
                        this.TempCanvasControl.Invalidate(); // Invalidate

                        // 3. Source => Origin
                        await Task.Delay(400);
                        // History
                        this.BitmapLayer.Flush();
                        this.OriginCanvasControl.Invalidate(); // Invalidate
                        this.SourceCanvasControl.Invalidate(); // Invalidate
                        this.TempCanvasControl.Invalidate(); // Invalidate

                        this.IsEnabled = true;
                        break;
                    default:
                        break;
                }

                this.BitmapLayer.InkMode = InkMode.None;
                this.BitmapLayer.RenderThumbnail();
                this.CanvasControl.Invalidate(); // Invalidate
            };
        }

        private InkMode GetInkMode()
        {
            if (this.InkBlendMode == null)
            {
                if (this.InkOpacity == 1f)
                    return InkMode.Dry;
                else
                    return InkMode.WetWithOpacity;
            }
            else
            {
                if (this.InkOpacity == 1f)
                    return InkMode.WetWithBlendMode;
                else
                    return InkMode.WetWithOpacityAndBlendMode;
            }
        }
        private ICanvasImage GetInk()
        {
            switch (this.BitmapLayer.InkMode)
            {
                case InkMode.None:
                case InkMode.Dry:
                    return this.BitmapLayer.Source;
                case InkMode.WetWithOpacity:
                    return this.BitmapLayer.GetWeting(this.InkOpacity);
                case InkMode.WetWithBlendMode:
                    return this.BitmapLayer.GetWeting(this.InkBlendMode.Value);
                case InkMode.WetWithOpacityAndBlendMode:
                    return this.BitmapLayer.GetWeting(this.InkOpacity, this.InkBlendMode.Value);
                default:
                    return this.BitmapLayer.Source;
            }
        }

    }
}