using System.Linq;
using System.Numerics;
using Windows.UI;

namespace Luo_Painter.Layers.Models
{
    public sealed partial class BitmapLayer
    {
        public Vector4 MixHdr { get; private set; } = Vector4.Zero;
        public Vector4 PersistenceHdr { get; private set; } = Vector4.Zero;

        Vector4 MixWetHdr;
        Vector4 StartingMixWetHdr;

        Vector2 StartingMixPoint;

        public Vector4 GetMix(Vector4 colorHdr, float mix)
        {
            float x = 1 - mix;
            float z = mix;
            return colorHdr * x + this.PersistenceHdr * z;
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
            this.MixWetHdr =
            this.StartingMixWetHdr =
            this.MixHdr =
            this.PersistenceHdr =
            new Vector4(1, 1, 1, 0);

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
            this.PersistenceHdr =
            new Vector4(color.R, color.G, color.B, color.A) / 255f;
        }

        public void Mix(Vector2 point, float size, float wet)
        {
            float distance = Vector2.Distance(this.StartingMixPoint, point); // Point
            float length = size * wet; // Size

            if (distance < length)
            {
                // 1. Lerp
                this.MixHdr = Vector4.Lerp(this.StartingMixWetHdr, this.MixWetHdr, distance / length);
                return;
            }

            // 2. Update
            this.MixHdr = this.MixWetHdr;
            this.StartingMixWetHdr = this.MixWetHdr;
            this.MixWetHdr.W = 0;

            this.StartingMixPoint = point;

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