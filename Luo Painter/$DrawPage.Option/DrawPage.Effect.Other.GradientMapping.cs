using Luo_Painter.Elements;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using System.Collections.Generic;
using System.Numerics;
using Windows.UI;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {
        readonly IDictionary<double, Color> Stops = new Dictionary<double, Color>
        {
            [0] = Colors.LightBlue,
            [0.3333] = Colors.LightSteelBlue,
            [0.6666] = Colors.LightGoldenrodYellow,
            [1] = Colors.PaleVioletRed,
        };

        private void ResetGradientMapping()
        {
            using (CanvasLinearGradientBrush brush = new CanvasLinearGradientBrush(this.CanvasDevice, this.GradientMappingSelector.Data)
            {
                StartPoint = Vector2.Zero,
                EndPoint = new Vector2(256, 0),
            })
            using (CanvasDrawingSession ds = this.GradientMesh.CreateDrawingSession())
            {
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
           
                ds.FillRectangle(0, 0, 256, 1, brush);
            }
        }

        private void ConstructGradientMapping()
        {
            this.GradientMappingSelector.Reset(this.Stops);
            this.GradientMappingSelector.ItemClick += (s, e) =>
            {
                if (this.GradientMappingSelector.IsItemClickEnabled)
                {
                    this.GradientMappingSelector.SetCurrent(s);
                    this.ColorShowAt(this.GradientMappingSelector);
                }
            };
            this.GradientMappingSelector.ItemRemoved += (s, e) =>
            {
                this.ResetGradientMapping();
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.GradientMappingSelector.Invalidate += (s, e) =>
            {
                this.ResetGradientMapping();
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
        }

    }
}