using Luo_Painter.Layers;
using Luo_Painter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Luo_Painter.Controls
{
    public sealed partial class LayerListView
    {

        private void ConstructMenus()
        {
            base.RightTapped += (s, e) =>
            {
                if (e.OriginalSource is FrameworkElement element)
                {
                    if (element.DataContext is ILayer item)
                    {
                        if (base.SelectedItem is null)
                            base.SelectedItem = item;
                        else if (base.SelectedItems.All(c => c != item))
                            base.SelectedItem = item;
                        this.MenuFlyout.ShowAt(this, e.GetPosition(this));
                    }
                }
            };
        }

        private void ConstructMenu()
        {
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

                TagType type = isEnabled && base.SelectedItem is ILayer layer ? layer.TagType : TagType.None;
                this.TagTypeSegmented.Type = (int)type;
                this.TagTypeSegmented.IsEnabled = isEnabled;
            };
            this.TagTypeSegmented.TypeChanged += (s, e) =>
            {
                TagType type = (TagType)e;
                foreach (ILayer item in base.SelectedItems.Cast<ILayer>())
                {
                    item.TagType = type;
                }
            };
        }

    }
}