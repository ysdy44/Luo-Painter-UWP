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


        public void DrawSource(ICanvasImage image)
        {
            using (CanvasDrawingSession ds = this.SourceRenderTarget.CreateDrawingSession())
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.Clear(Colors.Transparent);
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

        public void Flush()
        {
            using (CanvasDrawingSession ds = this.OriginRenderTarget.CreateDrawingSession())
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.Clear(Colors.Transparent);
                ds.DrawImage(this.SourceRenderTarget);
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

        public void FillCircleDry(Vector2 position, Vector2 targetPosition, float size, Color color)
            => this.FillCircle(position, targetPosition, size, color, this.SourceRenderTarget);
        public void FillCircleWet(Vector2 position, Vector2 targetPosition, float size, Color color)
            => this.FillCircle(position, targetPosition, size, color, this.TempRenderTarget);


        public ArithmeticCompositeEffect GetEraseWeting(float opacity) => new ArithmeticCompositeEffect
        {
            MultiplyAmount = 0,
            Source1Amount = 1,
            Source2Amount = -opacity,
            Offset = 0,
            Source1 = this.OriginRenderTarget,
            Source2 = this.TempRenderTarget,
        };

        public void ErasingDry(Vector2 position, Vector2 targetPosition, float size)
            => this.ErasingDryCore(position, targetPosition, size);
        public void ErasingWet(Vector2 position, Vector2 targetPosition, float size)
            => this.ErasingWetCore(position, targetPosition, size);

    }
}