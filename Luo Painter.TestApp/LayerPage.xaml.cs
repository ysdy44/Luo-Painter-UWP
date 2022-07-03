using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Luo_Painter.TestApp
{
    internal class LayerCommand : RelayCommand<ILayer>
    {
    }

    public sealed partial class LayerPage : Page
    {

        readonly CanvasDevice CanvasDevice = new CanvasDevice();

        ObservableCollection<ILayer> Layers { get; } = new ObservableCollection<ILayer>();

        private IEnumerable<string> Ids()
        {
            foreach (object item in this.ListView.SelectedItems)
            {
                if (item is ILayer layer)
                {
                    yield return layer.Id;
                }
            }
        }


        public LayerPage()
        {
            this.InitializeComponent();
            this.ConstructLayers();
            this.ConstructLayer();
        }

        private void ConstructLayers()
        {
            this.ListView.SelectionChanged += (s, e) =>
            {
                this.AddRun.Text = e.AddedItems == null ? "0" : e.AddedItems.Count.ToString();
                this.RemovedRun.Text = e.RemovedItems == null ? "0" : e.RemovedItems.Count.ToString();
            };

            this.VisualCommand.Click += (s, layer) =>
            {
                switch (layer.Visibility)
                {
                    case Visibility.Visible:
                        layer.Visibility = Visibility.Collapsed;
                        break;
                    case Visibility.Collapsed:
                        layer.Visibility = Visibility.Visible;
                        break;
                    default:
                        break;
                }
            };
        }

        private void ConstructLayer()
        {
            this.RemoveButton.Click += (s, e) =>
            {
                int startingIndex = this.ListView.SelectedIndex;

                foreach (string id in this.Ids().ToArray())
                {
                    if (this.Layers.FirstOrDefault(c => c.Id == id) is ILayer layer)
                    {
                        this.Layers.Remove(layer);
                    }
                }

                int index = Math.Min(startingIndex, this.Layers.Count - 1);
                this.ListView.SelectedIndex = index;
            };

            this.AddButton.Click += (s, e) =>
            {
                ICanvasResourceCreator sender = this.CanvasDevice;
                BitmapLayer bitmapLayer = new BitmapLayer(sender, 512, 512);
                this.Layers.Add(bitmapLayer);

                this.ListView.SelectedIndex = this.Layers.Count - 1;
            };

            this.SelectAllButton.Click += (s, e) =>
            {
                this.ListView.SelectAll();
            };
        }

    }
}