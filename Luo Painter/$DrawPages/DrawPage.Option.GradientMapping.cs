using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.Foundation;
using Windows.Graphics.Effects;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

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
            this.GradientMappingSelector.Reset(this.Stops);
            this.GradientMapping();
        }

        private void ConstructGradientMapping()
        {
            this.GradientMappingSelector.ItemClick += (s, e) =>
            {
                this.GradientMappingSelector.SetCurrent(s);
                this.ColorShowAt(this.GradientMappingSelector);
            };

            this.GradientMappingSelector.ItemManipulationStarted += (s, e) =>
            {
                this.GradientMapping();
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.GradientMappingSelector.ItemManipulationDelta += (s, e) =>
            {
                this.GradientMapping();
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.GradientMappingSelector.ItemManipulationCompleted += (s, e) =>
            {
                this.GradientMapping();
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.GradientMappingSelector.ItemPreviewKeyDown += (s, e) =>
            {
                if (e.Handled)
                {
                    this.GradientMapping();
                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                }
            };

            this.GradientMappingSelector.ManipulationMode = ManipulationModes.TranslateX;
            this.GradientMappingSelector.ManipulationStarted += (s, e) =>
            {
                Point point = e.Position;
                bool result = this.GradientMappingSelector.Interpolation(point);
                if (result is false) return;

                this.GradientMappingSelector.SetCurrent(point);
                this.GradientMapping();
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.GradientMappingSelector.ManipulationDelta += (s, e) =>
            {
                this.GradientMappingSelector.SetCurrentOffset(e.Position);
                this.GradientMapping();
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.GradientMappingSelector.ManipulationCompleted += (s, e) =>
            {
                this.GradientMappingSelector.SetCurrentOffset(e.Position);
                this.GradientMapping();
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
        }

        private void GradientMapping()
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

    }
}