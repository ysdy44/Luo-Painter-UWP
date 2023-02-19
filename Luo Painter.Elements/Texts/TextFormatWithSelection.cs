using Windows.UI.Text.Core;

namespace Luo_Painter.Elements
{
    public class TextFormatWithSelection : TextFormat
    {

        CoreTextRange selection;

        public CoreTextRange Selection { get => this.selection; set => this.selection = value; }
        public int Start { get => this.selection.StartCaretPosition; set => this.selection.StartCaretPosition = value; }
        public int End { get => this.selection.EndCaretPosition; set => this.selection.EndCaretPosition = value; }

        public string Text { get; set; }
        public override string ToString() => $"Range: {this.Start}~{this.End}";

        public void TextUpdating(CoreTextTextUpdatingEventArgs args)
        {
            this.Text =
            this.Text.Substring(0, args.Range.First()) +
            args.Text +
            this.Text.Substring(args.Range.Last(this.Text.Length));

            this.UpdateRange(args.Range.First(), args.Range.Last(), args.Text.Length);
            this.Start = this.End = args.NewSelection.StartCaretPosition;
        }

        public bool SelectAll() => this.Caret(0, this.Text.Length);
        public bool SelectToStart() => this.Caret(0, 0);
        public bool SelectToEnd() => this.Caret(this.Text.Length, this.Text.Length);

        public bool Left() => this.Move(true, false);
        public bool Right() => this.Move(false, false);
        public bool ShiftLeft() => this.Move(true, true);
        public bool ShiftRight() => this.Move(false, true);

        public void InsertSelection(string text)
        {
            if (this.Start <= 0)
            {
                this.Text =
                    text +
                this.Text.Substring(System.Math.Min(this.Text.Length, this.End));
            }
            else if (this.End >= this.Text.Length)
            {
                this.Text =
                this.Text.Substring(0, this.Start) +
                text;
            }
            else
            {
                this.Text =
                this.Text.Substring(0, this.Start) +
                text +
                this.Text.Substring(System.Math.Min(this.Text.Length, this.End));
            }

            base.AddRange(this.selection.First(), this.selection.Last(), text.Length);
            this.Start = this.End = this.selection.Last() + text.Length;
        }

        public void Insert(string text)
        {
            if (this.Start <= 0)
            {
                this.Text =
                    text +
                this.Text;
            }
            else if (this.End >= this.Text.Length)
            {
                this.Text =
                this.Text +
                text;
            }
            else
            {
                this.Text =
                this.Text.Substring(0, this.Start) +
                text +
                this.Text.Substring(System.Math.Min(this.Text.Length, this.End));
            }

            base.AddRange(this.selection.First(), this.selection.Last(), text.Length);
            this.Start = this.End = this.selection.Last() + text.Length;
        }

        private bool Caret(int start, int end)
        {
            if (this.Start == start && this.End == end) return false;

            this.Start = start;
            this.End = end;

            return true;
        }

        private bool Move(bool isLeft, bool isShift)
        {
            if (isLeft)
            {
                if (this.End <= 0) return false;
                this.End--;
            }
            else
            {
                if (this.End >= this.Text.Length) return false;
                this.End++;
            }

            if (isShift is false)
            {
                this.Start = this.End;
            }

            return true;
        }

        public void DeleteSelection()
        {
            int first = this.selection.First();
            int last = this.selection.Last();

            if (first <= 0)
            {
                if (last >= this.Text.Length)
                {
                    this.Text = string.Empty;
                    base.Clear();
                }
                else
                {
                    this.Text = this.Text.Substring(last);
                    base.RemoveRange(0, last, last);
                }

                this.End = this.Start = 0;
            }
            else
            {
                if (last >= this.Text.Length)
                {
                    this.Text = this.Text.Substring(0, first);
                    base.RemoveRange(0, first, first);
                }
                else
                {
                    this.Text = this.Text.Substring(0, first) + this.Text.Substring(last);
                    base.RemoveRange(first, last, this.selection.Length());
                }

                this.End = this.Start = first;
            }
        }

        public bool Delete(bool isLeft)
        {
            int index = this.selection.First();

            if (isLeft)
            {
                if (index <= 0) return false;
                else if (index >= this.Text.Length)
                    this.Text = this.Text.Substring(0, System.Math.Min(this.Text.Length, index - 1));
                else
                    this.Text = this.Text.Substring(0, index - 1) + this.Text.Substring(index);

                base.RemoveSingle(index - 2);
                this.Start = this.End = index - 1;
            }
            else
            {
                if (index >= this.Text.Length) return false;
                else if (index <= 0)
                    this.Text = this.Text.Substring(index + 1);
                else
                    this.Text = this.Text.Substring(0, index) + this.Text.Substring(index + 1);

                base.RemoveSingle(index - 1);
                this.Start = this.End = index;
            }

            return true;
        }

    }
}