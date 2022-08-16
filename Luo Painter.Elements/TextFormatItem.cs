using System.Collections.Generic;

namespace Luo_Painter.Elements
{
    public interface ITextFormat
    {
        void RemoveSingle(int index);

        void Add(int first, int last);
        void Remove(int first, int last);

        void AddRange(int first, int last, int length);
        void RemoveRange(int first, int last, int length);

        void UpdateRange(int first, int last, int length);
        void Clear();
    }

    public sealed class TextFormatItem : List<int>, ITextFormat
    {

        public IList<int> Removes = new List<int>();

        public void RemoveSingle(int index)
        {
            for (int i = 0; i < base.Count; i++)
            {
                int n = this[i];
                if (n > index) this[i] = n - 1;
            }
            base.Remove(index);
        }

        public void Add(int first, int last)
        {
            for (int i = first; i < last; i++)
            {
                if (base.Contains(i)) return;
                else base.Add(i);
            }
        }
        public void Remove(int first, int last)
        {
            this.Removes.Clear();

            for (int i = first; i < last; i++)
            {
                if (base.Contains(i)) this.Removes.Add(i);
            }

            foreach (int item in this.Removes)
            {
                base.Remove(item);
            }
        }

        public void AddRange(int first, int last, int length)
        {
            this.Removes.Clear();

            for (int i = 0; i < base.Count; i++)
            {
                int n = this[i];
                if (n >= last) this[i] = n + length - last + first;
                else if (n >= first) this.Removes.Add(n);
            }

            foreach (int item in this.Removes)
            {
                base.Remove(item);
            }
        }
        public void RemoveRange(int first, int last, int length)
        {
            this.Removes.Clear();

            for (int i = 0; i < base.Count; i++)
            {
                int n = this[i];
                if (n >= last) this[i] = n - length;
                else if (n >= first) this.Removes.Add(n);
            }

            foreach (int item in this.Removes)
            {
                base.Remove(item);
            }
        }

        public void UpdateRange(int first, int last, int length)
        {
            int l = last - first;

            int offset;
            if (l is 0)
                offset = 1;
            if (l is 1 && length is 1)
                return;
            else if (length == l)
                offset = l - length;
            else
                offset = length - l;

            if (offset is 0) return;
            for (int i = 0; i < base.Count; i++)
            {
                int n = this[i];
                if (n >= first) this[i] = n + offset;
            }
        }
    }
}