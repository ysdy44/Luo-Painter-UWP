using Luo_Painter.Layers;
using Windows.System;
using Windows.UI.Xaml;

namespace Luo_Painter.Controls
{
    public sealed partial class LayerListView : XamlListView
    {

        private void ConstructRenames()
        {
            this.RenameFlyout.Closed += (s, e) =>
            {
            };

            this.RenameFlyout.Opened += (s, e) =>
            {
                if (base.SelectedItem is ILayer layer)
                {
                    this.OKButton.IsEnabled = true;
                    this.CancelButton.IsEnabled = true;

                    this.RenameTextBox.IsEnabled = true;
                    this.RenameTextBox.Text = layer.Name ?? string.Empty;

                    this.RenameTextBox.SelectAll();
                }
                else
                {
                    this.OKButton.IsEnabled = false;
                    this.CancelButton.IsEnabled = false;

                    this.RenameTextBox.IsEnabled = false;
                    this.RenameTextBox.Text = string.Empty;
                }
            };
        }

        private void ConstructRename()
        {
            this.KeyboardAccelerator.Invoked += (s, e) =>
            {
                if (base.SelectedItem is ILayer layer)
                {
                    this.RenameFlyout.ShowAt(this.TitleTextBlock);
                }
            };

            this.RenameTextBox.KeyDown += (s, e) =>
            {
                switch (e.Key)
                {
                    case VirtualKey.Enter:
                        if (base.SelectedItem is ILayer layer)
                        {
                            layer.Name = this.RenameTextBox.Text;
                        }
                        this.RenameFlyout.Hide();
                        break;
                    case VirtualKey.Execute:
                        this.RenameFlyout.Hide();
                        break;
                    default:
                        break;
                }
            };

            this.OKButton.Click += (s, e) =>
            {
                if (base.SelectedItem is ILayer layer)
                {
                    layer.Name = this.RenameTextBox.Text;
                }
                this.RenameFlyout.Hide();
            };
            this.CancelButton.Click += (s, e) =>
            {
                this.RenameFlyout.Hide();
            };
        }

    }
}