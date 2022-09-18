using Luo_Painter.Historys;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;

namespace Luo_Painter.Layers
{
    public sealed partial class LayerNodes : List<ILayer>, ILayerRender
    {

        public IEnumerable<XElement> Save() =>
            from layer
            in this
            select
            layer.Children.Count is 0 ?
            new XElement("Layerage", new XAttribute("Id", layer.Id)) :
            new XElement("Layerage", new XAttribute("Id", layer.Id), new XElement("Children", layer.Children.Save()));

        public void Load(XElement element)
        {
            if (element is null) return;

            foreach (XElement item in element.Elements("Layerage"))
            {
                if (item.Attribute("Id") is XAttribute id2)
                {
                    string id = id2.Value;
                    if (string.IsNullOrEmpty(id)) continue;

                    ILayer layer = LayerDictionary.Instance[id];
                    base.Add(layer);

                    XElement children = item.Element("Children");
                    if (children is null) continue;

                    layer.Children.Load(children);
                }
            }
        }

        public void Load(Layerage[] layerages)
        {
            if (layerages is null) return;

            foreach (Layerage item in layerages)
            {
                string id = item.Id;
                if (string.IsNullOrEmpty(id)) continue;

                ILayer layer = LayerDictionary.Instance[id];
                layer.Children.Clear();
                base.Add(layer);

                Layerage[] children = item.Children;
                if (children is null) continue;

                layer.Children.Load(children);
            }
        }

    }
}