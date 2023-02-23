using Luo_Painter.Blends;
using Luo_Painter.Models;
using Luo_Painter.Options;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Luo_Painter
{
    //@Attached
    // CommandParameterExtensions
    public static class Cmd
    {
        // ButtonBase
        public static OptionType GetBB(ButtonBase dp) => dp.CommandParameter is OptionType value ? value : default;
        public static void SetBB(ButtonBase dp, OptionType value) => dp.CommandParameter = value;

        // MenuFlyoutItem 
        public static OptionType GetMFI(MenuFlyoutItem dp) => dp.CommandParameter is OptionType value ? value : default;
        public static void SetMFI(MenuFlyoutItem dp, OptionType value)
        {
            if (value.IsItemClickEnabled())
            {
                dp.Text = App.Resource.GetString(value.ToString());
                dp.CommandParameter = value;
            }
        }

        // MenuFlyoutItemWidthIcon
        public static OptionType GetMFIWI(MenuFlyoutItem dp) => Cmd.GetMFI(dp);
        public static void SetMFIWI(MenuFlyoutItem dp, OptionType value)
        {
            Cmd.SetMFI(dp, value);

            if (value.ExistIcon())
            {
                dp.Resources.Source = new Uri(value.GetResource());
                dp.Tag = new ContentControl
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Template = value.GetTemplate(dp.Resources)
                };
            }

            if (value.HasPreview())
            {
                dp.KeyboardAcceleratorTextOverride = "•";
            }
            else if (value.HasMenu())
            {
                dp.KeyboardAcceleratorTextOverride = ">";
            }
        }

        // MenuFlyoutSubItem
        public static OptionType GetMFS(MenuFlyoutSubItem dp) => default;
        public static void SetMFS(MenuFlyoutSubItem dp, OptionType value) => dp.Text = App.Resource.GetString(value.ToString());
    }
}