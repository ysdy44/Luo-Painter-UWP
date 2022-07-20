using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Xml.Linq;
using Windows.UI.Xaml;

namespace Luo_Painter.Layers
{
    public abstract partial class LayerBase : IRender
    {

        public XElement Save(object type) => new XElement("Layer",
                new XAttribute("Id", this.Id),
                new XAttribute("Type", type),
                new XAttribute("Name", this.Name is null ? string.Empty : this.Name),
                new XAttribute("Opacity", this.Opacity),
                new XAttribute("BlendMode", this.BlendMode is null ? "None" : $"{this.BlendMode}"),
                new XAttribute("Visibility", this.Visibility),
                new XAttribute("IsExpand", this.IsExpand));

        public void Load(XElement element)
        {
            if (element.Attribute("Name") is XAttribute name) this.name = name.Value;
            if (element.Attribute("Opacity") is XAttribute opacity) this.opacity = (float)opacity;
            if (element.Attribute("BlendMode") is XAttribute blendMode)
                if (blendMode.Value is "None" is false)
                    this.blendMode = (BlendEffectMode)Enum.Parse(typeof(BlendEffectMode), blendMode.Value);
            if (element.Attribute("Visibility") is XAttribute visibility) this.visibility = visibility.Value == "Collapsed" ? Visibility.Collapsed : Visibility.Visible;
            if (element.Attribute("IsExpand") is XAttribute isExpand) this.isExpand = (bool)isExpand;
            this.RenderMode = this.GetRenderMode();
        }

    }
}