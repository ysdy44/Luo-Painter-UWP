using System;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Luo_Painter.Models
{
    /// <summary>
    /// Result of <see cref="FileImageSource"/>.
    /// </summary>
    public enum FileImageSourceResult
    {
        /// <summary> Result without Source. </summary>
        None,
        /// <summary> String Source. </summary>
        Static,
        /// <summary> Writeable Source. </summary>
        Dynamic,
        /// <summary> Writeable Source with Refreshing. </summary>
        DynamicWithRefresh,
    }

    /// <summary>
    /// Represents an Source of the Image-Control
    /// whose data can be Refreshed 
    /// after changes to the Image-File.
    /// </summary>
    public sealed class FileImageSource
    {
        BitmapImage BitmapImage;
        WriteableBitmap WriteableBitmap;

        /// <summary>
        /// Gets the Source of the Image-Control.
        /// </summary>
        public ImageSource ImageSource
        {
            get
            {
                if (this.WriteableBitmap is null)
                {
                    if (this.BitmapImage is null)
                    {
                        // None
                        return null;
                    }
                    else
                    {
                        // Static
                        return this.BitmapImage;
                    }
                }
                else
                {
                    // Dynamic
                    return this.WriteableBitmap;
                }
            }
        }

        //@Construct
        /// <summary>
        /// Constructs a FileImageSource.
        /// </summary>
        /// <param name="path"> The path of Image-File. </param>
        public FileImageSource(string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            this.BitmapImage = new BitmapImage(new Uri(path));
        }

        /// <summary>
        /// Refresh the data.
        /// </summary>
        /// <param name="path"> The path of Image-File. </param>
        /// <returns> The refreshed result. </returns>
        public async Task<FileImageSourceResult> Refresh(string path)
        {
            if (string.IsNullOrEmpty(path)) return FileImageSourceResult.None;

            if (this.WriteableBitmap is null)
            {
                if (this.BitmapImage is null)
                {
                    // None
                    this.BitmapImage = new BitmapImage(new Uri(path));
                    return FileImageSourceResult.Static;
                }
                else
                {
                    // Static
                    StorageFile file;
                    try
                    {
                        file = await StorageFile.GetFileFromPathAsync(path);
                    }
                    catch (Exception)
                    {
                        return FileImageSourceResult.None;
                    }

                    if (file is null) return FileImageSourceResult.None;

                    using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
                    using (SoftwareBitmap softwareBitmap = await stream.ToSoftwareBitmap())
                    {
                        this.WriteableBitmap = new WriteableBitmap(softwareBitmap.PixelWidth, softwareBitmap.PixelHeight);
                        softwareBitmap.CopyToBuffer(this.WriteableBitmap.PixelBuffer);
                        return FileImageSourceResult.Dynamic;
                    }
                }
            }
            else
            {
                // Dynamic
                StorageFile file;

                try
                {
                    file = await StorageFile.GetFileFromPathAsync(path);
                }
                catch (Exception)
                {
                    return FileImageSourceResult.None;
                }

                if (file is null) return FileImageSourceResult.None;

                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    this.WriteableBitmap.SetSource(stream);
                    this.WriteableBitmap.Invalidate();
                    return FileImageSourceResult.DynamicWithRefresh;
                }
            }
        }
    }
}