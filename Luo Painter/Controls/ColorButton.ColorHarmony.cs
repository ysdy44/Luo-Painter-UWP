using Luo_Painter.HSVColorPickers;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Luo_Painter.Controls
{
	public sealed partial class ColorButton
	{

		private void ConstructColorHarmony()
		{
			this.HarmonyPicker.ColorChanged += (s, e) => this.EllipseSolidColorBrush.Color = e;
			this.HarmonyPicker.Color1Changed += (s, e) => this.Ellipse1SolidColorBrush.Color = e;
			this.HarmonyPicker.Color2Changed += (s, e) => this.Ellipse2SolidColorBrush.Color = e;
			this.HarmonyPicker.Color3Changed += (s, e) => this.Ellipse3SolidColorBrush.Color = e;

			this.HarmonyPicker.Recolor(Colors.DodgerBlue);
			this.HarmonyPicker.Remode(HarmonyMode.Triadic);

			this.HarmonyListView.ItemClick += (s, e) =>
			{
				if (e.ClickedItem is Rectangle item)
				{
					if (item.Fill is SolidColorBrush brush)
					{
						this.Color3(brush.Color);
                    }
				}
			};

			this.ModeListView.SelectionChanged += (s, e) =>
			{
				HarmonyMode mode = this.ModeConverter(this.ModeListView.SelectedIndex);
				this.HarmonyPicker.Remode(mode);

				this.Ellipse1Item.Visibility = mode.HasFlag(HarmonyMode.HasPoint1) ? Visibility.Visible : Visibility.Collapsed;
				this.Ellipse2Item.Visibility = mode.HasFlag(HarmonyMode.HasPoint2) ? Visibility.Visible : Visibility.Collapsed;
				this.Ellipse3Item.Visibility = mode.HasFlag(HarmonyMode.HasPoint3) ? Visibility.Visible : Visibility.Collapsed;
			};
		}

	}
}