using Luo_Painter.Brushes;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Models;
using Microsoft.Graphics.Canvas;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Luo_Painter
{
    public sealed partial class BrushPage : Page, IInkParameter
    {

        public CanvasDevice CanvasDevice => this.InkParameter.CanvasDevice;

        BitmapLayer BitmapLayer { get; set; }

        //@Task
        readonly object Locker = new object();
        //@ Paint
        readonly PaintTaskCollection Tasks = new PaintTaskCollection();

        Vector2 StartingPosition;
        Vector2 Position;
        float StartingPressure;
        float Pressure;

        #region IInkParameter

        public InkType InkType { get => this.InkParameter.InkType; set => this.InkParameter.InkType = value; }
        public InkPresenter InkPresenter => this.InkParameter.InkPresenter;

        bool IsDrak;
        public Color Color => this.IsDrak ? Colors.White : Colors.Black;
        public Vector4 ColorHdr => this.IsDrak ? Vector4.One : Vector4.UnitW;

        public string TextureSelectedItem => this.InkParameter.TextureSelectedItem;
        public void ConstructTexture(string path) => this.InkParameter.ConstructTexture(path);
        public Task<ContentDialogResult> ShowTextureAsync() => this.InkParameter.ShowTextureAsync();

        IInkParameter InkParameter;
        public void Construct(IInkParameter item)
        {
            this.InkParameter = item;
        }
        public void TryInkAsync() => this.InkParameter.TryInkAsync();
        public void TryInk() => this.InkParameter.TryInk();

        #endregion

        //@Construct
        public BrushPage()
        {
            this.InitializeComponent();
            this.ConstructCanvas();
            this.ConstructOperator();

            this.ConstructInk();

            this.IsDrak = base.ActualTheme is ElementTheme.Dark;
            base.ActualThemeChanged += (s, e) => this.IsDrak = base.ActualTheme is ElementTheme.Dark;


            // Drag and Drop 
            base.AllowDrop = true;
            base.Drop += async (s, e) =>
            {
                //@Task
                if (System.Threading.Monitor.TryEnter(this.Locker)) System.Threading.Monitor.Exit(this.Locker);
                else return;

                if (e.DataView.Contains(StandardDataFormats.StorageItems) is false) return;

                foreach (IStorageItem item in await e.DataView.GetStorageItemsAsync())
                {
                    if (item is StorageFile file)
                    {
                        using (IRandomAccessStreamWithContentType stream = await file.OpenReadAsync())
                        {
                            CanvasBitmap bitmap = await CanvasBitmap.LoadAsync(this.CanvasControl, stream);
                            lock (this.Locker)
                            {
                                this.BitmapLayer.Draw(bitmap);

                                // History
                                this.BitmapLayer.Flush();

                                this.CanvasControl.Invalidate(); // Invalidate
                                return;
                            }
                        }
                    }
                }
            };
            base.DragOver += (s, e) =>
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
                //e.DragUIOverride.Caption = 
                e.DragUIOverride.IsCaptionVisible = e.DragUIOverride.IsContentVisible = e.DragUIOverride.IsGlyphVisible = true;
            };
        }

        //@BackRequested
        /// <summary> The current page no longer becomes an active page. </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.InkParameter = null;
        }
        //[DrawPageToBrushPage(NavigationMode.New)]
        /// <summary> The current page becomes the active page. </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is IInkParameter item)
            {
                this.InkParameter = item;
            }
            else
            {
                this.InkParameter = null;

                if (base.Frame.CanGoBack)
                {
                    base.Frame.GoBack();
                }
            }
        }

    }
}