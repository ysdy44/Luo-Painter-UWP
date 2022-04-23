using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;
using Windows.UI;

namespace Luo_Painter.Layers.Models
{
    public partial class BitmapLayer : LayerBase, ILayer
    {
        byte[] Pixels;
        IBuffer Buffer;
        readonly HashSet<int> Retrieves = new HashSet<int>();
        readonly HashSet<int> TargetRetrieves = new HashSet<int>();
        readonly HashSet<int> Add1Retrieves = new HashSet<int>();
        readonly HashSet<int> Add2Retrieves = new HashSet<int>();

        public bool FloodSelect(Vector2 point, Color color, float tolerance = 0.1f, bool feather = false)
        {
            // 1. Get Position and Target
            int px = (int)point.X;
            int py = (int)point.Y;

            bool hasTarget = (px >= 0 && px < this.Width && py >= 0 && py < this.Height);
            if (hasTarget is false) return false;

            Color target = this.SourceRenderTarget.GetPixelColors(px, py, 1, 1).Single();
            if (target.A is byte.MinValue) return false;


            // 2. Draw ChromaKeyEffect
            using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
            {
                ds.Clear(Windows.UI.Colors.Transparent);
                ds.DrawImage(new ChromaKeyEffect
                {
                    Tolerance = tolerance,
                    Feather = feather,
                    InvertAlpha = true,
                    Color = target,
                    Source = this.SourceRenderTarget,
                });
            }


            // 3. Get Retrieves
            if (this.Pixels is null || this.Buffer is null)
            {
                this.Pixels = this.TempRenderTarget.GetPixelBytes();
                this.Buffer = this.Pixels.AsBuffer();
            }
            else
            {
                this.TempRenderTarget.GetPixelBytes(this.Buffer);
                this.Buffer.CopyTo(this.Pixels);
            }

            this.Retrieves.Clear();
            int length = this.Pixels.Length / 4;
            for (int i = 0; i < length; i++)
            {
                if (this.Pixels[i * 4 + 3] != byte.MinValue) // Alpha
                {
                    this.Retrieves.Add(i);
                }
            }
            if (this.Retrieves.Count is 0) return false;


            // 4. Init Retrieves
            int index = px + py * this.Width;
            this.TargetRetrieves.Clear();
            this.TargetRetrieves.Add(index);
            this.Add1Retrieves.Clear();
            this.Add1Retrieves.Add(index);
            this.Add2Retrieves.Clear();


            while (this.Add1Retrieves.Count > 0)
            {
                // 5. Get Add1Retrieves
                foreach (int i in this.Add1Retrieves)
                {
                    int x = i % this.Width;
                    int y = i / this.Width;

                    // Left
                    if (x - 1 >= 0)
                    {
                        int left = i - 1;
                        if (this.Retrieves.Contains(left))
                            if (this.TargetRetrieves.Contains(left) is false)
                                this.Add2Retrieves.Add(left);
                    }
                    // Top
                    if (y - 1 >= 0)
                    {
                        int top = i - this.Width;
                        if (this.Retrieves.Contains(top))
                            if (this.TargetRetrieves.Contains(top) is false)
                                this.Add2Retrieves.Add(top);
                    }
                    // Right
                    if (x + 1 < this.Width)
                    {
                        int right = i + 1;
                        if (this.Retrieves.Contains(right))
                            if (this.TargetRetrieves.Contains(right) is false)
                                this.Add2Retrieves.Add(right);
                    }
                    // Bottom
                    if (y + 1 < this.Height)
                    {
                        int bottom = i + this.Width;
                        if (this.Retrieves.Contains(bottom))
                            if (this.TargetRetrieves.Contains(bottom) is false)
                                this.Add2Retrieves.Add(bottom);
                    }
                }


                // 6. Add TargetRetrieves, Get Add1Retrieves
                this.Add1Retrieves.Clear();
                foreach (int i in this.Add2Retrieves)
                {
                    this.TargetRetrieves.Add(i);
                    this.Add1Retrieves.Add(i);
                }
                this.Add2Retrieves.Clear();
            }


            // 7. TargetRetrieves to Colors
            Array.Clear(this.Pixels, 0, this.Pixels.Length);
            foreach (int i in this.TargetRetrieves)
            {
                this.Pixels[i * 4 + 3] = color.A;
                this.Pixels[i * 4 + 2] = color.R;
                this.Pixels[i * 4 + 1] = color.G;
                this.Pixels[i * 4 + 0] = color.B;
            }
            this.TempRenderTarget.SetPixelBytes(this.Pixels);
            return true;
        }

    }
}