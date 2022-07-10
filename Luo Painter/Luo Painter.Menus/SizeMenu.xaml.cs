using Luo_Painter.Elements;
using System;
using Windows.ApplicationModel.Resources;

namespace Luo_Painter.Menus
{
    internal sealed class BrushSize
    {
        public double Size { get; set; }
        public string Text { get; set; }

        public int Number { get; set; }
        public double Preview { get; set; }

        //public double GetPreview()
        //{
        //    double a = (Number + 100000.0) / 1020.0 - 97;
        //    double b = (1.0 - 0.09 / (Number)) * 600 - 590;
        //    return (a + b) / 1.2 - 1.7;
        //}
    }

    public sealed partial class SizeMenu : Expander
    {
        //@Delegate
        public event EventHandler<double> ItemClick;

        //@Construct
        public SizeMenu()
        {
            this.InitializeComponent();
            this.ListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is BrushSize item)
                {
                    this.ItemClick?.Invoke(this, item.Size);//Delegate
                }
            };
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

    }
}