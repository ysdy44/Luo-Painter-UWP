using Luo_Painter.Blends;
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
    public sealed partial class BitmapBrushPage : Page
    {
        readonly InkPresenter InkPresenter = new InkPresenter();
        readonly CanvasDevice Device = new CanvasDevice();
        BitmapLayer BitmapLayer;

        Vector2 Position;
        float Pressure;

        InkType InkType = InkType.CircleDry;

        public BitmapBrushPage()
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
                    case 0: this.InkType = InkType.CircleDry; break;
                    case 1: this.InkType = InkType.CircleWetOpacity; break;
                    case 2: this.InkType = InkType.CircleWetBlend; break;
                    case 3: this.InkType = InkType.CircleWetOpacityBlend; break;
                    default: break;
                }
            };
            this.OpacitySlider.ValueChanged += (s, e) =>
            {
                this.InkPresenter.Opacity = (float)e.NewValue;
            };
            this.BlendModeListView.ItemsSource = System.Enum.GetValues(typeof(BlendEffectMode));
            this.BlendModeListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is BlendEffectMode item)
                {
                    if (item.IsDefined())
                    {
                        this.InkPresenter.Mode = InkType.Blend;
                        this.InkPresenter.BlendMode = item;
                    }
                    else
                    {
                        this.InkPresenter.Mode = InkType.None;
                    }
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

                args.DrawingSession.Clear(Colors.White);
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

                //this.InkType = this.InkPresenter.GetType(InkType.BrushDry);
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
                    case InkType.CircleDry:
                        {
                            bool result = this.BitmapLayer.IsometricFillCircle(
                                this.ColorPicker.Color,
                                this.Position, position,
                                this.Pressure, pressure,
                                this.InkPresenter.Size,
                                this.InkPresenter.Spacing,
                                BitmapType.Source);
                            if (result is false) return;
                        }
                        break;
                    case InkType.CircleWetOpacity:
                    case InkType.CircleWetBlend:
                    case InkType.CircleWetOpacityBlend:
                        {
                            bool result = this.BitmapLayer.IsometricFillCircle(
                                this.ColorPicker.Color,
                                this.Position, position,
                                this.Pressure, pressure,
                                this.InkPresenter.Size,
                                this.InkPresenter.Spacing,
                                BitmapType.Temp);
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
                    case InkType.CircleDry: /// <see cref="InkType.Dry"/>
                        this.BitmapLayer.Flush();
                        this.OriginCanvasControl.Invalidate(); // Invalidate
                        this.SourceCanvasControl.Invalidate(); // Invalidate
                        this.TempCanvasControl.Invalidate(); // Invalidate
                        break;
                    case InkType.CircleWetOpacity: /// <see cref="InkType.Wet"/>
                        // 1.  Temp => Source
                        this.BitmapLayer.Draw(this.InkPresenter.GetWet(this.InkType, this.BitmapLayer[BitmapType.Temp]));

                        // 2. Temp => 0   
                        this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Temp);

                        // 3. Source => Origin
                        this.BitmapLayer.Flush();
                        this.OriginCanvasControl.Invalidate(); // Invalidate
                        this.SourceCanvasControl.Invalidate(); // Invalidate
                        this.TempCanvasControl.Invalidate(); // Invalidate
                        break;
                    case InkType.CircleWetBlend: /// <see cref="InkType.WetComposite"/>
                    case InkType.CircleWetOpacityBlend: /// <see cref="InkType.WetComposite"/>
                        // 1.  Origin + Temp => Source
                        this.BitmapLayer.DrawCopy(this.InkPresenter.GetPreview(this.InkType, this.BitmapLayer[BitmapType.Origin], this.InkPresenter.GetWet(this.InkType, this.BitmapLayer[BitmapType.Temp])));

                        // 2. Temp => 0
                        this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Temp);

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