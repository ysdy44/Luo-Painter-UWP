using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class LayerDragPage : Page
    {

        readonly CanvasDevice CanvasDevice = new CanvasDevice();

        LayerObservableCollection ObservableCollection { get; } = new LayerObservableCollection();
        IList<ILayer> Nodes { get; }

        public LayerDragPage()
        {
            this.InitializeComponent();
            this.ConstructLayers();

            this.Nodes = new List<ILayer>
            {
                new BitmapLayer(this.CanvasDevice, 128,128)
                {
                    Children =
                    {
                        new BitmapLayer(this.CanvasDevice, 128,128)
                        {
                            Depth = 1,
                            Children =
                            {
                                new BitmapLayer(this.CanvasDevice, 128,128) { Depth = 2 },
                                new BitmapLayer(this.CanvasDevice, 128,128) { Depth = 2 },
                            }
                        },
                        new BitmapLayer(this.CanvasDevice, 128,128)
                        {
                            Depth = 1,
                            Children =
                            {
                                new BitmapLayer(this.CanvasDevice, 128,128) { Depth = 2 },
                                new BitmapLayer(this.CanvasDevice, 128,128) { Depth = 2 },
                            }
                        },
                    }
                },
            };

            foreach (ILayer item in this.Nodes)
            {
                this.ObservableCollection.AddChild(item);
            }
        }

        private void ConstructLayers()
        {
            this.ListView.DragItemsStarting += (s, e) =>
            {
                //e.Cancel = true;

                foreach (ILayer item in e.Items)
                {
                    item.CacheIsExpand();
                }
            };
            this.ListView.DragItemsCompleted += (s, e) =>
            {
                foreach (ILayer item in e.Items)
                {
                    this.ObservableCollection.RemoveDragItems(item);
                }

                foreach (ILayer item in e.Items)
                {
                    this.ObservableCollection.AddDragItems(item);
                }

                foreach (ILayer item in e.Items)
                {
                    item.ApplyIsExpand();
                }
            };
        }

    }
}