using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Foundation;
using Windows.UI;

namespace Luo_Painter.Elements
{
    public class GradientStopSelectorWithUI : GradientStopSelector
    {
        public Button CurrentButton { get; private set; }
        public GradientStop CurrentStop { get; private set; }
        public GradientStop CurrentStopUI { get; private set; }

        public IEnumerable<GradientStop> Source => this.Stops;
        readonly ObservableCollection<GradientStop> Stops = new ObservableCollection<GradientStop>();
        readonly GradientStopCollection StopsUI = new GradientStopCollection();

        public GradientStopSelectorWithUI()
        {
            base.ItemSource = this.Stops;
            base.Background = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 0),
                GradientStops = this.StopsUI
            };
            base.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                foreach (var item in base.Items)
                {
                    Canvas.SetLeft(item.Value, base.GetItemLeft(item.Key));
                    Canvas.SetTop(item.Value, base.GetItemTop(item.Key));
                }
            };


            base.ItemManipulationStarted += (s, e) =>
            {
                UIElement button = e.Container;

                button.RenderTransformOrigin = new Point
                {
                    X = Canvas.GetLeft(button) + e.Cumulative.Translation.X + base.Margin.Left,
                    Y = Canvas.GetTop(button) + e.Cumulative.Translation.Y + base.Margin.Top,
                };

                int index = -1;
                foreach (var item in base.Items)
                {
                    index++;
                    if (item.Value == button)
                    {
                        Canvas.SetZIndex(button, index);
                        break;
                    }
                }
            };
            base.ItemManipulationDelta += (s, e) =>
            {
                UIElement button = e.Container;

                // Remove
                if (base.Count > 2)
                {
                    double staringY = button.RenderTransformOrigin.Y;
                    double y = staringY + e.Cumulative.Translation.Y;
                    bool isRemove = y < -50;

                    if (isRemove)
                    {
                        Canvas.SetTop(button, y - base.Margin.Top);
                        base.IsHitTestVisible = false;
                        return;
                    }
                    else
                    {
                        Canvas.SetTop(button, -base.Margin.Top);
                        base.IsHitTestVisible = true;
                    }
                }

                double staringX = button.RenderTransformOrigin.X;
                double x = staringX + e.Cumulative.Translation.X;
                double width = base.ActualWidth;
                double offsetX = Math.Clamp(x / width, 0, 1);
                Canvas.SetLeft(button, offsetX * width - base.Margin.Left);

                int index = Canvas.GetZIndex(button);
                this.Stops[index].Offset = offsetX;
                this.StopsUI[index].Offset = offsetX;
            };
            base.ItemManipulationCompleted += (s, e) =>
            {
                UIElement button = e.Container;

                // Remove
                base.IsHitTestVisible = true;
                if (base.Count > 2)
                {
                    double staringY = button.RenderTransformOrigin.Y;
                    double y = staringY + e.Cumulative.Translation.Y;
                    double height = base.ActualHeight;
                    bool isRemove = y < -height;

                    if (isRemove)
                    {
                        this.SetCurrent(button);
                        this.RemoveCurrent();
                    }
                }

                button.RenderTransformOrigin = new Point();
                Canvas.SetZIndex(button, 0);
            };
        }

        public void Add(Color color, double offset)
        {
            this.Stops.Add(new GradientStop { Color = color, Offset = offset });
            this.StopsUI.Add(new GradientStop { Color = color, Offset = offset });
        }
        public bool Interpolation(Point point)
        {
            double x = point.X;
            double width = base.ActualWidth;
            double offsetX = Math.Clamp(x / width, 0, 1); // 0.5

            //double y = point.Y;
            //double height = base.ActualHeight;
            //double offsetY = Math.Clamp(y / height, 0, 1); // 0.5

            GradientStop left = null; // 0
            GradientStop right = null; // 1
            foreach (GradientStop item in this.Stops)
            {
                double offset = item.Offset;
                if (offset < offsetX)
                {
                    if (left is null) left = item;
                    else if (item.Offset < left.Offset) continue;
                    else left = item;
                }
                else if (offset > offsetX)
                {
                    if (right is null) right = item;
                    else if (item.Offset > left.Offset) continue;
                    else right = item;
                }
                else return false;
            }

            if (left is null) return false;
            if (right is null) return false;

            double length = Math.Abs(right.Offset - left.Offset);
            double min = Math.Abs(left.Offset - offsetX) / length;
            double max = Math.Abs(right.Offset - offsetX) / length;
            Color color = new Color
            {
                A = 255,
                R = (byte)(left.Color.R * min + right.Color.R * max),
                G = (byte)(left.Color.G * min + right.Color.G * max),
                B = (byte)(left.Color.B * min + right.Color.B * max),
            };

            this.Stops.Add(new GradientStop { Color = color, Offset = offsetX });
            this.StopsUI.Add(new GradientStop { Color = color, Offset = offsetX });
            return true;
        }
        public void Reset(IDictionary<double, Color> stops)
        {
            this.Stops.Clear();
            this.StopsUI.Clear();
            foreach (var item in stops)
            {
                double offsetX = item.Key;
                Color color = item.Value;
                this.Stops.Add(new GradientStop { Color = color, Offset = offsetX });
                this.StopsUI.Add(new GradientStop { Color = color, Offset = offsetX });
            }
        }

        public void RemoveCurrent()
        {
            this.CurrentButton = null;

            if (this.CurrentStop != null)
            {
                this.Stops.Remove(this.CurrentStop);
                this.CurrentStop = null;
            }

            if (this.CurrentStopUI != null)
            {
                this.StopsUI.Remove(this.CurrentStopUI);
                this.CurrentStopUI = null;
            }
        }

        public void SetCurrentColor(Color color)
        {
            if (this.CurrentStop == null) return;
            this.CurrentStop.Color = color;

            if (this.CurrentStopUI == null) return;
            this.CurrentStopUI.Color = color;
        }
        public void SetCurrentOffset(Point point)
        {
            double x = point.X;
            double width = base.ActualWidth;
            double offsetX = Math.Clamp(x / width, 0, 1); // 0.5

            if (this.CurrentButton == null) return;
            Canvas.SetLeft(this.CurrentButton, offsetX * width - base.Margin.Left);

            if (this.CurrentStop == null) return;
            this.CurrentStop.Offset = offsetX;

            if (this.CurrentStopUI == null) return;
            this.CurrentStopUI.Offset = offsetX;
        }

        public void SetCurrent(Point point, double distance = 20)
        {
            double width = base.ActualWidth;

            this.CurrentButton = null;
            this.CurrentStop = null;
            foreach (var item in base.Items)
            {
                double offset = item.Key.Offset;
                double x = offset * width;

                if (Math.Abs(x - point.X) < distance)
                {
                    this.CurrentButton = item.Value;
                    this.CurrentStop = item.Key;
                }
            }
            if (this.CurrentStop == null) return;

            foreach (GradientStop item in this.StopsUI)
            {
                if (item.Offset == this.CurrentStop.Offset)
                    this.CurrentStopUI = item;
            }
        }
        public void SetCurrent(object sender)
        {
            this.CurrentButton = sender as Button;
            if (this.CurrentButton == null) return;

            this.CurrentStop = this.CurrentButton.Content as GradientStop;
            if (this.CurrentStop == null) return;

            foreach (GradientStop item in this.StopsUI)
            {
                if (item.Offset == this.CurrentStop.Offset)
                    this.CurrentStopUI = item;
            }
        }

    }
}