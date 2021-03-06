using Luo_Painter.Historys;
using Luo_Painter.Historys.Models;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using System.Numerics;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Luo_Painter.Layers.Models
{
    [Flags]
    public enum RegionType : uint
    {
        None = 0, // 0000
        XRemainder = 1, // 0001
        YRemainder = 2, // 0010
        XYRemainder = 3, // 0011
    }

    public sealed partial class BitmapLayer : LayerBase, ILayer
    {

        /// <summary>
        /// (100, 100) (100, 100) (050, 100) 
        /// (100, 100) (100, 100) (050, 100) 
        /// (100, 050) (100, 050) (050, 050) 
        /// </summary>
        public const int Unit = 100;

        public readonly RegionType RegionType; // XYRemainder
        public readonly Vector2 Center; // (125, 125)
        public readonly int Width; // 250
        public readonly int Height; // 250

        readonly int XDivisor; // 2
        readonly int YDivisor; // 2
        readonly int XRemainder; // 50
        readonly int YRemainder; // 50

        readonly int XLength; // 3
        readonly int YLength; // 3
        readonly bool[] Hits; // [9*9]

        public int Length => this.Hits.Length;
        public bool this[int index]
        {
            get => this.Hits[index];
            set => this.Hits[index] = value;
        }


        public int GetX(int hitIndex) => hitIndex % this.XLength;
        public int GetY(int hitIndex) => hitIndex / this.XLength;

        public int GetLeft(int x) => x * BitmapLayer.Unit;
        public int GetTop(int y) => y * BitmapLayer.Unit;
        public int GetWidth(int x) => x == this.XDivisor ? this.XRemainder : BitmapLayer.Unit;
        public int GetHeight(int y) => y == this.YDivisor ? this.YRemainder : BitmapLayer.Unit;

        public Rect GetRect(int x, int y) => new Rect(this.GetLeft(x), this.GetTop(y), this.GetWidth(x), this.GetHeight(y));
        public Rect GetRect(int hitIndex) => this.GetRect(this.GetX(hitIndex), this.GetY(hitIndex));
        public IEnumerable<int> GetHitIndexs()
        {
            for (int i = 0; i < this.Length; i++)
            {
                if (this[i] == false) continue;
                yield return i;
            }
        }


        public void Hit(Rect rect)
        {
            if (rect.Left > this.Width) return;
            if (rect.Top > this.Height) return;
            if (rect.Right < 0) return;
            if (rect.Bottom < 0) return;

            int left = Math.Max(0, (int)rect.Left / BitmapLayer.Unit);
            int top = Math.Max(0, (int)rect.Top / BitmapLayer.Unit);
            int right = Math.Min(this.XLength, (int)Math.Ceiling(rect.Right / BitmapLayer.Unit));
            int bottom = Math.Min(this.YLength, (int)Math.Ceiling(rect.Bottom / BitmapLayer.Unit));
            for (int x = left; x < right; x++)
            {
                for (int y = top; y < bottom; y++)
                {
                    this[x + this.XLength * y] = true;
                }
            }
        }


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
                default:
                    return false;
            }
        }

        private void SetPixelBytes(IDictionary<int, IBuffer> colors)
        {
            foreach (var item in colors)
            {
                IBuffer bytes = item.Value;
                int hitIndex = item.Key;
                int x = this.GetX(hitIndex);
                int y = this.GetY(hitIndex);
                this.SourceRenderTarget.SetPixelBytes(bytes, this.GetLeft(x), this.GetTop(y), this.GetWidth(x), this.GetHeight(y));
            }
        }

    }
}