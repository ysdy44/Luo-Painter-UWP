using Microsoft.Graphics.Canvas;

namespace Luo_Painter.Brushes
{
    public sealed partial class InkPresenter
    {

        public void ConstructMask(string textureMask, CanvasBitmap mask)
        {
            this.AllowMask = true;
            this.MaskTexture = textureMask;
            this.Mask?.Dispose();
            this.Mask = mask;
        }
        public void ClearMask()
        {
            this.AllowMask = false;
            this.MaskTexture = null;
            this.Mask?.Dispose();
            this.Mask = null;
        }

        public void TurnOffMask() => this.AllowMask = false;
        public bool TryTurnOnMask()
        {
            if (this.AllowMask) return false;
            if (this.MaskTexture is null) return false;
            if (this.Mask is null) return false;
            this.AllowMask = true;
            return true;
        }


        public void ConstructPattern(string texturePattern, CanvasBitmap pattern)
        {
            this.AllowPattern = true;
            this.PatternTexture = texturePattern;
            this.Pattern?.Dispose();
            this.Pattern = pattern;
        }
        public void ClearPattern()
        {
            this.AllowPattern = false;
            this.PatternTexture = null;
            this.Pattern?.Dispose();
            this.Pattern = null;
        }

        public void TurnOffPattern() => this.AllowPattern = false;
        public bool TryTurnOnPattern()
        {
            if (this.AllowPattern) return false;
            if (this.PatternTexture is null) return false;
            if (this.Pattern is null) return false;
            this.AllowPattern = true;
            return true;
        }

    }
}