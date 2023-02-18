using Luo_Painter.HSVColorPickers;
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
        public double X { get => this.XButton.Value; set => this.XButton.Value = value; }
        public double Y { get => this.YButton.Value; set => this.YButton.Value = value; }
        public Vector2 Value
        {
            get => new Vector2
            {
                X = (float)this.XButton.Value, 
                Y = (float)this.YButton.Value 
            };
            set
            {
                this.XButton.Value = value.X;
                this.YButton.Value = value.Y;
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