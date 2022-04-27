using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.UI;

namespace Luo_Painter.Layers.Models
{
    public enum InkMode
    {
        None,

        Dry,
        WetWithOpacity,
        WetWithBlendMode,
        WetWithOpacityAndBlendMode,

        EraseDry,
        EraseWetWithOpacity,

        Liquefy,
    }

    public sealed partial class BitmapLayer : LayerBase, ILayer
    {

        public InkMode InkMode { get; set; }

        public ICanvasImage Origin => this.OriginRenderTarget;
        public ICanvasImage Source => this.SourceRenderTarget;
        public ICanvasImage Temp => this.TempRenderTarget;

        readonly CanvasRenderTarget OriginRenderTarget;
        readonly CanvasRenderTarget SourceRenderTarget;
        readonly CanvasRenderTarget TempRenderTarget;

        public CanvasDrawingSession CreateSourceDrawingSession() => this.SourceRenderTarget.CreateDrawingSession();
        public CanvasDrawingSession CreateTempDrawingSession() => this.TempRenderTarget.CreateDrawingSession();

        public void Flush() => this.OriginRenderTarget.CopyPixelsFromBitmap(this.SourceRenderTarget);
        public void CopyPixels(BitmapLayer bitmapLayer) => this.SourceRenderTarget.CopyPixelsFromBitmap(bitmapLayer.TempRenderTarget);

        public void DrawSource(ICanvasImage image)
        {
            using (CanvasDrawingSession ds = this.SourceRenderTarget.CreateDrawingSession())
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                ds.Blend = CanvasBlend.Copy;
                ds.DrawImage(image);
            }
        }

        public void Clear(Color color)
        {
            using (CanvasDrawingSession ds = this.OriginRenderTarget.CreateDrawingSession())
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.Clear(color);
            }
            using (CanvasDrawingSession ds = this.SourceRenderTarget.CreateDrawingSession())
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.Clear(color);
            }
        }

        public void ClearTemp()
        {
            using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.Clear(Colors.Transparent);
            }
        }


        public CompositeEffect GetWeting(float opacity) => new CompositeEffect
        {
            Sources =
            {
                this.OriginRenderTarget,
                new OpacityEffect
                {
                    Source = this.TempRenderTarget,
                    Opacity = opacity
                }
            }
        };
        public BlendEffect GetWeting(BlendEffectMode blendMode) => new BlendEffect
        {
            Background = this.OriginRenderTarget,
            Foreground = this.TempRenderTarget,
            Mode = blendMode
        };
        public BlendEffect GetWeting(float opacity, BlendEffectMode blendMode) => new BlendEffect
        {
            Background = this.OriginRenderTarget,
            Foreground = new OpacityEffect
            {
                Source = this.TempRenderTarget,
                Opacity = opacity
            },
            Mode = blendMode
        };

        public void FillCircleDry(Vector2 position, Vector2 targetPosition, float pressure, float targetPressure, float size, Color color)
        {
            using (CanvasDrawingSession ds = this.SourceRenderTarget.CreateDrawingSession())
            {
                ds.FillCircle(position, targetPosition, pressure, targetPressure, size, color);
            }
        }
        public void FillCircleWet(Vector2 position, Vector2 targetPosition, float pressure, float targetPressure, float size, Color color)
        {
            using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
            {
                ds.FillCircle(position, targetPosition, pressure, targetPressure, size, color);
            }
        }


        public ArithmeticCompositeEffect GetEraseWeting(float opacity) => new ArithmeticCompositeEffect
        {
            MultiplyAmount = 0,
            Source1Amount = 1,
            Source2Amount = -opacity,
            Offset = 0,
            Source1 = this.OriginRenderTarget,
            Source2 = this.TempRenderTarget,
        };

        public void ErasingDry(Vector2 position, Vector2 targetPosition, float pressure, float targetPressure, float size)
        {
            using (CanvasDrawingSession ds = this.SourceRenderTarget.CreateDrawingSession())
            {
                ds.ErasingDry(position, targetPosition, pressure, targetPressure, size);
            }
        }
        public void ErasingWet(Vector2 position, Vector2 targetPosition, float pressure, float targetPressure, float size)
        {
            using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
            {
                ds.ErasingWet(position, targetPosition, pressure, targetPressure, size);
            }
        }

    }
}