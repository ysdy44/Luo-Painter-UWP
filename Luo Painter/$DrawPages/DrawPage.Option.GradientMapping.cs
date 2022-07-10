using Luo_Painter.Layers;
using Luo_Painter.Options;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.Foundation;
using Windows.Graphics.Effects;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Luo_Painter
{
    internal sealed class GradientMesh
    {
        readonly int Length;
        readonly CanvasRenderTarget Map;
        public IGraphicsEffectSource Source => this.Map;

        public GradientMesh(ICanvasResourceCreator resourceCreator, int length = 256)
        {
            this.Length = length;
            this.Map = new CanvasRenderTarget(resourceCreator, length, 1, 96);
        }

        public void Render(ICanvasResourceCreator resourceCreator, IEnumerable<GradientStop> stops)
        {
            IEnumerable<CanvasGradientStop> array =
                from item
                in stops
                select new CanvasGradientStop
                {
                    Position = (float)item.Offset,
                    Color = item.Color,
                };

            CanvasLinearGradientBrush brush = new CanvasLinearGradientBrush(resourceCreator, array.ToArray())
            {
                StartPoint = Vector2.Zero,
                EndPoint = new Vector2(this.Length, 0),
            };

            using (CanvasDrawingSession ds = this.Map.CreateDrawingSession())
            {
                ds.Units = CanvasUnits.Pixels;

                ds.FillRectangle(0, 0, this.Length, 1, brush);
            }
        }
    }

    public sealed partial class DrawPage : Page, ILayerManager
    {

        readonly IDictionary<double, Color> Stops = new Dictionary<double, Color>
        {
            [0] = Colors.LightBlue,
            [0.3333] = Colors.LightSteelBlue,
            [0.6666] = Colors.LightGoldenrodYellow,
            [1] = Colors.PaleVioletRed,
        };

        private void SetGradientMapping()
        {
            this.GradientMappingSelector.Reset(this.Stops);
            this.GradientMesh.Render(this.CanvasDevice, this.GradientMappingSelector.Source);
        }


        private void ConstructGradientMapping()
        {
            this.GradientMappingSelector.ItemClick += (s, e) =>
            {
                this.GradientMappingSelector.SetCurrent(s);
                if (this.GradientMappingSelector.CurrentStop == null) return;

                this.ColorMenu.Show(this.GradientMappingSelector.CurrentStop.Color);
                this.ColorMenu.ShowAt(this.GradientMappingSelector.CurrentButton, Elements.ExpanderPlacementMode.Top);
            };

            this.GradientMappingSelector.ItemManipulationStarted += (s, e) =>
            {
                this.GradientMesh.Render(this.CanvasDevice, this.GradientMappingSelector.Source);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.GradientMappingSelector.ItemManipulationDelta += (s, e) =>
            {
                this.GradientMesh.Render(this.CanvasDevice, this.GradientMappingSelector.Source);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.GradientMappingSelector.ItemManipulationCompleted += (s, e) =>
            {
                this.GradientMesh.Render(this.CanvasDevice, this.GradientMappingSelector.Source);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.GradientMappingSelector.ItemPreviewKeyDown += (s, e) =>
            {
                if (e.Handled)
                {
                    this.GradientMesh.Render(this.CanvasDevice, this.GradientMappingSelector.Source);
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
                this.GradientMesh.Render(this.CanvasDevice, this.GradientMappingSelector.Source);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.GradientMappingSelector.ManipulationDelta += (s, e) =>
            {
                this.GradientMappingSelector.SetCurrentOffset(e.Position);
                this.GradientMesh.Render(this.CanvasDevice, this.GradientMappingSelector.Source);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.GradientMappingSelector.ManipulationCompleted += (s, e) =>
            {
                this.GradientMappingSelector.SetCurrentOffset(e.Position);
                this.GradientMesh.Render(this.CanvasDevice, this.GradientMappingSelector.Source);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
        }


        private ICanvasImage GetGradientMappingPreview(ICanvasImage image)
        {
            return new PixelShaderEffect(this.GradientMappingShaderCodeBytes)
            {
                Source2BorderMode = EffectBorderMode.Hard,
                Source1 = image,
                Source2 = this.GradientMesh.Source
            };
        }


        private void GradientMappingColorChanged(Color color)
        {
            this.GradientMappingSelector.SetCurrentColor(color);
            this.GradientMesh.Render(this.CanvasDevice, this.GradientMappingSelector.Source);
            this.CanvasVirtualControl.Invalidate(); // Invalidate
        }

    }
}