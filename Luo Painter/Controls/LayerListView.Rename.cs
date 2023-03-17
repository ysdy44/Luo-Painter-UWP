using Luo_Painter.Layers;
using System.Linq;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Xaml;

namespace Luo_Painter.Controls
{
    public sealed partial class LayerListView
    {

        public void Rename()
        {
            if (base.SelectedItem is ILayer layer)
            {
                if (base.ContainerFromItem(layer) is FrameworkElement element)
                {
                    this.ShowRename(layer, element);
                }
            }
        }

        private void ConstructRenames()
        {
            base.DoubleTapped += async (s, e) =>
            {
                await System.Threading.Tasks.Task.Delay(100);

                if (e.OriginalSource is FrameworkElement element)
                {
                    if (element.DataContext is ILayer item)
                    {
                        if (base.SelectedItem is null)
                            base.SelectedItem = item;
                        else if (base.SelectedItems.All(c => c != item))
                            base.SelectedItem = item;

                        this.ShowRename(item, element);
                    }
                }
            };
        }

        private void ShowRename(ILayer layer, FrameworkElement element)
        {
            if (element.ActualWidth < 10) return;
            if (element.ActualHeight < 10) return;

            double padding = (element.ActualHeight - 22) / 2;
            this.RenameTextBox.Padding = new Thickness(4, padding, 4, padding);
            this.RenameTextBox.Width = element.ActualWidth - 8;
            this.RenameTextBox.Height = element.ActualHeight;

            Point transform = element.TransformToVisual(Window.Current.Content).TransformPoint(default);
            this.RenamePopup.HorizontalOffset = transform.X + 4;
            this.RenamePopup.VerticalOffset = transform.Y;
            this.RenamePopup.IsOpen = true;

            this.RenameTextBox.Text = layer.Name ?? string.Empty;
            this.RenameTextBox.SelectAll();
            this.RenameTextBox.Focus(FocusState.Keyboard);
        }

        private void ConstructRename()
        {
            this.RenameTextBox.LostFocus += (s, e) =>
            {
                this.RenamePopup.IsOpen = false;
                if (base.SelectedItem is ILayer layer)
                {
                    layer.Name = this.RenameTextBox.Text;
                }
            };

            this.RenameTextBox.KeyDown += (s, e) =>
            {
                switch (e.Key)
                {
                    case VirtualKey.Enter:
                        this.RenamePopup.IsOpen = false;
                        if (base.SelectedItem is ILayer layer)
                        {
                            layer.Name = this.RenameTextBox.Text;
                        }
                        break;
                    case VirtualKey.Execute:
                        this.RenamePopup.IsOpen = false;
                        break;
                    default:
                        break;
                }
            };
        }

    }
}