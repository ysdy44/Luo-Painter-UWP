using Luo_Painter.Historys;
using Luo_Painter.Historys.Models;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;
using Windows.UI;

namespace Luo_Painter.Layers.Models
{
    public sealed partial class BitmapLayer : LayerBase, ILayer
    {

        public IHistory GetBitmapHistory()
        {
            BitmapHistory bitmap = new BitmapHistory
            {
                Id = base.Id,
                UndoParameter = new Dictionary<int, IBuffer>(),
                RedoParameter = new Dictionary<int, IBuffer>(),
            };

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

                bitmap.UndoParameter[i] = this.OriginRenderTarget.GetPixelBytes(left, top, width, height).AsBuffer();
                bitmap.RedoParameter[i] = this.SourceRenderTarget.GetPixelBytes(left, top, width, height).AsBuffer();
            }

            return bitmap;
        }
        public IHistory GetBitmapClearHistory() => new BitmapClearHistory
        {
            Id = base.Id,
            UndoParameter = this.OriginRenderTarget.GetPixelBytes().AsBuffer()
        };
        public IHistory GetBitmapResetHistory() => new BitmapResetHistory
        {
            Id = base.Id,
            UndoParameter = this.OriginRenderTarget.GetPixelBytes().AsBuffer(),
            RedoParameter = this.SourceRenderTarget.GetPixelBytes().AsBuffer()
        };

        public bool Undo(IHistory history)
        {
            switch (history.Type)
            {
                case HistoryType.Visibility:
                    if (history is VisibilityHistory visibility)
                    {
                        base.Visibility = visibility.UndoParameter;
                        return true;
                    }
                    else return false;
                case HistoryType.Bitmap:
                    if (history is BitmapHistory bitmap)
                    {
                        this.SetPixelBytes(bitmap.UndoParameter);
                        this.Flush();
                        this.RenderThumbnail();
                        return true;
                    }
                    else return false;
                case HistoryType.BitmapClear:
                    if (history is BitmapClearHistory bitmapClear)
                    {
                        this.OriginRenderTarget.SetPixelBytes(bitmapClear.UndoParameter);
                        this.SourceRenderTarget.SetPixelBytes(bitmapClear.UndoParameter);
                        this.RenderThumbnail();
                        return true;
                    }
                    else return false;
                case HistoryType.BitmapReset:
                    if (history is BitmapResetHistory bitmapReset)
                    {
                        this.OriginRenderTarget.SetPixelBytes(bitmapReset.UndoParameter);
                        this.SourceRenderTarget.SetPixelBytes(bitmapReset.UndoParameter);
                        this.RenderThumbnail();
                        return true;
                    }
                    else return false;
                default:
                    return false;
            }
        }
        public bool Redo(IHistory history)
        {
            switch (history.Type)
            {
                case HistoryType.Visibility:
                    if (history is VisibilityHistory visibility)
                    {
                        base.Visibility = visibility.RedoParameter;
                        return true;
                    }
                    else return false;
                case HistoryType.Bitmap:
                    if (history is BitmapHistory bitmap)
                    {
                        this.SetPixelBytes(bitmap.RedoParameter);
                        this.Flush();
                        this.RenderThumbnail();
                        return true;
                    }
                    else return false;
                case HistoryType.BitmapClear:
                    this.Clear(Colors.Transparent);
                    this.ClearThumbnail(Colors.Transparent);
                    return true;
                case HistoryType.BitmapReset:
                    if (history is BitmapResetHistory bitmapReset)
                    {
                        this.OriginRenderTarget.SetPixelBytes(bitmapReset.RedoParameter);
                        this.SourceRenderTarget.SetPixelBytes(bitmapReset.RedoParameter);
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