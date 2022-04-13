using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page
    {

        /// <summary>
        /// Export to ...
        /// </summary>
        private async Task<bool?> Export()
        {
            // Render
            ICanvasResourceCreator resourceCreator = this.CanvasControl;
            bool isClearWhite = true;

            using (CanvasCommandList commandList = new CanvasCommandList(resourceCreator))
            {
                using (CanvasDrawingSession ds = commandList.CreateDrawingSession())
                {
                    //@DPI 
                    ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                    if (isClearWhite) ds.Clear(Colors.White);

                    this.Render(ds);
                }

                // Export
                return await FileUtil.SaveCanvasImageFile
                (
                    resourceCreator: resourceCreator,
                    image: commandList,

                    width: this.Transformer.Width,
                    height: this.Transformer.Height,
                    dpi: this.DPI,

                    fileChoices: this.FileChoices,
                    suggestedFileName: this.ApplicationView.Title,

                    fileFormat: this.FileFormat,
                    quality: 1
                );
            }
        }

        private void Render(CanvasDrawingSession ds)
        {
            for (int i = this.ObservableCollection.Count - 1; i >= 0; i--)
            {
                ILayer item = this.ObservableCollection[i];

                float opacity = item.Opacity;
                if (opacity == 0) continue;

                switch (item.Visibility)
                {
                    case Visibility.Visible:
                        ICanvasImage source = item.Source;
                        if (opacity == 1) ds.DrawImage(source);
                        else ds.DrawImage(new OpacityEffect
                        {
                            Opacity = opacity,
                            Source = source
                        });
                        break;
                    case Visibility.Collapsed:
                        break;
                    default:
                        break;
                }
            }
        }

        private void Render(CanvasDrawingSession ds, Matrix3x2 matrix)
        {
            for (int i = this.ObservableCollection.Count - 1; i >= 0; i--)
            {
                ILayer item = this.ObservableCollection[i];

                float opacity = item.Opacity;
                if (opacity == 0) continue;

                switch (item.Visibility)
                {
                    case Visibility.Visible:
                        ICanvasImage source = new Transform2DEffect
                        {
                            BorderMode = EffectBorderMode.Hard,
                            InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                            TransformMatrix = matrix,
                            Source = item.Source,
                        };
                        if (opacity == 1) ds.DrawImage(source);
                        else ds.DrawImage(new OpacityEffect
                        {
                            Opacity = opacity,
                            Source = source
                        });
                        break;
                    case Visibility.Collapsed:
                        break;
                    default:
                        break;
                }
            }
        }

        private void Render(CanvasDrawingSession ds, Matrix3x2 matrix, string id, ICanvasImage mezzanine)
        {
            for (int i = this.ObservableCollection.Count - 1; i >= 0; i--)
            {
                ILayer item = this.ObservableCollection[i];

                float opacity = item.Opacity;
                if (opacity == 0) continue;

                switch (item.Visibility)
                {
                    case Visibility.Visible:
                        ICanvasImage source = new Transform2DEffect
                        {
                            BorderMode = EffectBorderMode.Hard,
                            InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                            TransformMatrix = matrix,
                            Source = (item.Id == id) ? mezzanine : item.Source,
                        };
                        if (opacity == 1) ds.DrawImage(source);
                        else ds.DrawImage(new OpacityEffect
                        {
                            Opacity = opacity,
                            Source = source
                        });
                        break;
                    case Visibility.Collapsed:
                        break;
                    default:
                        break;
                }
            }
        }

        public async Task<bool?> AddAsync(IRandomAccessStreamReference reference)
        {
            if (reference == null) return null;

            ICanvasResourceCreator sender = this.CanvasControl;

            try
            {
                using (IRandomAccessStreamWithContentType stream = await reference.OpenReadAsync())
                {
                    CanvasBitmap bitmap = await CanvasBitmap.LoadAsync(this.CanvasControl, stream);
                    BitmapLayer bitmapLayer = new BitmapLayer(sender, bitmap, this.Transformer.Width, this.Transformer.Height);
                    this.Add(bitmapLayer);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Add(ILayer layer)
        {
            int index = this.LayerListView.SelectedIndex;

            if (index >= 0)
            {
                this.ObservableCollection.Insert(index, layer);
                this.LayerListView.SelectedIndex = index;
            }
            else
            {
                this.ObservableCollection.Add(layer);
                this.LayerListView.SelectedIndex = 0;
            }
        }

    }
}