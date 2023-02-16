using Luo_Painter.Elements;
using Luo_Painter.HSVColorPickers;
using Luo_Painter.Layers;
using Luo_Painter.Models;
using Luo_Painter.Options;
using System;
using System.Windows.Input;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.Controls
{
    internal class LayerCommand : RelayCommand<ILayer> { }

    public sealed partial class LayerListView : XamlListView
    {
        //@Delegate
        public event EventHandler<ILayer> VisualClick { remove => this.VisualCommand.Click -= value; add => this.VisualCommand.Click += value; }
        public event EventHandler<IHistory> History;
        public event EventHandler<object> Invalidate;
        public event RoutedEventHandler OpacitySliderClick { remove => this.OpacitySlider.Click -= value; add => this.OpacitySlider.Click += value; }

        //@Command
        public ICommand Command { get; set; }

        //@Content
        public bool PasteLayerIsEnabled { get; set; }
        public bool IsOpen => this.RenameFlyout.IsOpen;
        public ImageSource Source { get; set; }

        public INumberBase OpacityNumber => this.OpacitySlider;
        public double OpacitySliderValue { set => this.OpacitySlider.Value = value; }

        //@Construct
        public LayerListView()
        {
            this.InitializeComponent();
            this.ConstructPropertys();
            this.ConstructProperty();

            this.ConstructRenames();
            this.ConstructRename();

            base.RightTapped += (s, e) =>
            {
                if (e.OriginalSource is FrameworkElement element)
                {
                    if (element.DataContext is ILayer item)
                    {
                        base.SelectedItem = item;
                        this.MenuFlyout.ShowAt(this, e.GetPosition(this));
                    }
                }
            };
            base.DoubleTapped += async (s, e) =>
            {
                await System.Threading.Tasks.Task.Delay(100);

                if (e.OriginalSource is FrameworkElement element)
                {
                    if (element.DataContext is ILayer item)
                    {
                        base.SelectedItem = item;
                        this.RenameFlyout.ShowAt(element);
                    }
                }
            };

            this.MenuFlyout.Opened += (s, e) =>
            {
                bool isEnabled = base.SelectedItem is null is false;

                this.CutLayerItem.GoToState(isEnabled);
                this.CopyLayerItem.GoToState(isEnabled);
                this.PasteLayerItem.GoToState(this.PasteLayerIsEnabled);
                this.RemoveItem.GoToState(isEnabled);
                this.DuplicateItem.GoToState(isEnabled);
                this.ExtractItem.GoToState(isEnabled);
                this.MergeItem.GoToState(isEnabled);
                this.FlattenItem.GoToState(isEnabled);
                this.GroupItem.GoToState(isEnabled);
                this.UngroupItem.GoToState(isEnabled);
                this.ReleaseItem.GoToState(isEnabled);
            };
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

    }
}