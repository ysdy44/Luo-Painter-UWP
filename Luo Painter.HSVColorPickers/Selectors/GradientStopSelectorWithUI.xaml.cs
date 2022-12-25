using Microsoft.Graphics.Canvas.Brushes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.HSVColorPickers
{
    public sealed partial class GradientStopSelectorWithUI : GradientStopSelector, IColorBase
    {
        //@Content
        public FrameworkElement PlacementTarget
        {
            get
            {
                if (this.SelectedIndex < 0) return this;
                if (this.SelectedIndex >= base.Count) return this;

                GradientStop key = base.Stops[base.SelectedIndex];
                return base.Items[key];
            }
        }
        public Color Color
        {
            get
            {
                if (this.SelectedIndex < 0) return default;
                if (this.SelectedIndex >= base.Count) return default;

                GradientStop key = base.Stops[base.SelectedIndex];
                return key.Color;
            }
        }

        public GradientStopSelectorWithUI()
        {
            this.InitializeComponent();
        }

        public void SetColor(Color color) => base.SetCurrentColor(color);
        public void SetColor(Vector4 colorHdr) => this.SetColor(Color.FromArgb((byte)(colorHdr.W * 255f), (byte)(colorHdr.X * 255f), (byte)(colorHdr.Y * 255f), (byte)(colorHdr.Z * 255f)));
    }

    public class GradientStopSelector : ButtonSelector<GradientStop>
    {
        //@Delegate
        public event EventHandler<double> ItemRemoved;

        //@Content
        public bool IsItemClickEnabled { get; private set; } = true;
        public int SelectedIndex { get; private set; } = -1;

        public IEnumerable<GradientStop> Source => this.Stops;
        protected readonly ObservableCollection<GradientStop> Stops = new ObservableCollection<GradientStop>();
        readonly GradientStopCollection StopsUI = new GradientStopCollection();

        public CanvasGradientStop[] Data { get; private set; }
        private IEnumerable<CanvasGradientStop> CreateData() => from item in this.Source select new CanvasGradientStop
        {
            Position = (float)item.Offset,
            Color = item.Color,
        };

        Point StatringPosition;

        //@Construct
        public GradientStopSelector()
        {
            base.ItemSource = this.Stops;
            base.Background = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 0),
                GradientStops = this.StopsUI
            };

            base.ItemManipulationStarted += (s, e) =>
            {
                UIElement button = e.Container;

                this.StatringPosition = new Point
                {
                    X = Canvas.GetLeft(button) + e.Cumulative.Translation.X + 25,
                    Y = Canvas.GetTop(button) + e.Cumulative.Translation.Y + 0,
                };

                this.SelectedIndex = -1;
                foreach (var item in base.Items)
                {
                    if (item.Value == button)
                    {
                        this.SelectedIndex = this.Stops.IndexOf(item.Key);
                        break;
                    }
                }
                e.Handled = true;
                this.IsItemClickEnabled = false;
            };
            base.ItemManipulationDelta += (s, e) =>
            {
                if (this.SelectedIndex < 0) return;
                if (this.SelectedIndex >= base.Count) return;

                UIElement button = e.Container;

                // Remove
                if (base.Count > 2)
                {
                    double y = this.StatringPosition.Y + e.Cumulative.Translation.Y;
                    bool isRemove = y < -50;

                    if (isRemove)
                    {
                        Canvas.SetTop(button, y);
                        base.IsHitTestVisible = false;
                    }
                    else
                    {
                        Canvas.SetTop(button, 0);
                        base.IsHitTestVisible = true;
                    }
                }

                double x = this.StatringPosition.X + e.Cumulative.Translation.X;
                double width = base.ActualWidth;
                double offsetX = Math.Clamp(x / width, 0, 1);
                Canvas.SetLeft(button, offsetX * width - 25);

                this.Stops[this.SelectedIndex].Offset = offsetX;
                this.StopsUI[this.SelectedIndex].Offset = offsetX;
                this.Data[this.SelectedIndex].Position = (float)offsetX;
                e.Handled = true;
            };
            base.ItemManipulationCompleted += (s, e) =>
            {
                if (this.SelectedIndex < 0) return;
                if (this.SelectedIndex >= base.Count) return;

                UIElement button = e.Container;

                // Remove
                base.IsHitTestVisible = true;
                if (base.Count > 2)
                {
                    double y = this.StatringPosition.Y + e.Cumulative.Translation.Y;
                    bool isRemove = y < -50;

                    if (isRemove)
                    {
                        this.SetCurrent(button);
                        this.ItemRemoved?.Invoke(this, this.Stops[this.SelectedIndex].Offset); // Delegate
                        this.RemoveCurrent();
                    }
                }

                e.Handled = true;
                this.ItemClickEnabled();
            };
            base.ItemPreviewKeyDown += (s, e) =>
            {
                if (this.SelectedIndex < 0) return;
                if (this.SelectedIndex >= base.Count) return;

                GradientStop key = this.Stops[this.SelectedIndex];

                switch (e.Key)
                {
                    case VirtualKey.Delete:
                        if (base.Count > 2)
                        {
                            this.ItemRemoved?.Invoke(this, key.Offset); // Delegate
                            this.RemoveCurrent();
                            e.Handled = true;
                        }
                        break;
                    case VirtualKey.Left:
                        {
                            double offsetX = key.Offset;
                            offsetX = Math.Clamp(offsetX - 0.01, 0, 1);

                            key.Offset = offsetX;
                            this.StopsUI[this.SelectedIndex].Offset = offsetX;
                            this.Data[this.SelectedIndex].Position = (float)offsetX;

                            double width = base.ActualWidth;
                            Canvas.SetLeft(base.Items[key], offsetX * width - 25);
                            e.Handled = true;
                        }
                        break;
                    case VirtualKey.Right:
                        {
                            double offsetX = key.Offset;
                            offsetX = Math.Clamp(offsetX + 0.01, 0, 1);

                            key.Offset = offsetX;
                            this.StopsUI[this.SelectedIndex].Offset = offsetX;
                            this.Data[this.SelectedIndex].Position = (float)offsetX;

                            double width = base.ActualWidth;
                            Canvas.SetLeft(base.Items[key], offsetX * width - 25);
                            e.Handled = true;
                        }
                        break;
                    default:
                        break;
                }

            };
        }

        private async void ItemClickEnabled()
        {
            await Task.Delay(300);
            this.IsItemClickEnabled = true;
        }

        //@Override
        public override double GetItemLeft(GradientStop key) => key.Offset * base.ActualWidth - 25;
        public override double GetItemTop(GradientStop key) => 0;

        public void Add(Color color, double offset)
        {
            this.Stops.Add(new GradientStop { Color = color, Offset = offset });
            this.StopsUI.Add(new GradientStop { Color = color, Offset = offset });
            this.Data = this.CreateData().ToArray();
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
                    else if (item.Offset > right.Offset) continue;
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
            this.Data = this.CreateData().ToArray();
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
            this.Data = this.CreateData().ToArray();
        }

        public void RemoveCurrent()
        {
            if (this.SelectedIndex < 0) return;
            if (this.SelectedIndex >= base.Count) return;

            this.Stops.RemoveAt(this.SelectedIndex);
            this.StopsUI.RemoveAt(this.SelectedIndex);
            this.Data = this.CreateData().ToArray();
        }

        public void SetCurrentColor(Color color)
        {
            if (this.SelectedIndex < 0) return;
            if (this.SelectedIndex >= base.Count) return;

            this.Stops[this.SelectedIndex].Color = color;
            this.StopsUI[this.SelectedIndex].Color = color;
            this.Data[this.SelectedIndex].Color = color;
        }
        public void SetCurrentOffset(Point point)
        {
            double x = point.X;
            double width = base.ActualWidth;
            double offsetX = Math.Clamp(x / width, 0, 1); // 0.5

            this.Stops[this.SelectedIndex].Offset = offsetX;
            this.StopsUI[this.SelectedIndex].Offset = offsetX;
            this.Data[this.SelectedIndex].Position = (float)offsetX;

            GradientStop key = this.Stops[this.SelectedIndex];
            Canvas.SetLeft(base.Items[key], offsetX * width - base.Margin.Left);
        }

        public void SetCurrent(Point point, double distance = 20)
        {
            double width = base.ActualWidth;

            this.SelectedIndex = -1;
            foreach (var item in base.Items)
            {
                double offset = item.Key.Offset;
                double x = offset * width;

                if (Math.Abs(x - point.X) < distance)
                {
                    this.SelectedIndex = this.Stops.IndexOf(item.Key);
                    item.Value.Focus(FocusState.Keyboard);
                    break;
                }
            }
        }
        public void SetCurrent(object sender)
        {
            if (sender is Button button)
            {
                foreach (var item in base.Items)
                {
                    if (item.Value == button)
                    {
                        this.SelectedIndex = this.Stops.IndexOf(item.Key);
                        item.Value.Focus(FocusState.Keyboard);
                    }
                }
            }
        }

    }
}