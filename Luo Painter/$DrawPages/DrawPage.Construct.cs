﻿using Luo_Painter.Elements;
using Luo_Painter.Options;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page
    {

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }


        private void ConstructDialog()
        {
            this.SettingButton.Click += async (s, e) =>
            {
                await this.SettingDislog.ShowInstance();
            };

            this.ExportButton.ExportClick += async (s, e) =>
            {
                this.Tip("Saving...", this.ApplicationView.Title); // Tip

                bool? result = await this.Export();
                if (result == null) return;

                if (result.Value)
                    this.Tip("Saved successfully", this.ApplicationView.Title); // Tip
                else
                    this.Tip("Failed to Save", "Try again?"); // Tip
            };
        }


        public void Tip(string title, string subtitle)
        {
            this.ToastTip.Tip(title, subtitle);
        }

        private void ConstructColor()
        {
            this.ColorButton.ColorChanged += (s, e) =>
            {
                switch (this.OptionType)
                {
                    case OptionType.GradientMapping:
                        this.GradientMappingColorChanged(e.NewColor);
                        break;
                    default:
                        break;
                }
            };
        }


        private void SetFullScreenState(bool isFullScreen, bool isWriteable)
        {
            if (isWriteable)
            {
                this.ExpanderLightDismissOverlay.Hide();

                this.ToolListView.IsShow = false;
                this.LayerListView.IsShow = false;
                VisualStateManager.GoToState(this, nameof(Writeable), useTransitions: true);
            }
            else if (isFullScreen)
            {
                this.ExpanderLightDismissOverlay.Hide();

                this.ToolListView.IsShow = false;
                this.LayerListView.IsShow = false;
                VisualStateManager.GoToState(this, nameof(FullScreen), useTransitions: true);
            }
            else
            {
                this.ToolListView.IsShow = true;
                this.LayerListView.IsShow = true;
                VisualStateManager.GoToState(this, nameof(UnFullScreen), useTransitions: true);
            }
        }

        private void ConstructStoryboard()
        {
            this.UnFullScreenButton.Click += (s, e) =>
            {
                this.IsFullScreen = false;
                this.SetFullScreenState(this.IsFullScreen, this.OptionType != default);
            };
            this.FullScreenButton.Click += async (s, e) =>
            {
                if (this.OptionType == default)
                {
                    this.IsFullScreen = !this.IsFullScreen;
                }
                else
                {
                    this.IsFullScreen = false;

                    this.OptionType = default;
                    this.SetOptionType(default);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate

                    this.CanvasAnimatedControl.Paused = false;
                    this.CanvasAnimatedControl.Visibility = Visibility.Visible;
                }

                this.SetFullScreenState(this.IsFullScreen, false);

                this.FullScreenKey.IsEnabled = false;
                await Task.Delay(200);
                this.FullScreenKey.IsEnabled = true;
            };
        }



    }
}