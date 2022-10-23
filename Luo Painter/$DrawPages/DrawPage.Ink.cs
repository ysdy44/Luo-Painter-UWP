using Luo_Painter.Blends;
using Luo_Painter.Brushes;
using Luo_Painter.Layers;
using Luo_Painter.Options;
using Microsoft.Graphics.Canvas;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter
    {

        bool InkIsEnabled;

        private void SetInkToolType(OptionType type)
        {
            InkType toolType;
            switch (type)
            {
                case OptionType.PaintBrush: toolType = InkType.General; break;
                case OptionType.PaintWatercolorPen: toolType = InkType.Tip; break;
                case OptionType.PaintPencil: toolType = InkType.Line; break;
                case OptionType.PaintEraseBrush: toolType = InkType.Erase; break;
                case OptionType.PaintLiquefaction: toolType = InkType.Liquefy; break;
                default: toolType = default; break;
            }

            if (this.InkPresenter.Type == toolType) return;
            this.InkPresenter.Type = toolType;

            this.InkType = this.InkPresenter.GetType();
        }


        private void ConstructBrush()
        {
            this.BrushMenu.ItemClick += async (s, brush) =>
            {
                if (brush.Shape is PaintTexture shape) this.InkPresenter.ConstructShape(shape.Path, await CanvasBitmap.LoadAsync(this.CanvasDevice, shape.Source));
                else this.InkPresenter.ClearShape();

                if (brush.Grain is PaintTexture grain) this.InkPresenter.ConstructGrain(grain.Path, await CanvasBitmap.LoadAsync(this.CanvasDevice, grain.Source));
                else this.InkPresenter.ClearGrain();

                this.InkPresenter.Construct(brush);
                this.InkType = this.InkPresenter.GetType();

                this.InkIsEnabled = false;
                this.AppBar.ConstructInk(this.InkPresenter, true);
                this.InkIsEnabled = true;

                this.PaintScrollViewer.ConstructInk(this.InkPresenter);
                this.PaintScrollViewer.TryInk();
            };

            this.SizeMenu.ItemClick += (s, size) =>
            {
                this.InkPresenter.Size = (float)size;

                this.InkIsEnabled = false;
                this.AppBar.ConstructInk(this.InkPresenter, true);
                this.InkIsEnabled = true;

                this.PaintScrollViewer.ConstructInk(this.InkPresenter);
                this.PaintScrollViewer.TryInk();
            };
        }

        private void ConstructInk()
        {
            this.AppBar.ConstructInk(this.InkPresenter, false);
            this.InkIsEnabled = true;

            this.AppBar.SizeValueChanged += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;

                double size = this.SizeRange.ConvertXToY(e.NewValue);
                this.InkPresenter.Size = (float)size;
            };
            this.AppBar.OpacityValueChanged += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;

                double opacity = System.Math.Clamp(e.NewValue / 100, 0, 1);
                this.InkPresenter.Opacity = (float)opacity;
                this.InkType = this.InkPresenter.GetType();
            };
        }

    }
}