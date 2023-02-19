using System.Collections.Generic;

namespace Luo_Painter.Elements
{
    public class TextFormat : ITextFormat
    {

        public readonly IDictionary<string, TextFormatItem> FontFamily = new Dictionary<string, TextFormatItem>();
        public readonly TextFormatItem FontWeight = new TextFormatItem();
        public readonly TextFormatItem FontStyle = new TextFormatItem();
        public readonly TextFormatItem Underline = new TextFormatItem();
        public readonly TextFormatItem Strikethrough = new TextFormatItem();

        public void RemoveSingle(int index)
        {
            foreach (var item in this.FontFamily)
            {
                item.Value.RemoveSingle(index);
            }
            this.FontWeight.RemoveSingle(index);
            this.FontStyle.RemoveSingle(index);
            this.Underline.RemoveSingle(index);
            this.Strikethrough.RemoveSingle(index);
        }

        public void Add(int first, int last)
        {
            foreach (var item in this.FontFamily)
            {
                item.Value.Add(first, last);
            }
            this.FontWeight.Add(first, last);
            this.FontStyle.Add(first, last);
            this.Underline.Add(first, last);
            this.Strikethrough.Add(first, last);
        }
        public void Remove(int first, int last)
        {
            foreach (var item in this.FontFamily)
            {
                item.Value.Remove(first, last);
            }
            this.FontWeight.Remove(first, last);
            this.FontStyle.Remove(first, last);
            this.Underline.Remove(first, last);
            this.Strikethrough.Remove(first, last);
        }

        public void AddRange(int first, int last, int length)
        {
            foreach (var item in this.FontFamily)
            {
                item.Value.AddRange(first, last, length);
            }
            this.FontWeight.AddRange(first, last, length);
            this.FontStyle.AddRange(first, last, length);
            this.Underline.AddRange(first, last, length);
            this.Strikethrough.AddRange(first, last, length);
        }
        public void RemoveRange(int first, int last, int length)
        {
            foreach (var item in this.FontFamily)
            {
                item.Value.RemoveRange(first, last, length);
            }
            this.FontWeight.RemoveRange(first, last, length);
            this.FontStyle.RemoveRange(first, last, length);
            this.Underline.RemoveRange(first, last, length);
            this.Strikethrough.RemoveRange(first, last, length);
        }

        public void UpdateRange(int first, int last, int length)
        {
            foreach (var item in this.FontFamily)
            {
                item.Value.UpdateRange(first, last, length);
            }
            this.FontWeight.UpdateRange(first, last, length);
            this.FontStyle.UpdateRange(first, last, length);
            this.Underline.UpdateRange(first, last, length);
            this.Strikethrough.UpdateRange(first, last, length);
        }
        public void Clear()
        {
            foreach (var item in this.FontFamily)
            {
                item.Value.Clear();
            }
            this.FontFamily.Clear();
            this.FontWeight.Clear();
            this.FontStyle.Clear();
            this.Underline.Clear();
            this.Strikethrough.Clear();
        }
    }
}