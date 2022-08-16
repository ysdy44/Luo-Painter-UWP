using Luo_Painter.Elements;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Text.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.TestApp
{
    public sealed partial class CustomEditorPage : Page
    {
        //@Key
        private bool IsKeyDown(VirtualKey key) => Window.Current.CoreWindow.GetKeyState(key).HasFlag(CoreVirtualKeyStates.Down);
        private bool IsCtrl => this.IsKeyDown(VirtualKey.Control);
        private bool IsShift => this.IsKeyDown(VirtualKey.Shift);


        readonly DispatcherTimer CaretTimer = new DispatcherTimer
        {
            Interval = System.TimeSpan.FromMilliseconds(500)
        };

        readonly InputPane InputPane = InputPane.GetForCurrentView();
        readonly CoreTextEditContext EditContext = CoreTextServicesManager.GetForCurrentView().CreateEditContext();
        bool InternalFocus;

        string Text = "The quick brown fox jumps over the lazy dog.";
        string TextRange(CoreTextRange range) => this.Text.Substring(range.StartCaretPosition, range.EndCaretPosition - range.StartCaretPosition);

        CoreTextRange Selection;
        int Start { get => this.Selection.StartCaretPosition; set => this.Selection.StartCaretPosition = value; }
        int End { get => this.Selection.EndCaretPosition; set => this.Selection.EndCaretPosition = value; }

        Rect TextBounds;
        Rect ControlBounds;

        public CustomEditorPage()
        {
            this.InitializeComponent();
            base.Loaded += (s, e) => this.Focus(FocusState.Keyboard);
            base.LostFocus += (s, e) =>
            {
                this.ContentPanel.BorderBrush = null;

                if (this.InternalFocus)
                {
                    this.EditContext.NotifyFocusLeave();

                    this.InternalFocus = false;
                    this.InputPane.TryHide();
                    this.UpdateTextUI();
                }
            };
            base.GotFocus += (s, e) =>
            {
                this.ContentPanel.BorderBrush = new SolidColorBrush(Colors.DodgerBlue);

                if (!this.InternalFocus)
                {
                    this.InternalFocus = true;
                    this.UpdateTextUI();

                    this.EditContext.NotifyFocusEnter();
                }

                this.InputPane.TryShow();
            };


            base.KeyDown += (s, e) =>
            {
                switch (e.Key)
                {
                    case VirtualKey.Back: this.KeyBack(); break;
                    case VirtualKey.Delete: this.KeyDelete(); break;
                    case VirtualKey.Left: this.KeyLeft(this.IsShift); break;
                    case VirtualKey.Right: this.KeyRight(this.IsShift); break;
                    case VirtualKey.A: if (this.IsCtrl) this.SelectAll(); break;
                    case VirtualKey.S: if (this.IsCtrl) this.SelectToStart(); break;
                    case VirtualKey.E: if (this.IsCtrl) this.SelectToEnd(); break;
                    default: break;
                }
            };


            this.EditContext.InputPaneDisplayPolicy = CoreTextInputPaneDisplayPolicy.Manual;
            this.EditContext.InputScope = CoreTextInputScope.Text;

            this.EditContext.FocusRemoved += (sender, args) =>
            {
                this.InternalFocus = false;
                this.InputPane.TryHide();
                this.UpdateTextUI();
            };

            this.EditContext.SelectionRequested += (sender, args) => args.Request.Selection = this.Selection;
            this.EditContext.SelectionUpdating += (sender, args) => this.Selection = args.Selection;

            this.EditContext.TextRequested += (sender, args) =>
            {
                int startIndex = args.Request.Range.StartCaretPosition;
                int endIndex = System.Math.Min(args.Request.Range.EndCaretPosition, this.Text.Length);
                args.Request.Text = this.Text.Substring(startIndex, endIndex - startIndex);
            };
            this.EditContext.TextUpdating += (sender, args) =>
            {
                if (this.Selection.HasSelection()) this.Delete(true);

                this.Text =
                this.Text.Substring(0, args.Range.StartCaretPosition) +
                args.Text +
                this.Text.Substring(System.Math.Min(this.Text.Length, args.Range.EndCaretPosition));

                this.Start = this.End = args.NewSelection.StartCaretPosition;
                this.UpdateTextUI();
            };

            this.EditContext.FormatUpdating += (sender, args) =>
            {
                if (args.TextColor is null) { }

                if (args.BackgroundColor is null) { }

                if (args.UnderlineType is null) { }
            };

            this.EditContext.CompositionStarted += (s, e) => { };
            this.EditContext.CompositionCompleted += (s, e) => { };

            this.CaretTimer.Tick += (s, e) =>
            {
                switch (this.Caret.Visibility)
                {
                    case Visibility.Visible: this.Caret.Visibility = Visibility.Collapsed; break;
                    case Visibility.Collapsed: this.Caret.Visibility = Visibility.Visible; break;
                    default: break;
                }
            };

            this.EditContext.LayoutRequested += (sender, args) =>
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
        }

        private void UpdateTextUI()
        {
            CoreTextRange range = this.Selection;

            this.BeforeSelectionTextBlock.Text = this.Text.Substring(0, range.StartCaretPosition) + "\ufeff";
            if (this.Selection.HasSelection())
            {
                this.Caret.Visibility = Visibility.Collapsed;
                this.CaretTimer.Stop();

                this.SelectionTextBlock.Text = this.TextRange(range) + "\ufeff";
            }
            else
            {
                this.SelectionTextBlock.Text = "";

                if (this.InternalFocus)
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

            this.AfterSelectionTextBlock.Text = this.Text.Substring(range.EndCaretPosition) + "\ufeff";

            this.FullTextBlock.Text = this.Text;
            this.HasSelectionTextBlock.Text = this.Selection.HasSelection().ToString();
            this.SelectionStartIndexTextBlock.Text = range.StartCaretPosition.ToString();
            this.SelectionEndIndexTextBlock.Text = range.EndCaretPosition.ToString();
        }


        public void KeyBack() => this.Delete(true);
        public void KeyDelete() => this.Delete(false);

        public void KeyLeft(bool isShift = false) => this.Move(true, isShift);
        public void KeyRight(bool isShift = false) => this.Move(false, isShift);

        public void SelectAll() => this.Move(0, this.Text.Length);
        public void SelectToStart() => this.Move(0, 0);
        public void SelectToEnd() => this.Move(this.Text.Length, this.Text.Length);


        private void Move(int start, int end)
        {
            if (this.Start == start && this.End == end) return;

            this.Start = start;
            this.End = end;

            this.EditContext.NotifySelectionChanged(this.Selection);
            this.UpdateTextUI();
        }

        private void Move(bool isLeft, bool isShift)
        {
            if (isLeft)
            {
                if (this.End <= 0) return;
                this.End--;
            }
            else
            {
                if (this.End >= this.Text.Length) return;
                this.End++;
            }

            if (isShift is false)
            {
                this.Start = this.End;
            }

            this.EditContext.NotifySelectionChanged(this.Selection);
            this.UpdateTextUI();
        }

        private void Delete(bool isLeft)
        {
            if (this.Selection.HasSelection())
            {
                int first = this.Selection.First();
                int last = this.Selection.Last();

                if (first <= 0)
                {
                    if (last >= this.Text.Length)
                        this.Text = string.Empty;
                    else
                        this.Text = this.Text.Substring(last);

                    this.End = this.Start = 0;
                }
                else
                {
                    if (last >= this.Text.Length)
                        this.Text = this.Text.Substring(0, first);
                    else
                        this.Text = this.Text.Substring(0, first) + this.Text.Substring(last);

                    this.End = this.Start = first;
                }
            }
            else
            {
                int index = this.Selection.First();

                if (isLeft)
                {
                    if (index <= 0) return;
                    else if (index >= this.Text.Length)
                        this.Text = this.Text.Substring(0, index - 1);
                    else
                        this.Text = this.Text.Substring(0, index - 1) + this.Text.Substring(index);
                    this.Start = this.End = index - 1;
                }
                else
                {
                    if (index >= this.Text.Length) return;
                    else if (index <= 0)
                        this.Text = this.Text.Substring(index + 1);
                    else
                        this.Text = this.Text.Substring(0, index) + this.Text.Substring(index + 1);
                }
            }

            this.EditContext.NotifySelectionChanged(this.Selection);
            this.UpdateTextUI();
        }
    }
}