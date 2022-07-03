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
        IList<ILayer> Collection { get; }

        public LayerDragPage()
        {
            this.InitializeComponent();
            this.ConstructLayers();

            this.Collection = new List<ILayer>
            {
                new BitmapLayer(this.CanvasDevice, 128,128)
                {
                    Children =
                    {
                        new BitmapLayer(this.CanvasDevice, 128,128)
                        {
                            Depth = 40,
                            Children =
                            {
                                new BitmapLayer(this.CanvasDevice, 128,128) { Depth = 80 },
                                new BitmapLayer(this.CanvasDevice, 128,128) { Depth = 80 },
                            }
                        },
                        new BitmapLayer(this.CanvasDevice, 128,128)
                        {
                            Depth = 40,
                            Children =
                            {
                                new BitmapLayer(this.CanvasDevice, 128,128) { Depth = 80 },
                                new BitmapLayer(this.CanvasDevice, 128,128) { Depth = 80 },
                            }
                        },
                    }
                },
            };

            foreach (ILayer item in this.Collection)
            {
                this.ObservableCollection.AddChild(item);
            }
        }

        private void ConstructLayers()
        {
            this.ListView.DragItemsStarting += (s, e) =>
            {

            };
            this.ListView.DragItemsCompleted += async (s, e) =>
            {
                await System.Threading.Tasks.Task.Delay(1000);

                foreach (ILayer item in e.Items)
                {
                    this.ObservableCollection.RemoveDragItems(item);
                }

                await System.Threading.Tasks.Task.Delay(1000);

                foreach (ILayer item in e.Items)
                {
                    this.ObservableCollection.AddDragItems(item);
                }
            };
        }

    }
}