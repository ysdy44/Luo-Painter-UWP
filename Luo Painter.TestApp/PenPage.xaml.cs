using FanKit.Transformers;
using Luo_Painter.Elements;
using Microsoft.Graphics.Canvas.Geometry;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class PenPage : Page
    {

        NodeCollection Nodes;

        public PenPage()
        {
            this.InitializeComponent();
            this.ConstructPen();
            this.ConstructCanvas();
            this.ConstructOperator();
        }

        private void ConstructPen()
        {
            this.SelectAllButton.Click += (s, e) =>
            {
                foreach (Node item in this.Nodes)
                {
                    item.IsSmooth = true;
                }
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.ClearButton.Click += (s, e) =>
            {
                this.Nodes = null;
                this.CanvasControl.Invalidate(); // Invalidate
            };
        }

        private void ConstructCanvas()
        {
            this.CanvasControl.CreateResources += (sender, args) =>
            {
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                if (this.Nodes == null) return;

                Matrix3x2 matrix = this.CanvasControl.Dpi.ConvertPixelsToDips();
                CanvasGeometry geometry = this.Nodes.CreateGeometry(sender).Transform(matrix);

                args.DrawingSession.DrawGeometry(geometry, Colors.DodgerBlue);
                args.DrawingSession.DrawNodeCollection(this.Nodes, matrix);
            };
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, properties) =>
            {
                Vector2 position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);

                if (this.Nodes == null)
                {
                    this.Nodes = new NodeCollection(position, position);
                }
                else
                {
                    this.Nodes.PenAdd(new Node
                    {
                        Point = position,
                        LeftControlPoint = position,
                        RightControlPoint = position,
                        IsChecked = true,
                        IsSmooth = false,
                    });
                }

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                Vector2 position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);

                this.Nodes[this.Nodes.Count - 2].Point = position;
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Complete += (point, properties) =>
            {
                Vector2 position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);

                this.Nodes[this.Nodes.Count - 2].Point = position;
                this.CanvasControl.Invalidate(); // Invalidate
            };
        }

    }
}