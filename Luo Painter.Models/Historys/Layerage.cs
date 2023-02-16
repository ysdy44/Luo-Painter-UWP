using System;

namespace Luo_Painter.Models
{
    public sealed class Layerage : IDisposable
    {

        public string Id { get; set; }
        public Layerage[] Children { get; set; }

        public override string ToString() => this.Id;

        public void Dispose()
        {
            this.Id = null;
            if (this.Children is null) return;

            foreach (Layerage item in this.Children)
            {
                item.Dispose();
            }

            Array.Clear(this.Children, 0, this.Children.Length);
            this.Children = null;
        }

    }
}