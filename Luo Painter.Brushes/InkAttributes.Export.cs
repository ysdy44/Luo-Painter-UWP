using Microsoft.Graphics.Canvas.Effects;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Windows.UI.Input.Inking;

namespace Luo_Painter.Brushes
{
    public partial class InkAttributes
    {

        public XElement Save() => new XElement("Brush",
            new XAttribute("Type", this.Type),

            new XAttribute("Tile", this.Tile),

            // Property
            new XAttribute("Size", this.Size),
            new XAttribute("Opacity", this.Opacity),
            new XAttribute("Spacing", this.Spacing),
            new XAttribute("Flow", this.Flow),

            new XAttribute("MinSize", this.MinSize),
            new XAttribute("MinFlow", this.MinFlow),

            new XAttribute("SizePressure", this.SizePressure),
            new XAttribute("FlowPressure", this.FlowPressure),

            new XAttribute("IgnoreSizePressure", this.IgnoreSizePressure),
            new XAttribute("IgnoreFlowPressure", this.IgnoreFlowPressure),

            new XAttribute("Tip", this.Tip),
            new XAttribute("IsStroke", this.IsStroke),

            new XAttribute("Hardness", this.Hardness),

            // Texture
            new XAttribute("Rotate", this.Rotate),
            new XAttribute("RecolorShape", this.RecolorShape),
            new XAttribute("Shape", this.SaveTexture(this.Shape)),

            new XAttribute("Step", this.Step),
            new XAttribute("RecolorGrain", this.RecolorGrain),
            new XAttribute("Grain", this.SaveTexture(this.Grain)),

            new XAttribute("BlendMode", this.SaveBlendMode(this.BlendMode)),

            // Mix
            new XAttribute("Mix", this.Mix),
            new XAttribute("Wet", this.Wet),
            new XAttribute("Persistence", this.Persistence)
        );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string SaveBlendMode(BlendEffectMode value)
        {
            if (System.Enum.IsDefined(typeof(BlendEffectMode), value))
                return this.BlendMode.ToString();
            else
                return "None";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string SaveTexture(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            else
                return value;
        }


        public void Load(XElement element)
        {
            if (element.Attribute("Type") is XAttribute type) this.Type = this.LoadType(type.Value);

            // Property
            if (element.Attribute("Size") is XAttribute size) this.Size = (float)size;
            if (element.Attribute("Opacity") is XAttribute opacity) this.Opacity = (float)opacity;
            if (element.Attribute("Spacing") is XAttribute spacing) this.Spacing = (float)spacing;
            if (element.Attribute("Flow") is XAttribute flow) this.Flow = (float)flow;

            if (element.Attribute("MinSize") is XAttribute minSize) this.MinSize = (float)minSize;
            if (element.Attribute("MinFlow") is XAttribute minFlow) this.MinFlow = (float)minFlow;

            if (element.Attribute("SizePressure") is XAttribute sizePressure) this.SizePressure = this.LoadPressure(sizePressure.Value);
            if (element.Attribute("FlowPressure") is XAttribute flowPressure) this.FlowPressure = this.LoadPressure(flowPressure.Value);

            if (element.Attribute("IgnoreSizePressure") is XAttribute ignoreSizePressure) this.IgnoreSizePressure = (bool)ignoreSizePressure;
            if (element.Attribute("IgnoreFlowPressure") is XAttribute ignoreFlowPressure) this.IgnoreFlowPressure = (bool)ignoreFlowPressure;

            if (element.Attribute("Tip") is XAttribute tip) this.Tip = this.LoadTip(tip.Value);
            if (element.Attribute("IsStroke") is XAttribute isStroke) this.IsStroke = (bool)isStroke;

            if (element.Attribute("Hardness") is XAttribute hardness) this.Hardness = this.LoadHardness(hardness.Value);

            // Texture
            if (element.Attribute("Rotate") is XAttribute rotate) this.Rotate = (bool)rotate;
            if (element.Attribute("RecolorShape") is XAttribute recolorShape) this.RecolorShape = (bool)recolorShape;
            if (element.Attribute("Shape") is XAttribute shape) this.Shape = shape.Value;

            if (element.Attribute("Step") is XAttribute step) this.Step = (int)step;
            if (element.Attribute("RecolorGrain") is XAttribute recolorGrain) this.RecolorGrain = (bool)recolorGrain;
            if (element.Attribute("Grain") is XAttribute grain) this.Grain = grain.Value;
         
            if (element.Attribute("BlendMode") is XAttribute blendMode) this.BlendMode = this.LoadBlendMode(blendMode.Value);

            // Mix
            if (element.Attribute("Mix") is XAttribute mix) this.Mix = (float)mix;
            if (element.Attribute("Wet") is XAttribute wet) this.Wet = (float)wet;
            if (element.Attribute("Persistence") is XAttribute persistence) this.Persistence = (float)persistence;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private InkType LoadType(string value)
        {
            switch (value)
            {
                case "General": return InkType.General;
                case "Tip": return InkType.Tip;
                case "Line": return InkType.Line;
                case "Blur": return InkType.Blur;
                case "Mosaic": return InkType.Mosaic;
                case "Erase": return InkType.Erase;
                case "Liquefy": return InkType.Liquefy;
                default: return InkType.General;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private BrushEasePressure LoadPressure(string value)
        {
            switch (value)
            {
                case "None": return BrushEasePressure.None;

                case "Linear": return BrushEasePressure.Linear;
                case "LinearFlip": return BrushEasePressure.LinearFlip;

                case "Quadratic": return BrushEasePressure.Quadratic;
                case "QuadraticFlip": return BrushEasePressure.QuadraticFlip;
                case "QuadraticReverse": return BrushEasePressure.QuadraticReverse;
                case "QuadraticFlipReverse": return BrushEasePressure.QuadraticFlipReverse;

                case "Mirror": return BrushEasePressure.Mirror;
                case "MirrorFlip": return BrushEasePressure.MirrorFlip;

                case "Symmetry": return BrushEasePressure.Symmetry;
                case "SymmetryFlip": return BrushEasePressure.SymmetryFlip;

                default: return BrushEasePressure.None;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private PenTipShape LoadTip(string value)
        {
            switch (value)
            {
                case "Circle": return PenTipShape.Circle;
                case "Rectangle": return PenTipShape.Rectangle;
                default: return PenTipShape.Circle;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private BrushEdgeHardness LoadHardness(string value)
        {
            switch (value)
            {
                case "None": return BrushEdgeHardness.None;
                case "Cosine": return BrushEdgeHardness.Cosine;
                case "Quadratic": return BrushEdgeHardness.Quadratic;
                case "Cube": return BrushEdgeHardness.Cube;
                case "Quartic": return BrushEdgeHardness.Quartic;
                default: return BrushEdgeHardness.None;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private BlendEffectMode LoadBlendMode(string value)
        {
            if (string.IsNullOrEmpty(value)) return (BlendEffectMode)(-1);
            if (value is "None" is false) return (BlendEffectMode)(-1);

            if (System.Enum.TryParse(typeof(BlendEffectMode), value, out object obj))
            {
                if (obj is BlendEffectMode blendMode)
                {
                    return blendMode;
                }
            }
            return (BlendEffectMode)(-1);
        }

    }
}