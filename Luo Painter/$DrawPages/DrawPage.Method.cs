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

        private string ToChoices(CanvasBitmapFileFormat format)
        {
            switch (format)
            {
                case CanvasBitmapFileFormat.Bmp: return ".bmp";
                case CanvasBitmapFileFormat.Png: return ".png";
                case CanvasBitmapFileFormat.Jpeg: return ".jpeg";
                case CanvasBitmapFileFormat.Tiff: return ".tiff";
                case CanvasBitmapFileFormat.Gif: return ".gif";
                default: return null;
            }
        }
        private CanvasBitmapFileFormat ToFileFormat(string format)
        {
            switch (format)
            {
                case "BMP": return CanvasBitmapFileFormat.Bmp;
                case "PNG": return CanvasBitmapFileFormat.Png;
                case "JPEG": return CanvasBitmapFileFormat.Jpeg;
                case "TIFF": return CanvasBitmapFileFormat.Tiff;
                case "GIF": return CanvasBitmapFileFormat.Gif;
                default: return CanvasBitmapFileFormat.Jpeg;
            }
        }

        /// <summary>
        /// Export to ...
        /// </summary>
        private async Task<bool?> Export()
        {
            // Render
            ICanvasResourceCreator resourceCreator = this.CanvasControl;
            float width = (float)this.Transformer.Width;
            float height = (float)this.Transformer.Height;
            int dpi = int.TryParse(this.DPIComboBox.SelectedItem.ToString(), out int result) ? result : 96;
            bool isClearWhite = true;

            using (CanvasCommandList commandList = new CanvasCommandList(resourceCreator))
            {
                using (CanvasDrawingSession ds = commandList.CreateDrawingSession())
                {
                    //@DPI 
                    ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                    if (isClearWhite) ds.Clear(Colors.White);

                    if (dpi == 96)
                        this.Render(ds);
                    else
                        this.Render(ds, Matrix3x2.CreateScale(dpi / 96));
                }

                // Export
                CanvasBitmapFileFormat format = this.ToFileFormat(this.FormatComboBox.SelectedItem as string);
                return await FileUtil.SaveCanvasImageFile
                (
                    resourceCreator: resourceCreator,
                    image: commandList,
                    bounds: new Windows.Foundation.Rect(0, 0, width, height),

                    fileChoices: this.ToChoices(format),
                    suggestedFileName: this.ApplicationView.Title,

                    fileFormat: format,
                    quality: 1,
                    dpi: dpi
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

            try
            {
                using (IRandomAccessStreamWithContentType stream = await reference.OpenReadAsync())
                {
                    CanvasBitmap bitmap = await CanvasBitmap.LoadAsync(this.CanvasControl, stream);
                    this.ObservableCollection.Add(new BitmapLayer(this.CanvasControl, bitmap, this.Transformer.Width, this.Transformer.Height));
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}