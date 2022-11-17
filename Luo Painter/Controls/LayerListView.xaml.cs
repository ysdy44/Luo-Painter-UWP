using Luo_Painter.Elements;
using Luo_Painter.Historys;
using Luo_Painter.Layers;
using Luo_Painter.Options;
using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.Controls
{
    internal class LayerCommand : RelayCommand<ILayer> { }

    public sealed partial class LayerListView : XamlListView
    {
        //@Delegate
        public event EventHandler<ILayer> VisualClick
        {
            remove => this.VisualCommand.Click -= value;
            add => this.VisualCommand.Click += value;
        }
        public event EventHandler<OptionType> ItemClick2
        {
            remove => this.Command.Click -= value;
            add => this.Command.Click += value;
        }
        public event EventHandler<IHistory> History;
        public event EventHandler<object> Invalidate;

        //@Content
        public bool PasteLayerIsEnabled { get; set; }
        public bool IsOpen => this.Flyout.IsOpen;
        public ImageSource Source { set => this.MarqueeImage.Source = value; }

        //@Construct
        public LayerListView()
        {
            this.InitializeComponent();
            this.ConstructPropertys();
            this.ConstructProperty();
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
            base.DoubleTapped += (s, e) =>
            {
                if (e.OriginalSource is FrameworkElement element)
                {
                    if (element.DataContext is ILayer item)
                    {
                        base.SelectedItem = item;
                        this.Flyout.ShowAt(element);
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