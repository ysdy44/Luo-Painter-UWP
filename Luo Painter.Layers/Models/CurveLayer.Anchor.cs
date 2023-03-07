using FanKit.Transformers;
using Luo_Painter.Blends;
using Microsoft.Graphics.Canvas;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;

namespace Luo_Painter.Layers.Models
{
    public sealed partial class CurveLayer
    {

        public IList<AnchorCollection> Anchorss;

        public int Index = -1;
        public AnchorCollection SelectedItem => (this.Index is -1) ? null : this.Anchorss[this.Index];

        public void RemoveSelectedItem()
        {
            this.Anchorss.RemoveAt(this.Index);
            this.Index = System.Math.Clamp(this.Index - 1, -1, this.Anchorss.Count);
        }

        public void BoxChoose(TransformerRect boxRect)
        {
            foreach (AnchorCollection item in this.Anchorss)
            {
                item.BoxChoose(boxRect);
            }
        }
        public void BoxSelect(TransformerRect boxRect)
        {
            foreach (AnchorCollection item in this.Anchorss)
            {
                item.IsChecked = item.Any(c => c.Contained(boxRect));
            }
        }
        public void Deselect()
        {
            foreach (AnchorCollection anchors in this.Anchorss)
            {
                foreach (Anchor item in anchors)
                {
                    item.IsChecked = false;
                }
            }
        }
        public void CacheTransform()
        {
            foreach (AnchorCollection anchors in this.Anchorss)
            {
                foreach (Anchor item in anchors)
                {
                    item.CacheTransform();
                }
            }
        }

        public void Invalidate()
        {
            using (CanvasDrawingSession ds = this.SourceRenderTarget.CreateDrawingSession())
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.Clear(Colors.Transparent);
                foreach (AnchorCollection anchors in this.Anchorss)
                {
                    ds.DrawImage(anchors.Source);
                }
            }
        }

    }
}