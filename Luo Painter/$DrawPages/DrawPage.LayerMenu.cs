using Luo_Painter.Layers;
using Luo_Painter.Models;
using System.Linq;
using Windows.UI.Xaml;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        private void ConstructMenus()
        {
            this.LayerListView.RightTapped += (s, e) =>
            {
                if (e.OriginalSource is FrameworkElement element)
                {
                    if (element.DataContext is ILayer item)
                    {
                        if (this.LayerSelectedItem is null)
                            this.LayerSelectedItem = item;
                        else if (this.LayerSelectedItems.All(c => c != item))
                            this.LayerSelectedItem = item;
                        this.MenuFlyout.ShowAt(this, e.GetPosition(this));
                    }
                }
            };
        }

        private void ConstructMenu()
        {
            this.MenuFlyout.Opened += (s, e) =>
            {
                bool isEnabled = this.LayerSelectedItem is null is false;

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

                TagType type = isEnabled && this.LayerSelectedItem is ILayer layer ? layer.TagType : TagType.None;
                this.TagTypeSegmented.Type = (int)type;
                this.TagTypeSegmented.IsEnabled = isEnabled;
            };
            this.TagTypeSegmented.TypeChanged += (s, e) =>
            {
                TagType type = (TagType)e;
                foreach (ILayer item in this.LayerSelectedItems.Cast<ILayer>())
                {
                    item.TagType = type;
                }
            };
        }

    }
}