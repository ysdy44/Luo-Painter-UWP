using Windows.Foundation;
using Windows.UI.Text.Core;
using Windows.UI.ViewManagement;

namespace Luo_Painter.Elements
{
    public sealed class CustomEditor
    {

        //@Delegate
        public event TypedEventHandler<CoreTextEditContext, CoreTextLayoutRequestedEventArgs> LayoutRequested { remove => this.EditContext.LayoutRequested -= value; add => this.EditContext.LayoutRequested += value; }

        public event System.Action UpdateTextUIAction;
        public event System.Action UpdateTextLayoutAction;
        private void UpdateTextUI() => this.UpdateTextUIAction?.Invoke();
        private void UpdateTextLayout() => this.UpdateTextLayoutAction?.Invoke();

        readonly InputPane InputPane = InputPane.GetForCurrentView();
        readonly CoreTextEditContext EditContext = CoreTextServicesManager.GetForCurrentView().CreateEditContext();

        public bool InternalFocus { get; private set; }
        public string Text { get; set; } = string.Empty;
        public string TextRange(CoreTextRange range) => this.Text.Substring(range.StartCaretPosition, range.EndCaretPosition - range.StartCaretPosition);

        CoreTextRange selection;
        public CoreTextRange Selection => this.selection;

        public int Start { get => this.selection.StartCaretPosition; set => this.selection.StartCaretPosition = value; }
        public int End { get => this.selection.EndCaretPosition; set => this.selection.EndCaretPosition = value; }

        public int First() => System.Math.Min(this.Start, this.End);
        public int Last() => System.Math.Max(this.Start, this.End);
        public int Length() => System.Math.Abs(this.End - this.Start);
        public bool HasSelection() => this.Start != this.End;

        public CustomEditor()
        {
            this.EditContext.InputPaneDisplayPolicy = CoreTextInputPaneDisplayPolicy.Manual;
            this.EditContext.InputScope = CoreTextInputScope.Text;

            this.EditContext.FocusRemoved += (sender, args) => this.RemoveInternalFocusWorker();

            this.EditContext.SelectionRequested += (sender, args) => args.Request.Selection = this.selection;
            this.EditContext.SelectionUpdating += (sender, args) => this.selection = args.Selection;

            this.EditContext.TextRequested += (sender, args) =>
            {
                int startIndex = args.Request.Range.StartCaretPosition;
                int endIndex = System.Math.Min(args.Request.Range.EndCaretPosition, this.Text.Length);
                args.Request.Text = this.Text.Substring(startIndex, endIndex - startIndex);
            };
            this.EditContext.TextUpdating += (sender, args) =>
            {
                if (this.HasSelection()) this.Delete(true);

                this.Text =
                this.Text.Substring(0, args.Range.StartCaretPosition) +
                args.Text +
                this.Text.Substring(System.Math.Min(this.Text.Length, args.Range.EndCaretPosition));
                this.UpdateTextLayout();

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
        }


        public override string ToString() => $"Range: {this.Start}~{this.End}";

        public void NotifySelection() => this.EditContext.NotifySelectionChanged(this.selection);


        public void SetInternalFocus()
        {
            if (!this.InternalFocus)
            {
                this.InternalFocus = true;

                this.UpdateTextUI();

                this.EditContext.NotifyFocusEnter();
            }

            this.InputPane.TryShow();
        }

        public void RemoveInternalFocus()
        {
            if (this.InternalFocus)
            {
                this.EditContext.NotifyFocusLeave();

                this.RemoveInternalFocusWorker();
            }
        }

        private void RemoveInternalFocusWorker()
        {
            this.InternalFocus = false;

            this.InputPane.TryHide();

            this.UpdateTextUI();
        }


        public void KeyBack() => this.Delete(true);
        public void KeyDelete() => this.Delete(false);

        public void KeyLeft(bool isShift = false) => this.Move(true, isShift);
        public void KeyRight(bool isShift = false) => this.Move(false, isShift);

        public void SelectAll() => this.Caret(0, this.Text.Length);
        public void SelectToStart() => this.Caret(0, 0);
        public void SelectToEnd() => this.Caret(this.Text.Length, this.Text.Length);


        private void Caret(int start, int end)
        {
            if (this.Start == start && this.End == end) return;

            this.Start = start;
            this.End = end;

            this.NotifySelection();
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

            this.NotifySelection();
            this.UpdateTextUI();
        }

        private void Delete(bool isLeft)
        {
            if (this.HasSelection())
            {
                int first = this.First();
                int last = this.Last();

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
                int index = this.First();

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

            this.NotifySelection();
            this.UpdateTextUI();
            this.UpdateTextLayout();
        }

    }
}