using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Luo_Painter.Elements
{
    public enum PipsMode
    {
        None,
        First,
        Last,
    }

    public sealed partial class PipsPivot : Canvas
    {

        bool AllowStoryboard = true;
        bool Inertia;
        double W;
        PipsMode Mode;

        private int index;
        public int Index
        {
            get => this.index;
            set
            {
                this.index = value;
                double w = this.W;
                double oldOffset = value * w;

                if (this.AllowStoryboard)
                {
                    this.SplineDoubleKeyFrame.Value = -oldOffset;
                    this.ScaleStoryboard.Begin();
                }
                else
                {
                    this.TranslateTransform.X = -oldOffset;
                }

                if (value <= 0)
                {
                    this.Mode = PipsMode.First;
                    this.Resize();
                }
                else if (value >= base.Children.Count - 1)
                {
                    this.Mode = PipsMode.Last;
                    this.Resize();
                }
                else
                {
                    this.Mode = PipsMode.None;
                    this.Resize();
                }
            }
        }

        public PipsPivot()
        {
            this.InitializeComponent();
            base.Loaded += (s, e) => this.Index = 0;
            this.ScaleStoryboard.Completed += (s, e) =>
            {
                this.AllowStoryboard = false;
                {
                    int count = this.Children.Count;
                    this.Index = (this.Index + count) % count;
                }
                this.AllowStoryboard = true;
            };

            base.ManipulationMode = ManipulationModes.All;
            base.ManipulationStarted += (s, e) =>
            {
                this.Inertia = false;
            };
            base.ManipulationDelta += (s, e) =>
            {
                if (this.Inertia is false)
                {
                    this.TranslateTransform.X += e.Delta.Translation.X;
                    return;
                }

                e.Complete();

                double w = this.W;
                double linear = e.Velocities.Linear.X;
                double scale = linear / 4;

                double x = this.TranslateTransform.X;
                double offset = x + w * scale;
                double oldOffset = -this.Index * w;

                if (offset > oldOffset + w / 2)
                {
                    this.Index--;
                }
                else if (offset < oldOffset - w / 2)
                {
                    this.Index++;
                }
                else
                {
                    this.Index = this.Index;
                }
            };
            base.ManipulationInertiaStarting += (s, e) =>
            {
                this.Inertia = true;
            };
            base.ManipulationCompleted += (s, e) =>
            {
            };
        }

        public void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.W = e.NewSize.Width;

            base.Width = base.Children.Count * e.NewSize.Width;
            foreach (FrameworkElement item in base.Children)
            {
                item.Width = e.NewSize.Width;
                item.Height = e.NewSize.Height;
            }

            this.Resize();
        }

        private void Resize()
        {
            switch (this.Mode)
            {
                case PipsMode.None:
                    for (int i = 0; i < base.Children.Count; i++)
                    {
                        UIElement item = base.Children[i];
                        Canvas.SetLeft(item, i * this.W);
                    }
                    break;
                case PipsMode.First:
                    for (int i = 0; i < base.Children.Count - 1; i++)
                    {
                        UIElement item = base.Children[i];
                        Canvas.SetLeft(item, i * this.W);
                    }
                    {
                        int i = base.Children.Count - 1;
                        UIElement item = base.Children[i];
                        Canvas.SetLeft(item, -this.W);
                    }
                    break;
                case PipsMode.Last:
                    for (int i = 1; i < base.Children.Count; i++)
                    {
                        UIElement item = base.Children[i];
                        Canvas.SetLeft(item, i * this.W);
                    }
                    {
                        int i = base.Children.Count;
                        UIElement item = base.Children[0];
                        Canvas.SetLeft(item, i * this.W);
                    }
                    break;
                default:
                    break;
            }
        }

    }
}