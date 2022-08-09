using Luo_Painter.Elements;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.System;
using Windows.UI;
using Windows.UI.Text.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.TestApp
{
    public sealed partial class CustomEditorPage : Page
    {

        Rect TextBounds;
        Rect ControlBounds;

        readonly CustomEditor Editor = new CustomEditor
        {
            Text = "The quick brown fox jumps over the lazy dog."
        };
        readonly DispatcherTimer CaretTimer = new DispatcherTimer
        {
            Interval = System.TimeSpan.FromMilliseconds(500)
        };

        //@BackRequested
        public bool InternalFocus
        {
            set
            {
                if (value)
                {
                    this.Editor.SetInternalFocus();
                    this.ContentPanel.BorderBrush = new SolidColorBrush(Colors.DodgerBlue);
                }
                else
                {
                    this.Editor.RemoveInternalFocus();
                    this.ContentPanel.BorderBrush = null;
                }
            }
        }

        public CustomEditorPage()
        {
            this.InitializeComponent();
            base.Loaded += (s, e) => this.Focus(FocusState.Keyboard);
            base.LostFocus += (s, e) => this.InternalFocus = false;
            base.GotFocus += (s, e) => this.InternalFocus = true;
            base.KeyDown += (s, e) =>
            {
                switch (e.Key)
                {
                    case VirtualKey.Back: this.Editor.KeyBack(); break;
                    case VirtualKey.Delete: this.Editor.KeyDelete(); break;
                    case VirtualKey.Left: this.Editor.KeyLeft(); break;
                    case VirtualKey.Right: this.Editor.KeyRight(); break;
                    default: break;
                }
            };

            this.CaretTimer.Tick += (s, e) =>
            {
                switch (this.Caret.Visibility)
                {
                    case Visibility.Visible: this.Caret.Visibility = Visibility.Collapsed; break;
                    case Visibility.Collapsed: this.Caret.Visibility = Visibility.Visible; break;
                    default: break;
                }
            };

            this.Editor.LayoutRequested += (sender, args) =>
            {
                double scale = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;

                Rect bounds = Window.Current.CoreWindow.Bounds;
                double x = bounds.X * scale;
                double y = bounds.Y * scale;

                Point p1 = this.SelectionTextBlock.TransformToVisual(null).TransformPoint(default);
                this.TextBounds.X = x + p1.X * scale;
                this.TextBounds.Y = y + p1.Y * scale;
                this.TextBounds.Width = this.SelectionTextBlock.ActualWidth * scale;
                this.TextBounds.Height = this.SelectionTextBlock.ActualHeight * scale;
                args.Request.LayoutBounds.TextBounds = this.TextBounds;

                Point p2 = this.ContentPanel.TransformToVisual(null).TransformPoint(default);
                this.ControlBounds.X = x + p2.X * scale;
                this.ControlBounds.Y = y + p2.Y * scale;
                this.ControlBounds.Width = this.ContentPanel.ActualWidth * scale;
                this.ControlBounds.Height = this.ContentPanel.ActualHeight * scale;
                args.Request.LayoutBounds.ControlBounds = this.ControlBounds;
            };
            this.Editor.UpdateTextUIAction += () =>
            {
                CoreTextRange range = this.Editor.Selection;

                this.BeforeSelectionTextBlock.Text = this.Editor.Text.Substring(0, range.StartCaretPosition) + "\ufeff";
                if (this.Editor.HasSelection())
                {
                    this.Caret.Visibility = Visibility.Collapsed;
                    this.CaretTimer.Stop();

                    this.SelectionTextBlock.Text = this.Editor.TextRange(range) + "\ufeff";
                }
                else
                {
                    this.SelectionTextBlock.Text = "";

                    if (this.Editor.InternalFocus)
                    {
                        this.Caret.Visibility = Visibility.Visible;
                        this.CaretTimer.Start();
                    }
                    else
                    {
                        this.Caret.Visibility = Visibility.Collapsed;
                        this.CaretTimer.Stop();
                    }
                }

                this.AfterSelectionTextBlock.Text = this.Editor.Text.Substring(range.EndCaretPosition) + "\ufeff";

                this.FullTextBlock.Text = this.Editor.Text;
                this.HasSelectionTextBlock.Text = this.Editor.HasSelection().ToString();
                this.SelectionStartIndexTextBlock.Text = range.StartCaretPosition.ToString();
                this.SelectionEndIndexTextBlock.Text = range.EndCaretPosition.ToString();
            };
        }
    }
}