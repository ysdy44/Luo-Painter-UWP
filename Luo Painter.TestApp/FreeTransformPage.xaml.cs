using FanKit.Transformers;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Shaders;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class FreeTransformPage : Page
    {
        readonly CanvasDevice Device = new CanvasDevice();
        BitmapLayer BitmapLayer;
        TransformerMode Mode = TransformerMode.None;

        TransformerRect TransformerRect = new TransformerRect(512, 512, Vector2.Zero);
        Transformer Transformer = new Transformer(512, 512, Vector2.Zero);

        Vector2 Point; // DPIs
        Vector2 Position; // Pixels

        byte[] ShaderCodeBytes;

        public FreeTransformPage()
        {
            this.InitializeComponent();
            this.ConstructFreeTransform();
            this.ConstructCanvas();
            this.ConstructOperator();
        }


        private void ConstructFreeTransform()
        {
            this.AddButton.Click += async (s, e) =>
            {
                StorageFile file = await this.PickSingleImageFileAsync(PickerLocationId.Desktop);
                if (file is null) return;

                bool? result = await this.AddAsync(file);
                if (result is null) return; 
                if (result is false) return;

                this.Transformer = new Transformer(512, 512, Vector2.Zero);

                this.CanvasControl.Invalidate(); // Invalidate
                this.ToolCanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
                this.SourceCanvasControl.Invalidate(); // Invalidate
                this.TempCanvasControl.Invalidate(); // Invalidate
            };
            this.ResetButton.Click += (s, e) =>
            {
                this.Transformer = new Transformer(512, 512, Vector2.Zero);

                this.CanvasControl.Invalidate(); // Invalidate
                this.ToolCanvasControl.Invalidate(); // Invalidate
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
                args.TrackAsyncAction(this.CreateResourcesAsync().AsAsyncAction());
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                args.DrawingSession.FillRectangle(0, 0, 512, 512, Colors.White);

                Matrix3x2 matrix = FanKit.Transformers.Transformer.FindHomography(this.Transformer, this.TransformerRect, out Vector2 distance);
                args.DrawingSession.DrawImage(new PixelShaderEffect(this.ShaderCodeBytes)
                {
                    Source1 = this.BitmapLayer[BitmapType.Source],
                    Properties =
                    {
                        ["matrix3x2"] = matrix,
                        ["zdistance"] = distance,
                        ["left"] = 0f,
                        ["top"] = 0f,
                        ["right"] = 512f,
                        ["bottom"] = 512f,
                    },
                });

                args.DrawingSession.DrawNode(this.Position);
            };


            this.ToolCanvasControl.UseSharedDevice = true;
            this.ToolCanvasControl.CustomDevice = this.Device;
            this.ToolCanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Dips; /// <see cref="DPIExtensions">

                Matrix3x2 matrix = sender.Dpi.ConvertPixelsToDips();
                args.DrawingSession.DrawBound(this.Transformer, matrix);
                args.DrawingSession.DrawNode2(Vector2.Transform(this.Transformer.LeftTop, matrix));
                args.DrawingSession.DrawNode2(Vector2.Transform(this.Transformer.RightTop, matrix));
                args.DrawingSession.DrawNode2(Vector2.Transform(this.Transformer.RightBottom, matrix));
                args.DrawingSession.DrawNode2(Vector2.Transform(this.Transformer.LeftBottom, matrix));
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
            this.ShaderCodeBytes = await new ShaderUri(ShaderType.FreeTransform).LoadAsync();
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, device, properties) =>
            {
                this.Point = point;
                this.Position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);

                Matrix3x2 matrix = this.CanvasControl.Dpi.ConvertPixelsToDips();
                this.Mode = Transformer.ContainsNodeMode(this.Point, this.Transformer, matrix);
                this.TextBlock.Text = this.Mode.ToString();

                //bool fillContainsPoint = this.Transformer.FillContainsPoint(this.PositionPixels);
                //this.TextBlock.Text = fillContainsPoint.ToString();

                this.CanvasControl.Invalidate(); // Invalidate
                this.ToolCanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
                this.SourceCanvasControl.Invalidate(); // Invalidate
                this.TempCanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Delta += (point, device, properties) =>
            {
                Vector2 position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);
                this.Position = position;
                this.Point = point;

                switch (this.Mode)
                {
                    case TransformerMode.ScaleLeftTop: this.Transformer.LeftTop = this.Position; break;
                    case TransformerMode.ScaleRightTop: this.Transformer.RightTop = this.Position; break;
                    case TransformerMode.ScaleRightBottom: this.Transformer.RightBottom = this.Position; break;
                    case TransformerMode.ScaleLeftBottom: this.Transformer.LeftBottom = this.Position; break;
                    default: break;
                }

                this.CanvasControl.Invalidate(); // Invalidate
                this.ToolCanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
                this.SourceCanvasControl.Invalidate(); // Invalidate
                this.TempCanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Complete += (point, device, properties) =>
            {
                // History

                this.CanvasControl.Invalidate(); // Invalidate
                this.ToolCanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
                this.SourceCanvasControl.Invalidate(); // Invalidate
                this.TempCanvasControl.Invalidate(); // Invalidate
            };
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
                using (CanvasBitmap bitmap = await CanvasBitmap.LoadAsync(this.CanvasControl, stream))
                {
                    this.BitmapLayer = new BitmapLayer(this.CanvasControl, bitmap, 512, 512);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}