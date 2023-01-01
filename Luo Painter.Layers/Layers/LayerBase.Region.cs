using Luo_Painter.Historys;
using Luo_Painter.Historys.Models;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using System.Numerics;
using Windows.Storage.Streams;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;

namespace Luo_Painter.Layers
{
    [Flags]
    public enum RegionType : uint
    {
        None = 0, // 0000
        XRemainder = 1, // 0001
        YRemainder = 2, // 0010
        XYRemainder = 3, // 0011
    }

    public abstract partial class LayerBase
    {

        /// <summary>
        /// (100, 100) (100, 100) (050, 100) 
        /// (100, 100) (100, 100) (050, 100) 
        /// (100, 050) (100, 050) (050, 050) 
        /// </summary>
        public const int Unit = 100;

        public readonly RegionType RegionType; // XYRemainder

        public readonly int Width; // 250
        public readonly int Height; // 250
        public readonly Vector2 Center; // (125, 125)
        public readonly PixelBounds Bounds; // 0, 0, 250, 250

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


        public int GetX(int hitIndex) => hitIndex % this.XLength; // 2
        public int GetY(int hitIndex) => hitIndex / this.XLength; // 2

        public int GetLeft(int x) => x * LayerBase.Unit; // 200
        public int GetTop(int y) => y * LayerBase.Unit; // 200
        public int GetWidth(int x) => x == this.XDivisor ? this.XRemainder : LayerBase.Unit; // 500
        public int GetHeight(int y) => y == this.YDivisor ? this.YRemainder : LayerBase.Unit; // 500

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


        public void Hit(FanKit.Transformers.Transformer transformer) => this.Hit((int)transformer.MinX, (int)transformer.MinY, (int)transformer.MaxX, (int)transformer.MaxY);
        public void Hit(FanKit.Transformers.TransformerBorder border) => this.Hit((int)border.Left, (int)border.Top, (int)border.Right, (int)border.Bottom);
        public void Hit(Rect rect) => this.Hit((int)rect.Left, (int)rect.Top, (int)rect.Right, (int)rect.Bottom);
        public void Hit(int left, int top, int right, int bottom)
        {
            if (left > this.Width) return;
            if (top > this.Height) return;
            if (right < 0) return;
            if (bottom < 0) return;

            left = Math.Max(0, left / LayerBase.Unit);
            top = Math.Max(0, top / LayerBase.Unit);
            right = Math.Min(this.XLength, right / LayerBase.Unit + 1);
            bottom = Math.Min(this.YLength, bottom / LayerBase.Unit + 1);
            for (int x = left; x < right; x++)
            {
                for (int y = top; y < bottom; y++)
                {
                    this[x + this.XLength * y] = true;
                }
            }
        }


        public void Hit(Color[] InterpolationColors)
        {
            for (int i = 0; i < this.Length; i++)
            {
                byte a = InterpolationColors[i].A;
                this[i] = a is byte.MinValue is false;
            }
        }

        public void SetPixelBytes(CanvasBitmap SourceRenderTarget, IDictionary<int, IBuffer> colors)
        {
            foreach (var item in colors)
            {
                IBuffer bytes = item.Value;
                int hitIndex = item.Key;
                int x = this.GetX(hitIndex);
                int y = this.GetY(hitIndex);
                SourceRenderTarget.SetPixelBytes(bytes, this.GetLeft(x), this.GetTop(y), this.GetWidth(x), this.GetHeight(y));
            }
        }

        public void DrawHits(CanvasDrawingSession ds, Color color, CanvasTextFormat textFormat, Func<int, string> text)
        {
            for (int i = 0; i < this.Length; i++)
            {
                if (this[i] == false) continue;

                int x = this.GetX(i);
                int y = this.GetY(i);
                int left = this.GetLeft(x);
                int top = this.GetTop(y);
                int width = this.GetWidth(x);
                int height = this.GetHeight(y);

                ds.DrawRectangle(left, top, width, height, color);
                ds.DrawText(text(i), left, top, width, height, color, textFormat);
            }
        }

    }
}