using System.Numerics;
using Windows.UI;

namespace Luo_Painter.Brushes
{
    public sealed class InkMixer
    {
        Vector4 Color;
        public Vector4 ColorHdr => this.Color;
        public bool Cache(Color target)
        {
            if (target.A is byte.MinValue)
            {
                this.Color = Vector4.Zero;
                return false;
            }
            else if (target.A is byte.MaxValue)
            {
                this.Color.X = target.R / 255f;
                this.Color.Y = target.G / 255f;
                this.Color.Z = target.B / 255f;
            }

            this.Color.W = target.A / 255f;
            return true;
        }
        public bool Mix(Color target, float opacity)
        {
            if (this.Color.W is 0f) return this.Cache(target);

            // Opacity
            // 0.0 ~ 1.0

            // 0.0 ~ 0.5
            // 0.5 ~ 1.0

            // Flow
            // 0.48 ~ 0.98

            float flow = opacity / 2f + 0.5f - 0.02f;
            float targetFlow = 1f - flow;

            if (target.A is byte.MinValue)
            {
                this.Color.W *= flow;
                return false;
            }
            else if (target.A is byte.MaxValue)
            {
                this.Color.X = this.Color.X * flow + target.R / 255f * targetFlow;
                this.Color.Y = this.Color.Y * flow + target.G / 255f * targetFlow;
                this.Color.Z = this.Color.Z * flow + target.B / 255f * targetFlow;
            }

            this.Color.W = this.Color.W * flow + target.A / 255f * targetFlow;
            return true;
        }
    }
}