﻿using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Text;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class PixelBoundsPage : Page
    {

        readonly CanvasDevice Device = new CanvasDevice();
        BitmapLayer BitmapLayer; // 1024 * 1024
        PixelBounds InterpolationBounds; // 5 * 7
        PixelBounds PixelBounds; // 540 * 620

        Color[] InterpolationColors;
        PixelBoundsMode PixelBoundsMode = PixelBoundsMode.Transarent;
        readonly CanvasTextFormat TextFormat = new CanvasTextFormat
        {
            HorizontalAlignment = CanvasHorizontalAlignment.Center,
            VerticalAlignment = CanvasVerticalAlignment.Center,
            FontWeight = FontWeights.Bold
        };

        public PixelBoundsPage()
        {
            this.InitializeComponent();
            this.ConstructPixelBounds();
            this.ConstructCanvas();
        }

        private void ConstructPixelBounds()
        {
            this.AddButton.Click += async (s, e) =>
            {
                StorageFile file = await this.PickSingleImageFileAsync(PickerLocationId.Desktop);
                if (file is null) return;

                bool? result = await this.AddAsync(file);
                if (result is null) return; 
                if (result is false) return;

                this.InterpolationColors = this.BitmapLayer.GetInterpolationColorsBySource();
                this.PixelBoundsMode = this.BitmapLayer.GetInterpolationBoundsMode(this.InterpolationColors);
                this.TextBlock.Text = this.PixelBoundsMode.ToString();

                switch (this.PixelBoundsMode)
                {
                    case PixelBoundsMode.None:
                        this.InterpolationBounds = this.BitmapLayer.CreateInterpolationBounds(this.InterpolationColors);
                        this.PixelBounds = this.BitmapLayer.CreatePixelBounds(this.InterpolationBounds, this.InterpolationColors);

                        this.BitmapLayer.Hit(this.InterpolationColors);
                        break;
                    default:
                        this.InterpolationBounds = this.BitmapLayer.CreateInterpolationBounds();
                        this.PixelBounds = this.BitmapLayer.Bounds;
                        break;
                }

                this.CanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
            };
        }

        private void ConstructCanvas()
        {
            this.CanvasControl.UseSharedDevice = true;
            this.CanvasControl.CustomDevice = this.Device;
            this.CanvasControl.Draw += (sender, args) =>
            {
                if (this.BitmapLayer is null) return;

                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                args.DrawingSession.DrawImage(this.BitmapLayer[BitmapType.Source]);

                float stroke = sender.Dpi.ConvertDipsToPixels(1);
                args.DrawingSession.DrawRectangle(this.BitmapLayer.Bounds.ToRect(), Colors.YellowGreen, stroke);

                if (this.PixelBoundsMode == PixelBoundsMode.Transarent) return;
                args.DrawingSession.DrawRectangle(this.InterpolationBounds.ToRect(BitmapLayer.Unit), Colors.Red, stroke);
                args.DrawingSession.DrawRectangle(this.PixelBounds.ToRect(), Colors.DodgerBlue, stroke);
            };


            this.OriginCanvasControl.UseSharedDevice = true;
            this.OriginCanvasControl.CustomDevice = this.Device;
            this.OriginCanvasControl.Draw += (sender, args) =>
            {
                if (this.BitmapLayer is null) return;

                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                args.DrawingSession.DrawImage(this.BitmapLayer.GetInterpolationScaled());


                float stroke = sender.Dpi.ConvertDipsToPixels(1);
                args.DrawingSession.DrawRectangle(this.BitmapLayer.Bounds.ToRect(), Colors.YellowGreen, stroke);

                if (this.PixelBoundsMode == PixelBoundsMode.Transarent) return;
                args.DrawingSession.DrawRectangle(this.InterpolationBounds.ToRect(BitmapLayer.Unit), Colors.Red, stroke);
                args.DrawingSession.DrawRectangle(this.PixelBounds.ToRect(), Colors.DodgerBlue, stroke);

                this.BitmapLayer.DrawHits(args.DrawingSession, Colors.Red, this.TextFormat, (i) => $"{i}\r\n({this.InterpolationColors[i].A})");
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
                {
                    //@DPI
                    CanvasBitmap bitmap = await CanvasBitmap.LoadAsync(this.CanvasControl, stream, 96);
                    this.BitmapLayer = new BitmapLayer(this.CanvasControl, bitmap);
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