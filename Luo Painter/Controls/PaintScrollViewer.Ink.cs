using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Microsoft.Graphics.Canvas;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class PaintScrollViewer : UserControl, IInkParameter
    {

        public void TryInkAsync()
        {
            if (this.InkCanvasControl.ReadyToDraw is false) return;

            System.Threading.Tasks.Task.Run(this.InkAsync);
        }
        public void TryInk()
        {
            if (this.InkCanvasControl.ReadyToDraw is false) return;

            lock (this.InkLocker) this.Ink();
        }

        private void InkAsync()
        {
            //@Task
            if (System.Threading.Monitor.TryEnter(this.InkLocker, System.TimeSpan.FromMilliseconds(100)))
            {
                this.Ink();
                System.Threading.Monitor.Exit(this.InkLocker);
            }
            //else // Frame dropping
        }

        private void Ink()
        {
            switch (this.InkType)
            {
                case InkType.General:
                case InkType.General_Opacity:
                case InkType.General_Grain:
                case InkType.General_Opacity_Grain:
                case InkType.General_Blend:
                case InkType.General_Opacity_Blend:
                case InkType.General_Grain_Blend:
                case InkType.General_Opacity_Grain_Blend:
                case InkType.General_Mix:
                case InkType.General_Opacity_Mix:
                case InkType.General_Grain_Mix:
                case InkType.General_Opacity_Grain_Mix:
                case InkType.General_Blend_Mix:
                case InkType.General_Opacity_Blend_Mix:
                case InkType.General_Grain_Blend_Mix:
                case InkType.General_Opacity_Grain_Blend_Mix:
                case InkType.Blur:
                case InkType.Erase:
                case InkType.Erase_Opacity:
                    using (CanvasDrawingSession ds = this.InkRender.CreateDrawingSession())
                    {
                        //@DPI 
                        ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                        ds.Clear(Colors.Transparent);
                        this.InkPresenter.IsometricDrawShaderBrushEdgeHardness(ds, this.BrushEdgeHardnessShaderCodeBytes, this.ColorHdr, this.InkCanvasControl.Dpi.ConvertPixels());
                    }
                    this.InkCanvasControl.Invalidate();
                    break;

                case InkType.ShapeGeneral:
                case InkType.ShapeGeneral_Opacity:
                case InkType.ShapeGeneral_Grain:
                case InkType.ShapeGeneral_Opacity_Grain:
                case InkType.ShapeGeneral_Blend:
                case InkType.ShapeGeneral_Opacity_Blend:
                case InkType.ShapeGeneral_Grain_Blend:
                case InkType.ShapeGeneral_Opacity_Grain_Blend:
                case InkType.ShapeGeneral_Mix:
                case InkType.ShapeGeneral_Opacity_Mix:
                case InkType.ShapeGeneral_Grain_Mix:
                case InkType.ShapeGeneral_Opacity_Grain_Mix:
                case InkType.ShapeGeneral_Blend_Mix:
                case InkType.ShapeGeneral_Opacity_Blend_Mix:
                case InkType.ShapeGeneral_Grain_Blend_Mix:
                case InkType.ShapeGeneral_Opacity_Grain_Blend_Mix:
                    using (CanvasDrawingSession ds = this.InkRender.CreateDrawingSession())
                    {
                        //@DPI
                        ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                        ds.Clear(Colors.Transparent);
                        this.InkPresenter.IsometricDrawShaderBrushEdgeHardnessWithTexture(ds, this.BrushEdgeHardnessWithTextureShaderCodeBytes, this.ColorHdr, this.InkCanvasControl.Dpi.ConvertPixels());
                    }
                    this.InkCanvasControl.Invalidate();
                    break;

                case InkType.Tip:
                case InkType.Tip_Opacity:
                case InkType.Tip_Grain:
                case InkType.Tip_Opacity_Grain:
                case InkType.Tip_Blend:
                case InkType.Tip_Opacity_Blend:
                case InkType.Tip_Grain_Blend:
                case InkType.Tip_Opacity_Grain_Blend:
                    using (CanvasDrawingSession ds = this.InkRender.CreateDrawingSession())
                    {
                        ds.Clear(Colors.Transparent);
                        this.InkPresenter.IsometricTip(ds, this.Color, false);
                    }
                    this.InkCanvasControl.Invalidate();
                    break;

                case InkType.Line:
                case InkType.Line_Opacity:
                case InkType.Line_Grain:
                case InkType.Line_Opacity_Grain:
                case InkType.Line_Blend:
                case InkType.Line_Opacity_Blend:
                case InkType.Line_Grain_Blend:
                case InkType.Line_Opacity_Grain_Blend:
                case InkType.Mosaic:
                    using (CanvasDrawingSession ds = this.InkRender.CreateDrawingSession())
                    {
                        ds.Clear(Colors.Transparent);
                        this.InkPresenter.DrawLine(ds, this.Color);
                    }
                    this.InkCanvasControl.Invalidate();
                    break;
                case InkType.Liquefy:
                    using (CanvasDrawingSession ds = this.InkRender.CreateDrawingSession())
                    {
                        ds.Clear(Colors.Transparent);
                        this.InkPresenter.IsometricTip(ds, this.Color, true);
                    }
                    this.InkCanvasControl.Invalidate();
                    break;

                default:
                    break;
            }
        }

    }
}