using Luo_Painter.Historys;
using Luo_Painter.Historys.Models;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;
using Windows.UI;

namespace Luo_Painter.Layers.Models
{
    public sealed partial class BitmapLayer
    {

        public IHistory GetBitmapHistory()
        {
            IDictionary<int, IBuffer> undoParameter = new Dictionary<int, IBuffer>();
            IDictionary<int, IBuffer> redoParameter = new Dictionary<int, IBuffer>();

            for (int i = 0; i < this.Length; i++)
            {
                if (this[i] == false) continue;
                this[i] = false;

                int x = this.GetX(i);
                int y = this.GetY(i);
                int left = this.GetLeft(x);
                int top = this.GetTop(y);
                int width = this.GetWidth(x);
                int height = this.GetHeight(y);

                undoParameter[i] = this.OriginRenderTarget.GetPixelBytes(left, top, width, height).AsBuffer();
                redoParameter[i] = this.SourceRenderTarget.GetPixelBytes(left, top, width, height).AsBuffer();
            }

            return new PropertyHistory<IDictionary<int, IBuffer>>(HistoryType.Bitmap, base.Id, undoParameter, redoParameter);
        }
        public IHistory GetBitmapClearHistory(Color color) => new PropertyHistory<IBuffer, Color>
        (
            HistoryType.BitmapClear,
            base.Id,
            this.OriginRenderTarget.GetPixelBytes().AsBuffer(),
            color
        );
        public IHistory GetBitmapResetHistory() => new PropertyHistory<IBuffer>
        (
            HistoryType.BitmapReset,
            base.Id,
            this.OriginRenderTarget.GetPixelBytes().AsBuffer(),
            this.SourceRenderTarget.GetPixelBytes().AsBuffer()
        );

        public override bool History(HistoryType type, object parameter)
        {
            if (base.History(type, parameter)) return true;

            switch (type)
            {
                case HistoryType.Bitmap:
                    if (parameter is IDictionary<int, IBuffer> bitmap)
                    {
                        this.SetPixelBytes(bitmap);
                        this.Flush();
                        this.RenderThumbnail();
                        return true;
                    }
                    else return false;
                case HistoryType.BitmapClear:
                    if (parameter is IBuffer bitmapClear)
                    {
                        this.OriginRenderTarget.SetPixelBytes(bitmapClear);
                        this.SourceRenderTarget.SetPixelBytes(bitmapClear);
                        this.RenderThumbnail();
                        return true;
                    }
                    else if (parameter is Color color)
                    {
                        this.Clear(color, BitmapType.Origin);
                        this.Clear(color, BitmapType.Source);
                        this.ClearThumbnail(color);
                        return true;
                    }
                    else return false;
                case HistoryType.BitmapReset:
                    if (parameter is IBuffer bitmapReset)
                    {
                        this.OriginRenderTarget.SetPixelBytes(bitmapReset);
                        this.SourceRenderTarget.SetPixelBytes(bitmapReset);
                        this.RenderThumbnail();
                        return true;
                    }
                    else return false;
                default:
                    return false;
            }
        }

    }
}