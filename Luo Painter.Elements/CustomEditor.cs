using System;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Text.Core;
using Windows.UI.ViewManagement;

namespace Luo_Painter.Elements
{
    public sealed class CustomEditor
    {

        //@Delegate
        public event TypedEventHandler<CoreTextEditContext, CoreTextLayoutRequestedEventArgs> LayoutRequested { remove => this.EditContext.LayoutRequested -= value; add => this.EditContext.LayoutRequested += value; }

        public event System.Action UpdateTextUIAction;
        private void UpdateTextUI() => this.UpdateTextUIAction?.Invoke();

        //@Static
        private static string PreserveTrailingSpaces(string s) => s + "\ufeff";


        readonly CoreTextEditContext EditContext = CoreTextServicesManager.GetForCurrentView().CreateEditContext();

        public string Text = string.Empty;
        public string TextRange(CoreTextRange range) => this.Text.Substring(range.StartCaretPosition, range.EndCaretPosition - range.StartCaretPosition);

        public CoreTextRange Selection;

        public bool InternalFocus = false;

        bool ExtendingLeft = false;

        readonly InputPane InputPane = InputPane.GetForCurrentView();

        readonly CoreWindow CoreWindow = CoreWindow.GetForCurrentThread();

        public CustomEditor()
        {
            this.EditContext.InputPaneDisplayPolicy = CoreTextInputPaneDisplayPolicy.Manual;
            this.EditContext.InputScope = CoreTextInputScope.Text;
            this.EditContext.TextRequested += (sender, args) =>
            {
                CoreTextTextRequest request = args.Request;

                int startIndex = request.Range.StartCaretPosition;
                int endIndex = Math.Min(request.Range.EndCaretPosition, this.Text.Length);
                request.Text = this.Text.Substring(startIndex, endIndex - startIndex);
            };

            this.EditContext.SelectionRequested += (sender, args) =>
            {
                CoreTextSelectionRequest request = args.Request;
                request.Selection = this.Selection;
            };

            this.EditContext.FocusRemoved += (sender, args) => this.RemoveInternalFocusWorker();

            this.EditContext.TextUpdating += (sender, args) =>
            {
                CoreTextRange range = args.Range;
                string newText = args.Text;
                CoreTextRange newSelection = args.NewSelection;

                this.Text =
                this.Text.Substring(0, range.StartCaretPosition) +
                newText +
                this.Text.Substring(Math.Min(this.Text.Length, range.EndCaretPosition));

                newSelection.EndCaretPosition = newSelection.StartCaretPosition;

                this.SetSelection(newSelection);
            };

            this.EditContext.SelectionUpdating += (sender, args) =>
            {
                CoreTextRange range = args.Selection;

                this.SetSelection(range);
            };

            this.EditContext.FormatUpdating += (sender, args) =>
            {
                if (args.TextColor is null)
                {
                }
                else
                {
                }

                if (args.BackgroundColor is null)
                {
                }
                else
                {
                }

                if (args.UnderlineType is null)
                {
                }
                else
                {
                }
            };


            this.EditContext.CompositionStarted += (s, e) =>
            {
            };

            this.EditContext.CompositionCompleted += (s, e) =>
            {
            };

            this.UpdateTextUI();
        }


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


        public void KeyBack()
        {
            CoreTextRange range = this.Selection;

            if (this.HasSelection())
            {
                this.ReplaceText(range, "");
            }
            else if (range.StartCaretPosition > 0)
            {
                range.StartCaretPosition = range.StartCaretPosition - 1;
                this.ReplaceText(range, "");
            }
        }

        public void KeyDelete()
        {
            CoreTextRange range = this.Selection;

            if (this.HasSelection())
            {
                this.ReplaceText(range, "");
            }
            else if (range.EndCaretPosition < this.Text.Length)
            {
                range.EndCaretPosition = range.StartCaretPosition + 1;
                this.ReplaceText(range, "");
            }
        }

        public void KeyLeft()
        {
            CoreTextRange range = this.Selection;

            if (this.CoreWindow.GetKeyState(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down))
            {
                if (!this.HasSelection())
                {
                    this.ExtendingLeft = true;
                }

                this.AdjustSelectionEndpoint(-1);
            }
            else
            {
                if (this.HasSelection())
                {
                    range.EndCaretPosition = range.StartCaretPosition;
                    this.SetSelectionAndNotify(range);
                }
                else
                {
                    range.StartCaretPosition = Math.Max(0, range.StartCaretPosition - 1);
                    range.EndCaretPosition = range.StartCaretPosition;
                    this.SetSelectionAndNotify(range);
                }
            }
        }

        public void KeyRight()
        {
            CoreTextRange range = this.Selection;

            if (this.CoreWindow.GetKeyState(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down))
            {
                if (!this.HasSelection())
                {
                    this.ExtendingLeft = false;
                }

                this.AdjustSelectionEndpoint(+1);
            }
            else
            {
                if (this.HasSelection())
                {
                    range.StartCaretPosition = range.EndCaretPosition;
                    this.SetSelectionAndNotify(range);
                }
                else
                {
                    range.StartCaretPosition = Math.Min(this.Text.Length, range.StartCaretPosition + 1);
                    range.EndCaretPosition = range.StartCaretPosition;
                    this.SetSelectionAndNotify(range);
                }
            }
        }


        private void AdjustSelectionEndpoint(int direction)
        {
            CoreTextRange range = this.Selection;
            if (this.ExtendingLeft)
            {
                range.StartCaretPosition = Math.Max(0, range.StartCaretPosition + direction);
            }
            else
            {
                range.EndCaretPosition = Math.Min(this.Text.Length, range.EndCaretPosition + direction);
            }

            this.SetSelectionAndNotify(range);
        }


        private void ReplaceText(CoreTextRange modifiedRange, string text)
        {
            CoreTextRange range = this.Selection;

            this.Text = this.Text.Substring(0, modifiedRange.StartCaretPosition) +
              text +
              this.Text.Substring(modifiedRange.EndCaretPosition);

            range.StartCaretPosition = modifiedRange.StartCaretPosition + text.Length;
            range.EndCaretPosition = range.StartCaretPosition;

            this.SetSelection(range);

            this.EditContext.NotifyTextChanged(modifiedRange, text.Length, range);
        }

        public bool HasSelection() => this.Selection.StartCaretPosition != this.Selection.EndCaretPosition;

        private void SetSelection(CoreTextRange selection)
        {
            this.Selection = selection;

            this.UpdateTextUI();
        }

        private void SetSelectionAndNotify(CoreTextRange selection)
        {
            this.SetSelection(selection);
            this.EditContext.NotifySelectionChanged(this.Selection);
        }

    }

}