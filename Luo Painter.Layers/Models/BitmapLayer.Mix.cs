using System.Linq;
using System.Numerics;
using Windows.UI;

namespace Luo_Painter.Layers.Models
{
    public sealed partial class BitmapLayer : LayerBase, ILayer
    {
        public Vector4 MixHdr { get; private set; } = Vector4.Zero;
        public Vector4 PersistenceHdr { get; private set; } = Vector4.Zero;

        Vector4 MixWetHdr;
        Vector4 StartingMixWetHdr;

        Vector2 StartingMixPoint;

        public Vector3 MixNormalize(float mix, float persistence)
        {
            float x = 1 - mix;
            float y = mix * (1 - persistence);
            float z = mix * persistence;
            return new Vector3(x, y, z);
        }

        public Vector4 GetMix(Vector4 colorHdr, float mix, float persistence)
        {
            float x = 1 - mix;
            float y = mix * (1 - persistence);
            float z = mix * persistence;
            return colorHdr * x + this.MixHdr * y + this.PersistenceHdr * z;
        }

        public void ConstructMix(Vector2 point)
        {
            this.MixWetHdr = Vector4.Zero;
            this.StartingMixWetHdr = Vector4.Zero;
            this.MixHdr = Vector4.Zero;
            this.PersistenceHdr = Vector4.Zero;

            this.StartingMixPoint = point;

            int x = (int)point.X;
            int y = (int)point.Y;

            if (x < 0) return;
            if (y < 0) return;
            if (x >= base.Width) return;
            if (y >= base.Height) return;

            Color color = this.OriginRenderTarget.GetPixelColors(x, y, 1, 1).Single();
            if (color == Colors.Transparent) return;

            this.MixWetHdr =
            this.StartingMixWetHdr =
            this.MixHdr =
              this.PersistenceHdr = new Vector4(color.R, color.G, color.B, color.A) / 255f;
        }

        public void Mix(Vector2 point, float size, float wet)
        {
            if (this.MixWetHdr == this.PersistenceHdr)
            {
                goto Recolor;
            }


            float length = Vector2.Distance(point, this.StartingMixPoint);
            float smooth = length / (size * wet);
            if (smooth < 1)
            {
                // 1. Mix
                this.MixHdr = Vector4.Lerp(this.StartingMixWetHdr, this.MixWetHdr, smooth);
                return;
            }


            // 2. Fade
            this.MixHdr = this.MixWetHdr;
            this.StartingMixWetHdr = this.MixWetHdr;
            this.MixWetHdr.W = 0;

            this.StartingMixPoint = point;


            Recolor:
            {
                int x = (int)point.X;
                int y = (int)point.Y;

                if (x < 0) return;
                if (y < 0) return;
                if (x >= base.Width) return;
                if (y >= base.Height) return;

                Color color = this.OriginRenderTarget.GetPixelColors(x, y, 1, 1).Single();
                if (color == Colors.Transparent) return;


                // 3. Recolor
                this.MixWetHdr = new Vector4(color.R, color.G, color.B, color.A) / 255f;
            }
        }
    }
}