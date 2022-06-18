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

        Vector2 Position;
        BitmapLayer BitmapLayer;
        ObservableCollection<ILayer> Layers { get; } = new ObservableCollection<ILayer>();

        private IEnumerable<string> Ids()
        {
            foreach (object item in this.LayerListView.SelectedItems)
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
            this.ConstructCanvas();
            this.ConstructOperator();
        }

        private void ConstructLayers()
        {
            this.LayerListView.SelectionChanged += (s, e) =>
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

                this.CanvasControl.Invalidate(); // Invalidate
            };
        }

        private void ConstructLayer()
        {
            this.RemoveButton.Click += (s, e) =>
            {
                int startingIndex = this.LayerListView.SelectedIndex;

                foreach (string id in this.Ids().ToArray())
                {
                    if (this.Layers.FirstOrDefault(c => c.Id == id) is ILayer layer)
                    {
                        this.Layers.Remove(layer);
                    }
                }

                int index = Math.Min(startingIndex, this.Layers.Count - 1);
                this.LayerListView.SelectedIndex = index;

                this.CanvasControl.Invalidate(); // Invalidate
            };

            this.AddButton.Click += (s, e) =>
            {
                ICanvasResourceCreator sender = this.CanvasControl;
                BitmapLayer bitmapLayer = new BitmapLayer(sender, 512, 512);
                this.Layers.Add(bitmapLayer);

                this.LayerListView.SelectedIndex = this.Layers.Count - 1;
            };

            this.SelectAllButton.Click += (s, e) =>
            {
                this.LayerListView.SelectAll();
            };
        }

        private void ConstructCanvas()
        {
            this.CanvasControl.CreateResources += (sender, args) =>
            {
                BitmapLayer bitmapLayer = new BitmapLayer(sender, 512, 512);
                this.Layers.Add(bitmapLayer);

                this.LayerListView.SelectedIndex = 0;
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                args.DrawingSession.FillRectangle(0, 0, 512, 512, Colors.White);

                foreach (ILayer item in this.Layers)
                {
                    switch (item.Visibility)
                    {
                        case Visibility.Visible:
                            args.DrawingSession.DrawImage(item[BitmapType.Source]);
                            break;
                        case Visibility.Collapsed:
                            break;
                    }
                }
            };
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, properties) =>
            {
                this.Position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                if (this.BitmapLayer == null) return;
                if (this.BitmapLayer.Visibility == Visibility.Collapsed) return;

                Vector2 position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);

                Rect rect = position.GetRect(12 * properties.Pressure);
                this.BitmapLayer.Hit(rect);

                bool result = this.BitmapLayer.IsometricFillCircle(Colors.Black, this.Position, position, 1, 1, 12, 0.25f, BitmapType.Source);
                if (result is false) return;

                this.Position = position;

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Complete += (point, properties) =>
            {
                if (this.BitmapLayer == null) return;
                if (this.BitmapLayer.Visibility == Visibility.Collapsed) return;

                // History
                this.BitmapLayer.Flush();

                this.BitmapLayer.RenderThumbnail();
                this.CanvasControl.Invalidate(); // Invalidate
            };
        }

    }
}