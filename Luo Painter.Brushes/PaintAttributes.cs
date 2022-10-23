using Microsoft.Graphics.Canvas.Effects;
using Windows.UI.Input.Inking;

namespace Luo_Painter.Brushes
{
    /// <summary>
    /// <see cref="InkDrawingAttributes"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PaintAttributes<T>
        where T : struct
    {

        /// <summary>
        /// <see cref=" InkType.None"/> <para/>
        /// <see cref=" InkType.General"/> <para/>
        /// <see cref=" InkType.Tip"/> <para/>
        /// <see cref=" InkType.Line"/> <para/>
        /// <see cref=" InkType.Blur"/> <para/>
        /// <see cref=" InkType.Mosaic"/> <para/>
        /// <see cref=" InkType.Erase"/> <para/>
        /// <see cref=" InkType.Liquefy"/> <para/>
        /// </summary>
        public InkType Type { get; set; } = InkType.General; /// <see cref="InkPresenter.GetType"/>

        /// <summary>
        /// <see cref=" InkType.None"/> <para/>
        /// <see cref=" InkType.Blend"/> <para/>
        /// <see cref=" InkType.Mix"/> <para/>
        /// </summary>
        public InkType Mode { get; set; } = InkType.None; /// <see cref="InkPresenter.GetType"/>


        public T Size { get; set; }
        public T Opacity { get; set; } /// <see cref="InkPresenter.GetType"/>

        public T Spacing { get; set; }
        public T Flow { get; set; }


        public bool IgnoreSizePressure { get; set; }
        public bool IgnoreFlowPressure { get; set; }

        public PenTipShape Tip { get; set; }
        public bool IsStroke { get; set; }


        public BlendEffectMode BlendMode { get; set; }
        public BrushEdgeHardness Hardness { get; set; }


        public bool Rotate { get; set; }
        public int Step { get; set; } = 1024;

        public string Shape { get; set; }
        public string Grain { get; set; }

    }
}