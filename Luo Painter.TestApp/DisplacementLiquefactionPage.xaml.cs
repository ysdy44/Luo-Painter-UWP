using Luo_Painter.Elements;
using Luo_Painter.Historys;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Shaders;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    internal enum DisplacementLiquefactionState
    {
        Liquefaction,
        Displacement,
        Grid,
    }

    internal sealed class DisplacementLiquefactionMesh
    {
        // Mesh
        public readonly CanvasBitmap Bitmap;
        public readonly ICanvasImage Image;
        public DisplacementLiquefactionMesh(ICanvasResourceCreator resourceCreator, float scale)
        {
            this.Bitmap = CanvasBitmap.CreateFromColors(resourceCreator, new Color[]
            {
                Colors.Black, Colors.White,
                Colors.White, Colors.Black
            }, 2, 2);

            this.Image = new BorderEffect
            {
                ExtendX = CanvasEdgeBehavior.Wrap,
                ExtendY = CanvasEdgeBehavior.Wrap,
                Source = new ScaleEffect
                {
                    Scale = new Vector2(scale),
                    BorderMode = EffectBorderMode.Hard,
                    InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                    Source = new BorderEffect
                    {
                        ExtendX = CanvasEdgeBehavior.Wrap,
                        ExtendY = CanvasEdgeBehavior.Wrap,
                        Source = this.Bitmap
                    }
                }
            };
        }
    }

    public sealed partial class DisplacementLiquefactionPage : Page
    {
        readonly CanvasDevice Device = new CanvasDevice();
        DisplacementLiquefactionMesh Mesh;
        BitmapLayer BitmapLayer;
        CanvasBitmap CanvasBitmap;

        Vector2 Position;

        float Amount = 512;
        float Pressure = 1;
        float RangeSize = 50;
        byte[] ShaderCodeBytes;

        Historian<IHistory> History { get; } = new Historian<IHistory>(20);

        public DisplacementLiquefactionPage()
        {
            this.InitializeComponent();
            this.ConstructDisplacementLiquefaction();
            this.ConstructCanvas();
            this.ConstructOperator();
        }


        private void ConstructDisplacementLiquefaction()
        {
            this.StateListView.ItemsSource = System.Enum.GetValues(typeof(DisplacementLiquefactionState));
            this.StateListView.SelectedIndex = 0;
            this.StateListView.SelectionChanged += (s, e) => this.CanvasControl.Invalidate(); // Invalidate

            this.ModeListView.ItemsSource = System.Enum.GetValues(typeof(DisplacementLiquefactionMode));
            this.ModeListView.SelectedIndex = 0;

            this.AddButton.Click += async (s, e) =>
            {
                StorageFile file = await this.PickSingleImageFileAsync(PickerLocationId.Desktop);
                if (file is null) return;

                bool? result = await this.AddAsync(file);
                if (result is null) return; 
                if (result is false) return;

                this.CanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
                this.SourceCanvasControl.Invalidate(); // Invalidate
                this.TempCanvasControl.Invalidate(); // Invalidate
            };
            this.PressureSlider.ValueChanged += (s, e) => this.Pressure = (float)e.NewValue / 100;
            this.AmountSlider.ValueChanged += (s, e) =>
            {
                this.Amount = (float)e.NewValue;
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.RangeSizeSlider.ValueChanged += (s, e) => this.RangeSize = (float)e.NewValue;
            this.ClearButton.Click += (s, e) =>
            {
                this.BitmapLayer.Clear(DisplacementExtension.ClearColor, BitmapType.Origin);
                this.BitmapLayer.Clear(DisplacementExtension.ClearColor, BitmapType.Source);
                this.BitmapLayer.Clear(DisplacementExtension.ClearColor, BitmapType.Temp);

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
                this.Mesh = new DisplacementLiquefactionMesh(sender, 75);
                this.BitmapLayer = new BitmapLayer(this.Device, 512, 512);
                this.BitmapLayer.Clear(DisplacementExtension.ClearColor, BitmapType.Origin);
                this.BitmapLayer.Clear(DisplacementExtension.ClearColor, BitmapType.Source);
                this.BitmapLayer.Clear(DisplacementExtension.ClearColor, BitmapType.Temp);
                args.TrackAsyncAction(this.CreateResourcesAsync().AsAsyncAction());
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                switch ((DisplacementLiquefactionState)this.StateListView.SelectedIndex)
                {
                    case DisplacementLiquefactionState.Displacement:
                        args.DrawingSession.DrawImage(this.BitmapLayer[BitmapType.Source]);
                        break;
                    case DisplacementLiquefactionState.Liquefaction:
                        args.DrawingSession.FillRectangle(this.BitmapLayer.Bounds.ToRect(), Colors.White);
                        if (this.CanvasBitmap is null) return;

                        args.DrawingSession.DrawImage(new CropEffect
                        {
                            SourceRectangle = this.BitmapLayer.Bounds.ToRect(),
                            Source = new DisplacementMapEffect
                            {
                                Amount = this.Amount,
                                XChannelSelect = EffectChannelSelect.Red,
                                YChannelSelect = EffectChannelSelect.Green,
                                Source = new BorderEffect
                                {
                                    ExtendX = CanvasEdgeBehavior.Clamp,
                                    ExtendY = CanvasEdgeBehavior.Clamp,
                                    Source = this.CanvasBitmap
                                },
                                Displacement = new GaussianBlurEffect
                                {
                                    BorderMode = EffectBorderMode.Hard,
                                    BlurAmount = 16,
                                    Source = this.BitmapLayer[BitmapType.Source]
                                },
                            }
                        });
                        break;
                    case DisplacementLiquefactionState.Grid:
                        args.DrawingSession.DrawImage(new CropEffect
                        {
                            SourceRectangle = this.BitmapLayer.Bounds.ToRect(),
                            Source = new AlphaMaskEffect
                            {
                                AlphaMask = new LuminanceToAlphaEffect
                                {
                                    Source = new EdgeDetectionEffect
                                    {
                                        Source = new DisplacementMapEffect
                                        {
                                            Amount = this.Amount,
                                            XChannelSelect = EffectChannelSelect.Red,
                                            YChannelSelect = EffectChannelSelect.Green,
                                            Source = this.Mesh.Image,
                                            Displacement = new GaussianBlurEffect
                                            {
                                                BorderMode = EffectBorderMode.Hard,
                                                BlurAmount = 16,
                                                Source = this.BitmapLayer[BitmapType.Source]
                                            }
                                        }
                                    }
                                },
                                Source = new ColorSourceEffect
                                {
                                    Color = Colors.DodgerBlue
                                }
                            }
                        });
                        break;
                    default:
                        break;
                }
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

        private async Task CreateResourcesAsync()
        {
            this.ShaderCodeBytes = await ShaderType.DisplacementLiquefaction.LoadAsync();
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, properties) =>
            {
                this.Position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);

                this.CanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
                this.SourceCanvasControl.Invalidate(); // Invalidate
                this.TempCanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                Vector2 position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);
                this.Render(this.Position, position);
                this.Position = position;

                this.CanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
                this.SourceCanvasControl.Invalidate(); // Invalidate
                this.TempCanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Complete += (point, properties) =>
            {
                // History
                this.History.Push(this.BitmapLayer.GetBitmapHistory());
                this.BitmapLayer.Flush();
                this.BitmapLayer.Clear(DisplacementExtension.ClearColor, BitmapType.Temp);
                this.BitmapLayer.RenderThumbnail();

                this.CanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
                this.SourceCanvasControl.Invalidate(); // Invalidate
                this.TempCanvasControl.Invalidate(); // Invalidate
            };
        }


        private void Render(Vector2 position, Vector2 targetPosition)
        {
            if (this.BitmapLayer is null) return;
            if (this.ShaderCodeBytes is null) return;

            if (position == targetPosition) return;

            // 1. Region
            Rect rect = targetPosition.GetRect(this.RangeSize);

            // 2. Shader
            PixelShaderEffect shader = new PixelShaderEffect(this.ShaderCodeBytes)
            {
                Source1BorderMode = EffectBorderMode.Hard,
                Source1 = this.BitmapLayer[BitmapType.Source],
                Properties =
                {
                    ["mode"] = this.ModeListView.SelectedIndex,
                    ["amount"] = this.Amount,
                    ["pressure"] = this.Pressure,
                    ["radius"] = this.RangeSize,
                    ["targetPosition"] = targetPosition,
                    ["position"] = position,
                }
            };

            // 3. Render
            this.BitmapLayer.Hit(rect);
            this.BitmapLayer.Shade(shader, rect);
        }

        public async Task<StorageFile> PickSingleImageFileAsync(PickerLocationId location)
        {
            // Picker
            FileOpenPicker openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = location,
                FileTypeFilter =
                {
                    ".jpg",
                    ".jpeg",
                    ".png",
                    ".bmp"
                }
            };

            // File
            StorageFile file = await openPicker.PickSingleFileAsync();
            return file;
        }

        public async Task<bool?> AddAsync(IRandomAccessStreamReference reference)
        {
            if (reference is null) return null;

            try
            {
                using (IRandomAccessStreamWithContentType stream = await reference.OpenReadAsync())
                {
                    this.CanvasBitmap = await CanvasBitmap.LoadAsync(this.CanvasControl, stream);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}