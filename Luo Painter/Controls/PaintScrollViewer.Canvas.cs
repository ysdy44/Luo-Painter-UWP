using Luo_Painter.Brushes;
using System;

namespace Luo_Painter.Controls
{
    public sealed partial class PaintScrollViewer
    {

        private void ConstructCanvas()
        {
            this.InkCanvasControl.CreateResources += (sender, args) =>
            {
                args.TrackAsyncAction(this.CreateResourcesAsync(sender).AsAsyncAction());
            };
            this.InkCanvasControl.Draw += (sender, args) =>
            {
                switch (this.InkType)
                {
                    case InkType.Blur:
                        args.DrawingSession.DrawImage(InkPresenter.GetBlur(this.InkRender, this.InkPresenter.Flow * 4));
                        break;
                    case InkType.Mosaic:
                        args.DrawingSession.DrawImage(InkPresenter.GetMosaic(this.InkRender, this.InkPresenter.Size / 10));
                        break;
                    default:
                        args.DrawingSession.DrawImage(this.InkRender);
                        break;
                }
            };
        }

    }
}