using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Elements
{
    partial class Expander
    {

        private void Close() => base.Visibility = Visibility.Collapsed;
        private void Open() => base.Visibility = Visibility.Visible;
        private void OpenWithResize()
        {
            base.Opacity = 0;
            base.Visibility = Visibility.Visible;
            base.Opacity = 1;
        }

        private void SetLeft(double value) => Canvas.SetLeft(this, this.W >= this.U ? 0 : System.Math.Clamp(value, 0, this.U - this.W));
        private void SetTop(double value) => Canvas.SetTop(this, this.H >= this.V ? 0 : System.Math.Clamp(value, 0, this.V - this.H));
        private void SetTopWithHeader(double value) => Canvas.SetTop(this, System.Math.Clamp(value, 0, this.V - 50));

        private bool InsideLeft() => this.PlacementTargetPosition.X > this.W;
        private bool InsideTop() => true; // this.PlacementTargetPosition.Y > this.H;
        private bool InsideRight() => this.U - this.PlacementTargetPosition.X - this.PlacementTargetW > this.W;
        private bool InsideBottom() => true; // this.V - this.PlacementTargetPosition.Y - this.PlacementTargetH > this.H;
        private ExpanderPlacementMode GetPlacement(ExpanderPlacementMode placement)
        {
            switch (placement)
            {
                case ExpanderPlacementMode.Center: return ExpanderPlacementMode.Center;

                case ExpanderPlacementMode.Left:
                case ExpanderPlacementMode.Right:
                    {
                        switch (placement)
                        {
                            case ExpanderPlacementMode.Left:
                                if (this.InsideLeft()) return ExpanderPlacementMode.Left;
                                else if (this.InsideRight()) return ExpanderPlacementMode.Right;
                                else break;
                            case ExpanderPlacementMode.Right:
                                if (this.InsideRight()) return ExpanderPlacementMode.Right;
                                else if (this.InsideLeft()) return ExpanderPlacementMode.Left;
                                else break;
                            default:
                                return ExpanderPlacementMode.Center;
                        }

                        if (this.InsideBottom()) return ExpanderPlacementMode.Bottom;
                        else if (this.InsideTop()) return ExpanderPlacementMode.Top;
                        else return ExpanderPlacementMode.Center;
                    }

                case ExpanderPlacementMode.Top:
                case ExpanderPlacementMode.Bottom:
                    {
                        switch (placement)
                        {
                            case ExpanderPlacementMode.Top:
                                if (this.InsideTop()) return ExpanderPlacementMode.Top;
                                else if (this.InsideBottom()) return ExpanderPlacementMode.Bottom;
                                else break;
                            case ExpanderPlacementMode.Bottom:
                                if (this.InsideBottom()) return ExpanderPlacementMode.Bottom;
                                else if (this.InsideTop()) return ExpanderPlacementMode.Top;
                                else break;
                            default:
                                return ExpanderPlacementMode.Center;
                        }

                        switch (base.FlowDirection)
                        {
                            case FlowDirection.LeftToRight:
                                if (this.InsideRight()) return ExpanderPlacementMode.Right;
                                else if (this.InsideLeft()) return ExpanderPlacementMode.Left;
                                else return ExpanderPlacementMode.Center;
                            case FlowDirection.RightToLeft:
                                if (this.InsideLeft()) return ExpanderPlacementMode.Left;
                                else if (this.InsideRight()) return ExpanderPlacementMode.Right;
                                else return ExpanderPlacementMode.Center;
                            default:
                                return ExpanderPlacementMode.Center;
                        }
                    }

                default:
                    return ExpanderPlacementMode.Center;
            }
        }

    }
}