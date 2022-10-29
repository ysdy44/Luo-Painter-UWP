using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Xml.Linq;

namespace Luo_Painter.Brushes
{
    public partial class InkAttributes
    {

        public XElement Save()
        {
            XElement element = new XElement("Brush",
                new XAttribute("Type", this.Type),


                new XAttribute("Size", this.Size),
                new XAttribute("Opacity", this.Opacity),
                new XAttribute("Spacing", this.Spacing),
                new XAttribute("Flow", this.Flow),

                new XAttribute("IgnoreSizePressure", this.IgnoreSizePressure),
                new XAttribute("IgnoreFlowPressure", this.IgnoreFlowPressure),


                new XAttribute("Tip", this.Tip),
                new XAttribute("IsStroke", this.IsStroke),
                new XAttribute("Hardness", this.Hardness),


                new XAttribute("BlendMode", this.BlendMode),
                new XAttribute("Mix", this.Mix),
                new XAttribute("Wet", this.Wet),
                new XAttribute("Persistence", this.Persistence),


                new XAttribute("Rotate", this.Rotate),
                new XAttribute("Step", this.Step));

            if (string.IsNullOrEmpty(this.Shape) is false) element.Add(new XAttribute("Shape", this.Shape));
            if (string.IsNullOrEmpty(this.Grain) is false) element.Add(new XAttribute("Grain", this.Grain));

            return element;
        }

        public void Load(XElement element)
        {
            if (element.Attribute("Type") is XAttribute type)
            {
                switch (type.Value)
                {
                    case "General": this.Type = InkType.General; break;
                    case "Tip": this.Type = InkType.Tip; break;
                    case "Line": this.Type = InkType.Line; break;
                    case "Blur": this.Type = InkType.Blur; break;
                    case "Mosaic": this.Type = InkType.Mosaic; break;
                    case "Erase": this.Type = InkType.Erase; break;
                    case "Liquefy": this.Type = InkType.Liquefy; break;
                    default: this.Type = InkType.General; break;
                }
            }
            else this.Type = InkType.General;


            if (element.Attribute("Size") is XAttribute size) this.Size = (float)size;
            if (element.Attribute("Opacity") is XAttribute opacity) this.Opacity = (float)opacity;
            if (element.Attribute("Spacing") is XAttribute spacing) this.Spacing = (float)spacing;
            if (element.Attribute("Flow") is XAttribute flow) this.Flow = (float)flow;

            if (element.Attribute("IgnoreSizePressure") is XAttribute ignoreSizePressure) this.IgnoreSizePressure = (bool)ignoreSizePressure;
            if (element.Attribute("IgnoreFlowPressure") is XAttribute ignoreFlowPressure) this.IgnoreFlowPressure = (bool)ignoreFlowPressure;


            if (element.Attribute("Tip") is XAttribute tip)
            {
                switch (tip.Value)
                {
                    case "Circle": this.Tip = Windows.UI.Input.Inking.PenTipShape.Circle; break;
                    case "Rectangle": this.Tip = Windows.UI.Input.Inking.PenTipShape.Rectangle; break;
                    default: this.Tip = default; break;
                }
            }
            if (element.Attribute("IsStroke") is XAttribute isStroke) this.IsStroke = (bool)isStroke;

            if (element.Attribute("Hardness") is XAttribute hardness)
            {
                switch (hardness.Value)
                {
                    case "None": this.Hardness = BrushEdgeHardness.None; break;
                    case "Cosine": this.Hardness = BrushEdgeHardness.Cosine; break;
                    case "Quadratic": this.Hardness = BrushEdgeHardness.Quadratic; break;
                    case "Cube": this.Hardness = BrushEdgeHardness.Cube; break;
                    case "Quartic": this.Hardness = BrushEdgeHardness.Quartic; break;
                    default: this.Hardness = BrushEdgeHardness.None; break;
                }
            }


            if (element.Attribute("BlendMode") is XAttribute blendMode)
                if (string.IsNullOrEmpty(blendMode.Value))
                    if (blendMode.Value is "None" is false)
                        this.BlendMode = (BlendEffectMode)Enum.Parse(typeof(BlendEffectMode), blendMode.Value);

            if (element.Attribute("Mix") is XAttribute mix) this.Mix = (float)mix;
            if (element.Attribute("Wet") is XAttribute wet) this.Wet = (float)wet;
            if (element.Attribute("Persistence") is XAttribute persistence) this.Persistence = (float)persistence;


            if (element.Attribute("Rotate") is XAttribute rotate) this.Rotate = (bool)rotate;
            if (element.Attribute("Step") is XAttribute step) this.Step = (int)step;

            if (element.Attribute("Shape") is XAttribute shape) this.Shape = shape.Value;
            if (element.Attribute("Grain") is XAttribute grain) this.Grain = grain.Value;
        }

    }
}