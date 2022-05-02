using Luo_Painter.Elements;
using Microsoft.Graphics.Canvas;
using System;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page
    {


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

    }
}