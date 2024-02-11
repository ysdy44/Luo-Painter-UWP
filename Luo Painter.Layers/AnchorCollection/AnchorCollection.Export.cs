using FanKit.Transformers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Xml.Linq;

namespace Luo_Painter.Layers
{
    partial class AnchorCollection
    {

        public XElement Save()
        {
            return new XElement
            (
                "AnchorCollection",

                new XElement("StrokeWidth", this.StrokeWidth),
                XML.SaveColor("Color", this.Color),

                new XElement("IsClosed", this.IsClosed),
                XML.SaveVector2("ClosePoint", this.ClosePoint),
                new XElement("CloseIsSmooth", this.CloseIsSmooth),

                new XElement("Data",
                    from anchor
                    in this
                    select new XElement
                    (
                        "Anchor",
                        new XAttribute("IsSmooth", anchor.IsSmooth),
                        new XAttribute("X", anchor.Point.X),
                        new XAttribute("Y", anchor.Point.Y)
                    ))
            );
        }

        private void Load(XElement element)
        {
            if (element.Element("StrokeWidth") is XElement strokeWidth) this.StrokeWidth = (float)strokeWidth;
            if (element.Element("Color") is XElement color) this.Color = FanKit.Transformers.XML.LoadColor(color);

            if (element.Element("IsClosed") is XElement isClosed) this.IsClosed = (bool)isClosed;
            if (element.Element("ClosePoint") is XElement closePoint) this.ClosePoint = FanKit.Transformers.XML.LoadVector2(closePoint);
            if (element.Element("CloseIsSmooth") is XElement closeIsSmooth) this.CloseIsSmooth = (bool)closeIsSmooth;

            if (element.Element("Data") is XElement data)
            {
                if (data.Elements("Anchor") is IEnumerable<XElement> anchors)
                {
                    foreach (XElement anchor in anchors)
                    {
                        if (anchor.Attribute("X") is XAttribute x && anchor.Attribute("Y") is XAttribute y)
                        {
                            Vector2 position = new Vector2((float)y, (float)x);
                            Anchor anchor1 = new Anchor
                            {
                                Point = position,
                                LeftControlPoint = position,
                                RightControlPoint = position,
                            };

                            if (anchor.Attribute("IsSmooth") is XAttribute isSmooth2) anchor1.IsSmooth = (bool)isSmooth2;
                            if (anchor.Attribute("Pressure") is XAttribute x2) anchor1.Pressure = (float)x2;
                            base.Add(anchor1);
                        }
                    }
                }
            }
        }

    }
}