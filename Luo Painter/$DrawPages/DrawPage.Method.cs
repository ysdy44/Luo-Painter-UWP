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

        private ICanvasImage Transparent => new CanvasCommandList(this.CanvasControl);
        private ICanvasImage White => new ColorSourceEffect
        {
            Color = Colors.White
        };

        /// <summary>
        /// Export to ...
        /// </summary>
        private async Task<bool?> Export()
        {
            bool isClearWhite = this.ExportButton.FileFormat == CanvasBitmapFileFormat.Jpeg;

            // Export
            return await FileUtil.SaveCanvasImageFile
            (
                resourceCreator: this.CanvasControl,
                image: this.Render(isClearWhite ? this.White : this.Transparent),

                width: this.Transformer.Width,
                height: this.Transformer.Height,
                dpi: this.ExportButton.DPI,

                fileChoices: this.ExportButton.FileChoices,
                suggestedFileName: this.ApplicationView.Title,

                fileFormat: this.ExportButton.FileFormat,
                quality: 1
            );
        }


        private int FillContainsPoint(Vector2 point)
        {
            for (int i = this.ObservableCollection.Count - 1; i >= 0; i--)
            {
                ILayer item = this.ObservableCollection[i];
                if (item.FillContainsPoint(point)) return i;
            }

            return -1;
        }

        private bool TryFillContainsPoint(Vector2 point, out ILayer layer)
        {
            for (int i = this.ObservableCollection.Count - 1; i >= 0; i--)
            {
                ILayer item = this.ObservableCollection[i];
                if (item.FillContainsPoint(point))
                {
                    layer = item;
                    return true;
                }
            }

            layer = null;
            return false;
        }


        private ICanvasImage Render(ICanvasImage background)
        {
            for (int i = this.ObservableCollection.Count - 1; i >= 0; i--)
            {
                ILayer item = this.ObservableCollection[i];

                background = item.Render(background, item.Source);
            }
            return background;
        }

        private ICanvasImage Render(ICanvasImage background, Matrix3x2 matrix, CanvasImageInterpolation interpolationMode)
        {
            for (int i = this.ObservableCollection.Count - 1; i >= 0; i--)
            {
                ILayer item = this.ObservableCollection[i];

                background = item.Render(background, new Transform2DEffect
                {
                    InterpolationMode = interpolationMode,
                    TransformMatrix = matrix,
                    Source = item.Source,
                });
            }
            return background;
        }

        private ICanvasImage Render(ICanvasImage background, Matrix3x2 matrix, CanvasImageInterpolation interpolationMode, string id, ICanvasImage mezzanine)
        {
            for (int i = this.ObservableCollection.Count - 1; i >= 0; i--)
            {
                ILayer item = this.ObservableCollection[i];

                background = item.Render(background, new Transform2DEffect
                {
                    InterpolationMode = interpolationMode,
                    TransformMatrix = matrix,
                    Source = (item.Id == id) ? mezzanine : item.Source,
                });
            }
            return background;
        }

        public async Task<CanvasBitmap> AddAsync(IRandomAccessStreamReference reference)
        {
            if (reference is null) return null;

            try
            {
                using (IRandomAccessStreamWithContentType stream = await reference.OpenReadAsync())
                {
                    return await CanvasBitmap.LoadAsync(this.CanvasControl, stream);
                }
            }
            catch (Exception)
            {
                return null;
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