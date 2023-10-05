using Luo_Painter.Elements;
using Luo_Painter.UI;
using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class SimulateCanvas : Canvas
    {

        //@Delegate
        public event Action<Vector2> Start;
        public event Action<Vector2> Delta;
        public event Action<Vector2> Complete;

        private SimulateType type;
        public SimulateType Type
        {
            get => this.type;
            set
            {
                switch (value)
                {
                    case SimulateType.None:
                        base.Visibility = Visibility.Collapsed;
                        break;
                    default:

                        switch (value)
                        {
                            case SimulateType.Pointer:
                                this.Follow();
                                this.FollowPointer();
                                this.XLineWhite.Visibility = Visibility.Visible;
                                this.YLineWhite.Visibility = Visibility.Visible;
                                this.XLineBlack.Visibility = Visibility.Visible;
                                this.YLineBlack.Visibility = Visibility.Visible;
                                this.PointerThumb.Visibility = Visibility.Visible;
                                break;
                            default:
                                this.XLineWhite.Visibility = Visibility.Collapsed;
                                this.YLineWhite.Visibility = Visibility.Collapsed;
                                this.XLineBlack.Visibility = Visibility.Collapsed;
                                this.YLineBlack.Visibility = Visibility.Collapsed;
                                this.PointerThumb.Visibility = Visibility.Collapsed;
                                break;
                        }

                        switch (value)
                        {
                            case SimulateType.Marble:
                                this.Follow();
                                this.FollowMarble();
                                this.MarbleBorder.Visibility = Visibility.Visible;
                                break;
                            default:
                                this.MarbleBorder.Visibility = Visibility.Collapsed;
                                break;
                        }

                        base.Visibility = Visibility.Visible;
                        break;
                }

                this.type = value;
            }
        }

        Vector2 Vector;
        Point Point;

        readonly Marble MarbleX = new Marble();
        readonly Marble MarbleY = new Marble();

        //@Construct
        public SimulateCanvas()
        {
            this.InitializeComponent();
            this.ConstructPointer();
            this.ConstructMarble();
        }

        private void Follow()
        {
            this.Point.X = Window.Current.Bounds.Width / 2;
            this.Point.Y = Window.Current.Bounds.Height / 2;
            this.Vector.X = (float)this.Point.X;
            this.Vector.Y = (float)this.Point.Y;
        }

        private void ConstructPointer()
        {
            this.PointerThumb.DragStarted += (s, e) => this.Start?.Invoke(this.Vector); // Delegate
            this.PointerThumb.DragDelta += (s, e) =>
            {
                this.Vector.X += (float)e.HorizontalChange;
                this.Vector.Y += (float)e.VerticalChange;
                this.Point.X += e.HorizontalChange;
                this.Point.Y += e.VerticalChange;

                this.FollowPointer();
                this.Delta?.Invoke(this.Vector); // Delegate
            };
            this.PointerThumb.DragCompleted += (s, e) => this.Complete?.Invoke(this.Vector); // Delegate
        }
        private void FollowPointer()
        {
            this.XLineWhite.X1 = this.XLineBlack.X1 = this.Point.X - 8;
            this.XLineWhite.Y1 = this.XLineBlack.Y1 = this.Point.Y;
            this.XLineWhite.X2 = this.XLineBlack.X2 = this.Point.X + 8;
            this.XLineWhite.Y2 = this.XLineBlack.Y2 = this.Point.Y;

            this.YLineWhite.X1 = this.YLineBlack.X1 = this.Point.X;
            this.YLineWhite.Y1 = this.YLineBlack.Y1 = this.Point.Y - 8;
            this.YLineWhite.X2 = this.YLineBlack.X2 = this.Point.X;
            this.YLineWhite.Y2 = this.YLineBlack.Y2 = this.Point.Y + 8;

            Canvas.SetLeft(this.PointerThumb, this.Point.X + 40);
            Canvas.SetTop(this.PointerThumb, this.Point.Y + 40);
        }

        private void ConstructMarble()
        {
            this.MarbleBorder.ManipulationStarted += (s, e) =>
            {
                e.Handled = true;
                this.Start?.Invoke(this.Vector); // Delegate
            };
            this.MarbleBorder.ManipulationDelta += (s, e) =>
            {
                e.Handled = true;

                this.Point.X = this.MarbleX.Move(this.Point.X, e.Delta.Translation.X, e.IsInertial, 30, base.ActualWidth - 30);
                this.Point.Y = this.MarbleY.Move(this.Point.Y, e.Delta.Translation.Y, e.IsInertial, 30, base.ActualHeight - 30);
                this.Vector.X = (float)this.Point.X;
                this.Vector.Y = (float)this.Point.Y;

                this.FollowMarble();
                this.Delta?.Invoke(this.Vector); // Delegate
            };
            this.MarbleBorder.ManipulationCompleted += (s, e) =>
            {
                e.Handled = true;
                this.Complete?.Invoke(this.Vector); // Delegate
            };
        }
        private void FollowMarble()
        {
            Canvas.SetLeft(this.MarbleBorder, this.Point.X - 30);
            Canvas.SetTop(this.MarbleBorder, this.Point.Y - 30);
        }

    }
}