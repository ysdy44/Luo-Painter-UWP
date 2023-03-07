using Luo_Painter.Blends;
using Luo_Painter.Elements;
using Luo_Painter.Models;
using Luo_Painter.Options;
using Microsoft.Graphics.Canvas.Effects;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    //@Attached
    // ResourceLoaderExtensions
    public static class Res
    {
        // UIText
        public static UIType GetUT(TextBlock dp) => default;
        public static void SetUT(TextBlock dp, UIType value) => dp.Text = App.Resource.GetString(value.ToString());

        // UIHeader
        public static UIType GetUH(Slider dp) => default;
        public static void SetUH(Slider dp, UIType value) => dp.Header = App.Resource.GetString(value.ToString());

        // UIContent
        public static UIType GetUC(ContentControl dp) => dp.Content is UIType value ? value : default;
        public static void SetUC(ContentControl dp, UIType value) => dp.Content = App.Resource.GetString(value.ToString());

        // UIDialog
        public static UIType GetUD(ContentDialog dp) => default;
        public static void SetUD(ContentDialog dp, UIType value)
        {
            dp.Title = App.Resource.GetString(value.ToString());
            dp.PrimaryButtonText = App.Resource.GetString(UIType.OK.ToString());
            dp.SecondaryButtonText = App.Resource.GetString(UIType.Cancel.ToString());
        }

        // UIDocker
        public static UIType GetUK(IDocker dp) => default;
        public static void SetUK(IDocker dp, UIType value)
        {
            dp.Title = App.Resource.GetString(value.ToString());
            dp.Subtitle = App.Resource.GetString($"{value}_Subtitle");
            dp.PrimaryButtonText = App.Resource.GetString(UIType.OK.ToString());
            dp.SecondaryButtonText = App.Resource.GetString(UIType.Cancel.ToString());
        }

        // UIToolTip
        public static UIType GetUTT(DependencyObject dp) => default;
        public static void SetUTT(DependencyObject dp, UIType value)
        {
            ToolTipService.SetToolTip(dp, new ToolTip
            {
                Content = App.Resource.GetString(value.ToString()),
                Style = App.Current.Resources["AppToolTipStyle"] as Style
            });
        }


        // OptionText
        public static OptionType GetOT(TextBlock dp) => default;
        public static void SetOT(TextBlock dp, OptionType value) => dp.Text = App.Resource.GetString(value.ToString());

        // OptionContent
        public static OptionType GetOC(ContentControl dp) => dp.Content is OptionType value ? value : default;
        public static void SetOC(ContentControl dp, OptionType value) => dp.Content = App.Resource.GetString(value.ToString());

        // OptionAccessKey
        public static OptionType GetOAK(MenuFlyoutSeparator dp) => default;
        public static void SetOAK(MenuFlyoutSeparator dp, OptionType value) => dp.AccessKey = App.Resource.GetString(value.ToString());

        // OptionToolTip
        public static OptionType GetOTT(DependencyObject dp) => default;
        public static void SetOTT(DependencyObject dp, OptionType value)
        {
            ToolTipService.SetToolTip(dp, new ToolTip
            {
                Content = App.Resource.GetString(value.ToString()),
                Style = App.Current.Resources["AppToolTipStyle"] as Style
            });
        }

        // OptionIcon
        public static OptionType GetOI(ContentControl dp) => dp.Content is OptionType value ? value : default;
        public static void SetOI(ContentControl dp, OptionType value)
        {
            dp.Resources.Source = new Uri(value.GetResource());
            dp.Content = new ContentControl
            {
                Content = App.Resource.GetString(value.ToString()),
                Template = value.GetTemplate(dp.Resources)
            };
        }

        // OptionIconWithToolTip
        public static OptionType GetOIWTT(ContentControl dp) => Res.GetOI(dp);
        public static void SetOIWTT(ContentControl dp, OptionType value)
        {
            Res.SetOI(dp, value);
            ToolTipService.SetToolTip(dp, new ToolTip
            {
                Content = App.Resource.GetString(value.ToString()),
                Style = App.Current.Resources["AppToolTipStyle"] as Style
            });
        }


        // ElementIcon
        public static ElementType GetEI(ContentControl dp) => dp.Content is ElementType value ? value : default;
        public static void SetEI(ContentControl dp, ElementType value)
        {
            dp.Resources.Source = new Uri(value.GetResource());
            dp.Content = new ContentControl
            {
                Content = value,
                Template = value.GetTemplate(dp.Resources)
            };
        }

        // ElementIconWithToolTip
        public static ElementType GetEIWTT(ContentControl dp) => Res.GetEI(dp);
        public static void SetEIWTT(ContentControl dp, ElementType value)
        {
            Res.SetEI(dp, value);
            ToolTipService.SetToolTip(dp, new ToolTip
            {
                Content = App.Resource.GetString(value.ToString()),
                Style = App.Current.Resources["AppToolTipStyle"] as Style
            });
        }

        // ElementItem
        public static ElementType GetEE(ContentControl dp) => Res.GetEI(dp);
        public static void SetEE(ContentControl dp, ElementType value)
        {
            dp.Resources.Source = new Uri(value.GetResource());
            dp.Content = CreateItem(value, App.Resource.GetString(value.ToString()), value.GetTemplate(dp.Resources));
        }


        // BlendItem
        public static BlendEffectMode GetBE(ContentControl dp) => dp.Content is BlendEffectMode value ? value : default;
        public static void SetBE(ContentControl dp, BlendEffectMode value)
        {
            dp.Content = CreateItem(value.GetIcon(), App.Resource.GetString($"Blends_{value.GetTitle()}"));
        }


        public static Grid CreateItem(string tag, string text) => new Grid
        {
            Children =
            {
                new TextBlock
                {
                    Text = text,
                    VerticalAlignment = VerticalAlignment.Center,
                    TextTrimming = TextTrimming.CharacterEllipsis,
                },
                new Border
                {
                    Width = 32,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Child = new TextBlock
                    {
                        Text = tag,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    }
                }
            }
        };
        public static Grid CreateItem(object value, string text, ControlTemplate icon) => new Grid
        {
            Children =
            {
                new TextBlock
                {
                    Text = text,
                    VerticalAlignment = VerticalAlignment.Center,
                    TextTrimming = TextTrimming.CharacterEllipsis,
                },
                new ContentControl
                {
                    Content = value,
                    Width = 32,
                    Template = icon,
                    FontWeight = Windows.UI.Text.FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                }
            }
        };
    }
}