using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
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
        readonly InkPresenter InkPresenter = new InkPresenter();
        readonly CanvasDevice Device = new CanvasDevice();
        BitmapLayer BitmapLayer;

        Vector2 Position;
        float Pressure;

        InkType InkType = InkType.EraseDry;

        public BitmapEraseBrushPage()
        {
            this.InitializeComponent();
            this.ConstructInk();
            this.ConstructCanvas();
            this.ConstructOperator();
        }


        private void ConstructInk()
        {
            this.InkTypeComboBox.SelectionChanged += (s, e) =>
            {
                switch (this.InkTypeComboBox.SelectedIndex)
                {
                    case 0: this.InkType = InkType.EraseDry; break;
                    case 1: this.InkType = InkType.EraseWetOpacity; break;
                    default: break;
                }
            };
            this.OpacitySlider.ValueChanged += (s, e) =>
            {
                this.InkPresenter.Opacity = (float)e.NewValue;
            };
            this.ClearButton.Click += (s, e) =>
            {
                this.BitmapLayer.Clear(Colors.DodgerBlue, BitmapType.Origin);
                this.BitmapLayer.Clear(Colors.DodgerBlue, BitmapType.Source);

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
                this.BitmapLayer.Clear(Colors.DodgerBlue, BitmapType.Origin);
                this.BitmapLayer.Clear(Colors.DodgerBlue, BitmapType.Source);
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                args.DrawingSession.FillRectangle(0, 0, this.BitmapLayer.Width, this.BitmapLayer.Height, Colors.White);

                args.DrawingSession.DrawImage(this.InkPresenter.GetPreview(this.InkType, this.BitmapLayer[BitmapType.Source], this.InkPresenter.GetWet(this.InkType, this.BitmapLayer[BitmapType.Temp])));
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
                    Source = this.BitmapLayer[BitmapType.Origin],
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
                    Source = this.BitmapLayer[BitmapType.Source],
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
                    Source = this.BitmapLayer[BitmapType.Temp],
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

                //this.InkType = this.InkPresenter.GetType(InkType.EraseDry);
                this.CanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
                this.SourceCanvasControl.Invalidate(); // Invalidate
                this.TempCanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                Vector2 position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);
                float pressure = properties.Pressure;

                switch (this.InkType)
                {
                    case InkType.None:
                        break;
                    case InkType.EraseDry:
                        {
                            bool result = this.BitmapLayer.IsometricErasingDry(this.Position, position, this.Pressure, pressure, this.InkPresenter.Size, 0.25f);
                            if (result is false) return;
                        }
                        break;
                    case InkType.EraseWetOpacity:
                        {
                            bool result = this.BitmapLayer.IsometricErasingWet(this.Position, position, this.Pressure, pressure, this.InkPresenter.Size, 0.25f);
                            if (result is false) return;
                        }
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
            this.Operator.Single_Complete += (point, properties) =>
            {
                switch (this.InkType)
                {
                    case InkType.None:
                        break;
                    case InkType.EraseDry: /// <see cref="InkType.Dry"/>
                        // History
                        this.BitmapLayer.Flush();
                        this.OriginCanvasControl.Invalidate(); // Invalidate
                        this.SourceCanvasControl.Invalidate(); // Invalidate
                        this.TempCanvasControl.Invalidate(); // Invalidate
                        break;
                    case InkType.EraseWetOpacity: /// <see cref="InkType.WetComposite"/>
                        // 1.  Origin + Temp => Source
                        this.BitmapLayer.DrawCopy(this.InkPresenter.GetPreview(this.InkType, this.BitmapLayer[BitmapType.Origin], this.InkPresenter.GetWet(this.InkType, this.BitmapLayer[BitmapType.Temp])));
                        this.OriginCanvasControl.Invalidate(); // Invalidate
                        this.SourceCanvasControl.Invalidate(); // Invalidate
                        this.TempCanvasControl.Invalidate(); // Invalidate

                        // 2. Temp => 0
                        this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Temp);
                        this.OriginCanvasControl.Invalidate(); // Invalidate
                        this.SourceCanvasControl.Invalidate(); // Invalidate
                        this.TempCanvasControl.Invalidate(); // Invalidate

                        // 3. Source => Origin
                        this.BitmapLayer.Flush();
                        this.OriginCanvasControl.Invalidate(); // Invalidate
                        this.SourceCanvasControl.Invalidate(); // Invalidate
                        this.TempCanvasControl.Invalidate(); // Invalidate
                        break;
                    default:
                        break;
                }

                //this.InkType = default;
                this.BitmapLayer.RenderThumbnail();
                this.CanvasControl.Invalidate(); // Invalidate
            };
        }

    }
}