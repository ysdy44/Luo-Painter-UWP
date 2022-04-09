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
    public sealed partial class BitmapEraseBrushPage : Page
    {
        readonly CanvasDevice Device = new CanvasDevice();
        BitmapLayer BitmapLayer;

        Vector2 Position;
        float Pressure;

        InkMode InkMode = InkMode.EraseDry;
        float InkOpacity = 0.4f;

        public BitmapEraseBrushPage()
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
                    case 0: this.InkMode = InkMode.EraseDry; break;
                    case 1: this.InkMode = InkMode.EraseWetWithOpacity; break;
                    default: break;
                }
            };
            this.OpacitySlider.ValueChanged += (s, e) =>
            {
                this.InkOpacity = (float)e.NewValue;
            };
            this.ClearButton.Click += (s, e) =>
            {
                this.BitmapLayer.Clear(Colors.DodgerBlue);

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
                this.BitmapLayer.Clear(Colors.DodgerBlue);
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

                args.DrawingSession.Clear(Colors.Orange);
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
                    case InkMode.EraseDry:
                        this.BitmapLayer.ErasingDry(this.Position, position, this.Pressure, properties.Pressure, 12);
                        break;
                    case InkMode.EraseWetWithOpacity:
                        this.BitmapLayer.ErasingWet(this.Position, position, this.Pressure, properties.Pressure, 12);
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
                    case InkMode.EraseDry:
                        // History
                        this.BitmapLayer.Flush();
                        this.OriginCanvasControl.Invalidate(); // Invalidate
                        this.SourceCanvasControl.Invalidate(); // Invalidate
                        this.TempCanvasControl.Invalidate(); // Invalidate
                        break;
                    case InkMode.EraseWetWithOpacity:
                        this.IsEnabled = false;

                        // 1.  Origin + Temp => Source
                        await Task.Delay(400);
                        this.BitmapLayer.DrawSource(this.GetInk());
                        this.OriginCanvasControl.Invalidate(); // Invalidate
                        this.SourceCanvasControl.Invalidate(); // Invalidate
                        this.TempCanvasControl.Invalidate(); // Invalidate

                        // 2. Temp => 0
                        await Task.Delay(400);
                        this.BitmapLayer.ClearTemp();
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
            if (this.InkOpacity == 1f)
                return InkMode.EraseDry;
            else
                return InkMode.EraseWetWithOpacity;
        }
        private ICanvasImage GetInk()
        {
            switch (this.BitmapLayer.InkMode)
            {
                case InkMode.None:
                case InkMode.EraseDry:
                    return this.BitmapLayer.Source;
                case InkMode.EraseWetWithOpacity:
                    return this.BitmapLayer.GetEraseWeting(this.InkOpacity);
                default:
                    return this.BitmapLayer.Source;
            }
        }

    }
}