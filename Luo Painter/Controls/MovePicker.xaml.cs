using Luo_Painter.Elements;
using System.Numerics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class MovePicker : UserControl
    {
        //@Delegate
        public event RoutedEventHandler XClick { remove => this.XButton.Click -= value; add => this.XButton.Click += value; }
        public event RoutedEventHandler YClick { remove => this.YButton.Click -= value; add => this.YButton.Click += value; }

        //@Content
        public int X { get => this.XButton.Number; set => this.XButton.Number = value; }
        public int Y { get => this.YButton.Number; set => this.YButton.Number = value; }
        public Vector2 Value
        {
            get => new Vector2
            {
                X = this.XButton.Number, 
                Y = this.YButton.Number 
            };
            set
            {
                this.XButton.Number = (int)value.X;
                this.YButton.Number = (int)value.Y;
            }
        }

        public INumberBase XNumber => this.XButton;
        public INumberBase YNumber => this.YButton;

        //@Construct
        public MovePicker()
        {
            this.InitializeComponent();
        }
    }
}