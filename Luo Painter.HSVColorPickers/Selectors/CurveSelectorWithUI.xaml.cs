using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Luo_Painter.HSVColorPickers
{
    public partial class CurveSelectorWithUI : CurveSelector
    {
        public CurveSelectorWithUI()
        {
            this.InitializeComponent();
        }
    }

    public class CurveSelector : ButtonSelector<CurveStop>
    {
        //@Delegate
        public event EventHandler<double> ItemRemoved;
        public event EventHandler<object> Invalidate;

        //@Content
        public bool IsItemClickEnabled { get; private set; } = true;
        public int SelectedIndex { get; private set; } = -1;

        public IEnumerable<CurveStop> Source => this.Stops;
        protected readonly ObservableCollection<CurveStop> Stops = new ObservableCollection<CurveStop>();

        public float[] Data { get; private set; }
        private IEnumerable<float> CreateData() => from item in this.Source select item.Offset;

        Point StatringPosition;

        //@Construct
        public CurveSelector()
        {
            base.ItemSource = this.Stops;

            base.ItemManipulationStarted += (s, e) =>
            {
                UIElement button = e.Container;

                this.StatringPosition = new Point
                {
                    X = Canvas.GetLeft(button) + e.Cumulative.Translation.X + 16,
                    Y = Canvas.GetTop(button) + e.Cumulative.Translation.Y + 16,
                };

                this.SelectedIndex = -1;
                foreach (var item in base.Items)
                {
                    if (item.Value == button)
                    {
                        this.SelectedIndex = this.Stops.IndexOf(item.Key);
                        this.Invalidate?.Invoke(this, null); // Delegate
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
                double y = this.StatringPosition.Y + e.Cumulative.Translation.Y;
                bool isRemove = false;
                if (base.Count > 2)
                {
                    isRemove = y < -50;

                    if (isRemove)
                    {
                        base.IsHitTestVisible = false;
                    }
                    else
                    {
                        base.IsHitTestVisible = true;
                    }
                }

                double height = base.ActualHeight;
                double offsetY = Math.Clamp(y / height, 0, 1);
                if (isRemove)
                    Canvas.SetTop(button, y);
                else
                    Canvas.SetTop(button, offsetY * height - 16);

                this.Stops[this.SelectedIndex].Offset = (float)offsetY;
                this.Data[this.SelectedIndex] = (float)offsetY;
                this.Invalidate?.Invoke(this, null); // Delegate
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
                        float offset = this.Stops[this.SelectedIndex].Offset;
                        this.RemoveCurrent();
                        foreach (var item in base.Items)
                        {
                            Canvas.SetLeft(item.Value, this.GetItemLeft(item.Key));
                        }
                        this.ItemRemoved?.Invoke(this, offset); // Delegate
                    }
                }

                this.Invalidate?.Invoke(this, null); // Delegate
                e.Handled = true;
                this.ItemClickEnabled();
            };
            base.ItemPreviewKeyDown += (s, e) =>
            {
                if (this.SelectedIndex < 0) return;
                if (this.SelectedIndex >= base.Count) return;

                CurveStop key = this.Stops[this.SelectedIndex];

                switch (e.Key)
                {
                    case VirtualKey.Delete:
                        if (base.Count > 2)
                        {
                            this.RemoveCurrent();
                            foreach (var item in base.Items)
                            {
                                Canvas.SetLeft(item.Value, this.GetItemLeft(item.Key));
                            }
                            this.ItemRemoved?.Invoke(this, key.Offset); // Delegate
                            this.Invalidate?.Invoke(this, null); // Delegate
                            e.Handled = true;
                        }
                        break;
                    case VirtualKey.Down:
                        {
                            float offsetX = key.Offset;
                            offsetX = (float)Math.Clamp(offsetX - 0.01, 0, 1);

                            key.Offset = offsetX;
                            this.Data[this.SelectedIndex] = (float)offsetX;

                            double width = base.ActualWidth;
                            Canvas.SetLeft(base.Items[key], offsetX * width - 16);
                            this.Invalidate?.Invoke(this, null); // Delegate
                            e.Handled = true;
                        }
                        break;
                    case VirtualKey.Up:
                        {
                            float offsetX = key.Offset;
                            offsetX = (float)Math.Clamp(offsetX + 0.01, 0, 1);

                            key.Offset = offsetX;
                            this.Data[this.SelectedIndex] = (float)offsetX;

                            double width = base.ActualWidth;
                            Canvas.SetLeft(base.Items[key], offsetX * width - 16);
                            this.Invalidate?.Invoke(this, null); // Delegate
                            e.Handled = true;
                        }
                        break;
                    default:
                        break;
                }
            };

            base.ManipulationMode = ManipulationModes.TranslateY;
            base.ManipulationStarted += (s, e) =>
            {
                Point point = e.Position;
                bool result = this.Interpolation(point);
                if (result is false) return;

                this.SetCurrent(point);
                this.Invalidate?.Invoke(this, null); // Delegate
            };
            base.ManipulationDelta += (s, e) =>
            {
                this.SetCurrentOffset(e.Position);
                this.Invalidate?.Invoke(this, null); // Delegate
            };
            base.ManipulationCompleted += (s, e) =>
            {
                this.SetCurrentOffset(e.Position);
                this.Invalidate?.Invoke(this, null); // Delegate
            };
        }

        private async void ItemClickEnabled()
        {
            await Task.Delay(300);
            this.IsItemClickEnabled = true;
        }

        //@Override
        public override double GetItemLeft(CurveStop key) => base.ActualWidth * this.Stops.IndexOf(key) / (this.Stops.Count - 1) - 16;
        public override double GetItemTop(CurveStop key) => key.Offset * base.ActualHeight - 16;

        public void Add(float offset)
        {
            this.Stops.Add(new CurveStop { Offset = offset });
            this.Data = this.CreateData().ToArray();
        }
        public bool Interpolation(Point point)
        {
            double x = point.X;
            double width = base.ActualWidth;
            double offsetX = Math.Clamp(x / width, 0, 1); 

            double y = point.Y;
            double height = base.ActualHeight;
            double offsetY = Math.Clamp(y / height, 0, 1); 

            int index = (int)(offsetX * this.Stops.Count);

            this.Stops.Insert(index, new CurveStop { Offset = (float)offsetY });
            this.Data = this.CreateData().ToArray();

            this.SelectedIndex = index;

            foreach (var item in base.Items)
            {
                Canvas.SetLeft(item.Value, this.GetItemLeft(item.Key));
            }
            return true;
        }
        public void Reset(IEnumerable<float> stops)
        {
            this.Stops.Clear();
            foreach (float item in stops)
            {
                float offsetX = item;
                this.Stops.Add(new CurveStop { Offset = offsetX });
            }
            this.Data = this.CreateData().ToArray();
        }

        public void RemoveCurrent()
        {
            if (this.SelectedIndex < 0) return;
            if (this.SelectedIndex >= base.Count) return;

            this.Stops.RemoveAt(this.SelectedIndex);
            this.Data = this.CreateData().ToArray();
        }

        public void SetCurrentOffset(Point point)
        {
            if (this.SelectedIndex < 0) return;
            if (this.SelectedIndex >= base.Count) return;

            double y = point.Y;
            double height = base.ActualHeight;
            float offsetY = (float)Math.Clamp(y / height, 0, 1); 

            this.Stops[this.SelectedIndex].Offset = offsetY;
            this.Data[this.SelectedIndex] = offsetY;
         
            CurveStop key = this.Stops[this.SelectedIndex];
            Canvas.SetTop(base.Items[key], offsetY * height - 16);
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