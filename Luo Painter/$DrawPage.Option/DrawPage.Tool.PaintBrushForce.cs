﻿using Luo_Painter.Blends;
using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Options;
using Microsoft.Graphics.Canvas;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        private async void PaintBrushForce_Start()
        {
            if (this.CanvasVirtualControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;
            if (this.Tasks.State != default) return;

            if (this.LayerSelectedItem is null)
            {
                this.Tip(TipType.NoLayer);
                return;
            }

            this.BitmapLayer = this.LayerSelectedItem as BitmapLayer;
            if (this.BitmapLayer is null)
            {
                this.Tip(TipType.NotBitmapLayer);
                return;
            }

            //@Task
            {
                StrokeCap cap = new StrokeCap(this.StartingPosition, this.StartingPressure, this.InkPresenter.Size);
                lock (this.Locker)
                {
                    this.BitmapLayer.Hit(cap.Bounds);
                    this.PaintCap(cap);
                }

                Rect? region = RectExtensions.TryGetRect(this.StartingPoint, this.CanvasVirtualControl.Size, this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(cap.Size * this.Transformer.Scale));
                if (region.HasValue)
                {
                    await CanvasVirtualControl.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                    {
                        this.CanvasVirtualControl.Invalidate(region.Value); // Invalidate
                    });
                }
            }

            //@Paint
            this.Tasks.StartForce(this.StartingPosition, this.StartingPressure, this.InkPresenter.Size, this.InkPresenter.Spacing);
            this.CanvasAnimatedControl.Paused = true; // Invalidate
            this.CanvasControl.Invalidate(); // Invalidate

            //@Paint
            this.Tasks.State = PaintTaskState.Painting;
            await Task.Run(this.PaintSegmentAsync);
        }

        private void PaintBrushForce_Delta()
        {
            if (this.CanvasVirtualControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;
            if (this.BitmapLayer is null) return;

            //@Paint
            this.Tasks.Position = this.Position;
            this.Tasks.Pressure = this.Pressure;

            this.CanvasControl.Invalidate(); // Invalidate
        }

        private void PaintBrushForce_Complete()
        {
            if (this.CanvasVirtualControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;
            if (this.BitmapLayer is null) return;

            //@Paint
            this.Tasks.StopForce();
            this.Tasks.State = PaintTaskState.Painted;

            this.CanvasAnimatedControl.Paused = this.OptionType.HasPreview(); // Invalidate
            this.CanvasControl.Invalidate(); // Invalidate
        }

    }
}