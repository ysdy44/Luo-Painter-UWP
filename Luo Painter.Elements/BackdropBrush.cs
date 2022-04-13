using Windows.Graphics.Effects;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.Elements
{
    /// <summary>
    /// A brush which alters the colors of whatever is behind it in the application by applying a effect. 
    /// </summary>
    public abstract class BackdropBrush : XamlCompositionBrushBase
    {
        /// <summary>
        /// Create effect.
        /// </summary>
        /// <param name="source"> The source of background. </param>
        /// <returns> The product effect. </returns>
        public abstract IGraphicsEffect CreateEffect(IGraphicsEffectSource source);

        protected override void OnConnected()
        {
            if (base.CompositionBrush != null) return;

            // Abort if effects aren't supported.
            if (CompositionCapabilities.GetForCurrentView().AreEffectsSupported() == false) return;

            CompositionBackdropBrush backdrop = Window.Current.Compositor.CreateBackdropBrush();

            // Use a Win2D affect applied to a CompositionBackdropBrush.
            IGraphicsEffect graphicsEffect = this.CreateEffect(new CompositionEffectSourceParameter("backdrop"));
            CompositionEffectFactory effectFactory = Window.Current.Compositor.CreateEffectFactory(graphicsEffect, new string[0] { });
            CompositionEffectBrush effectBrush = effectFactory.CreateBrush();
            effectBrush.SetSourceParameter("backdrop", backdrop);

            this.CompositionBrush = effectBrush;
        }
        protected override void OnDisconnected()
        {
            // Dispose of composition resources when no longer in use.
            base.CompositionBrush?.Dispose();
            base.CompositionBrush = null;
        }
    }

}