using FanKit.Transformers;
using Luo_Painter.Elements;
using Luo_Painter.HSVColorPickers;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
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
    public sealed partial class TransformPage : Page
    {
        readonly CanvasDevice Device = new CanvasDevice();
        BitmapLayer BitmapLayer;

        TransformMatrix Transform = new TransformMatrix
        {
            Matrix = Matrix3x2.Identity,
            Width = 512,
            Height = 512,
            Transformer = new Transformer(512, 512, Vector2.Zero)
        };

        Vector2 Point; // DPIs
        Vector2 Position; // Pixels

        Vector2 StartingPosition;
        Transformer StartingTransformer;

        public TransformPage()
        {
            this.InitializeComponent();
            this.ConstructBitmapTransformer();
            this.ConstructCanvas();
            this.ConstructOperator();
        }


        private void ConstructBitmapTransformer()
        {
            this.AddButton.Click += async (s, e) =>
            {
                StorageFile file = await this.PickSingleImageFileAsync(PickerLocationId.Desktop);
                if (file is null) return;

                bool? result = await this.AddAsync(file);
                if (result is null) return;
                if (result is false) return;

                this.Transform = new TransformMatrix
                {
                    Matrix = Matrix3x2.Identity,
                    Width = 512,
                    Height = 512,
                    Transformer = new Transformer(512, 512, Vector2.Zero)
                };

                this.CanvasControl.Invalidate(); // Invalidate
                this.ToolCanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
                this.SourceCanvasControl.Invalidate(); // Invalidate
                this.TempCanvasControl.Invalidate(); // Invalidate
            };
            this.ResetButton.Click += (s, e) =>
            {
                this.Transform = new TransformMatrix
                {
                    Matrix = Matrix3x2.Identity,
                    Width = 512,
                    Height = 512,
                    Transformer = new Transformer(512, 512, Vector2.Zero)
                };

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

                args.DrawingSession.DrawImage(new Transform2DEffect
                {
                    BorderMode = EffectBorderMode.Hard,
                    InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                    TransformMatrix = this.Transform.Matrix,
                    Source = this.BitmapLayer[BitmapType.Source]
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
                args.DrawingSession.DrawBoundNodes(this.Transform.Transformer, matrix);

                // args.DrawingSession.DrawNode(this.PositionDIPs);
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
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, device, properties) =>
            {
                this.Point = point;
                this.Position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);

                Matrix3x2 matrix = this.CanvasControl.Dpi.ConvertPixelsToDips();
                this.Transform.Mode = Transformer.ContainsNodeMode(this.Point, this.Transform.Transformer, matrix);
                this.TextBlock.Text = this.Transform.Mode.ToString();

                //bool fillContainsPoint = this.Transformer.FillContainsPoint(this.PositionPixels);
                //this.TextBlock.Text = fillContainsPoint.ToString();

                this.StartingPosition = this.Position;
                this.StartingTransformer = this.Transform.Transformer;

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

                this.Transform.Transformer = Transformer.Controller(this.Transform.Mode, this.StartingPosition, this.Position, this.StartingTransformer);
                this.Transform.UpdateMatrix();

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