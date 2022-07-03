using FanKit.Transformers;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class ScrollerPage : Page
    {
        public UIElement Target => this.InkCanvas;

        public ScrollerPage()
        {
            this.InitializeComponent();
            this.ConstructScroller();

            this.InkCanvas.InkPresenter.InputDeviceTypes = (CoreInputDeviceTypes)7;
            this.InkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(new InkDrawingAttributes
            {
                Color = Colors.Red,
            });
        }


        private void ConstructScroller()
        {
            this.Thumb.DragStarted += async (s, e) =>
            {
                if (this.Target.Visibility == Visibility.Collapsed) return;
                await this.Scroller.RenderAsync(this.Target);
                this.Scroller.DragStarted();
                this.Target.Visibility = Visibility.Collapsed;
            };
            this.Thumb.DragDelta += (s, e) => this.Scroller.DragDelta(e.HorizontalChange, e.VerticalChange);
            this.Thumb.DragCompleted += (s, e) => this.Scroller.DragCompleted();

            this.Scroller.DragPageDownCompleted += (s, e) => this.Target.Visibility = Visibility.Visible;
            this.Scroller.DragPageUpCompleted += (s, e) =>
            {
                this.Clear();
                this.Target.Visibility = Visibility.Visible;
            };
        }


        public void Clear()
        {
            this.InkCanvas.InkPresenter.StrokeContainer.Clear();
        }

        public async Task<FileUpdateStatus?> Save()
        {
            // Get all strokes on the InkCanvas.
            IReadOnlyList<InkStroke> currentStrokes = InkCanvas.InkPresenter.StrokeContainer.GetStrokes();

            // Strokes present on ink canvas.
            if (currentStrokes.Count == 0) return null;

            // Let users choose their ink file using a file picker.
            // Initialize the picker.
            FileSavePicker savePicker = new FileSavePicker
            {
                DefaultFileExtension = ".one",
                SuggestedFileName = "InkSample",
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                FileTypeChoices =
                {
                    ["OneNote"] = new List<string>
                    {
                        ".one"
                    }
                }
            };

            // Show the file picker.
            StorageFile file = await savePicker.PickSaveFileAsync();
            // When chosen, picker returns a reference to the selected file.
            if (file is null) return null;

            // Prevent updates to the file until updates are 
            // finalized with call to CompleteUpdatesAsync.
            CachedFileManager.DeferUpdates(file);

            // Open a file stream for writing.
            // Write the ink strokes to the output stream.
            using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            using (IOutputStream outputStream = stream.GetOutputStreamAt(0))
            {
                await this.InkCanvas.InkPresenter.StrokeContainer.SaveAsync(outputStream);
                await outputStream.FlushAsync();
            }

            // Finalize write so other apps can update file.
            return await CachedFileManager.CompleteUpdatesAsync(file);
        }

        public static NodeCollection CreateNodeCollection(IEnumerable<InkStroke> inkStrokes)
        {
            NodeCollection nodes = new NodeCollection();
            bool isBegin = false;

            foreach (InkStroke stroke in inkStrokes)
            {
                foreach (InkStrokeRenderingSegment segments in stroke.GetRenderingSegments())
                {
                    nodes.Add(new Node
                    {
                        Type = isBegin ? NodeType.Node : NodeType.BeginFigure,
                        Point = segments.Position.ToVector2(),
                        LeftControlPoint = segments.BezierControlPoint1.ToVector2(),
                        RightControlPoint = segments.BezierControlPoint2.ToVector2(),
                        IsSmooth = true,
                        IsChecked = false
                    });
                    isBegin = true;
                }

                nodes.Add(new Node
                {
                    Type = NodeType.EndFigure
                });
                isBegin = false;
            }

            return nodes;
        }

    }
}