using FanKit.Transformers;
using Luo_Painter.Elements;
using Microsoft.Graphics.Canvas.Geometry;

using System.Linq;
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
            this.EditModeToggleButton.Checked += (s, e) => AddModeToggleButton.IsEnabled = true;
            this.EditModeToggleButton.Unchecked += (s, e) => AddModeToggleButton.IsEnabled = false;
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
                if (this.Nodes is null) return;

                Matrix3x2 matrix = this.CanvasControl.Dpi.ConvertPixelsToDips();
                CanvasGeometry geometry = this.Nodes.CreateGeometry(sender).Transform(matrix);

                args.DrawingSession.DrawGeometry(geometry, Colors.DodgerBlue);
                args.DrawingSession.DrawNodeCollection(this.Nodes, matrix);
            };
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, device, properties) =>
            {
                Vector2 position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);

                if (this.Nodes is null)
                {
                    this.Nodes = new NodeCollection(position, position);
                }
                else if (EditModeToggleButton.IsChecked == true)
                {
                    if (AddModeToggleButton.IsChecked == true)
                    {
                        int id = -1;
                        for (int i = 0; i < this.Nodes.Count - 1; i++)
                        {
                            var item = this.Nodes[i];
                            item.IsChecked = false;

                            if (id == -1 && item.Point.X > position.X) id = i;
                        }
                        if (id == -1)
                            this.Nodes.PenAdd(new Node
                            {
                                Point = position,
                                LeftControlPoint = position,
                                RightControlPoint = position,
                                IsChecked = true,
                                IsSmooth = false,
                            });
                        else
                            this.Nodes.Insert(id, new Node
                            {
                                Point = position,
                                LeftControlPoint = position,
                                RightControlPoint = position,
                                IsChecked = true,
                                IsSmooth = false,
                            });
                    }
                    else
                        this.Nodes.SelectionOnlyOne(position, Matrix3x2.Identity);
                }
                else
                {
                    foreach (var item in this.Nodes)
                        item.IsChecked = false;

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
            this.Operator.Single_Delta += (point, device, properties) =>
            {
                Vector2 position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);

                if (EditModeToggleButton.IsChecked == true)
                {
                    var selItem = this.Nodes.FirstOrDefault(n => n.IsChecked);
                    if (selItem != null)
                        selItem.Point = position;
                }
                else
                {
                    this.Nodes[this.Nodes.Count - 2].Point = position;
                }

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Complete += (point, device, properties) =>
            {
                Vector2 position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);

                if (EditModeToggleButton.IsChecked == true)
                {
                    var selItem = this.Nodes.FirstOrDefault(n => n.IsChecked);
                    if (selItem != null)
                        selItem.Point = position;
                }
                else
                {
                    this.Nodes[this.Nodes.Count - 2].Point = position;
                }

                this.CanvasControl.Invalidate(); // Invalidate
            };
        }

    }
}