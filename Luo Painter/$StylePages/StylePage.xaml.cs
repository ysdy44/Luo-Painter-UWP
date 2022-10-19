using System;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class StylePage : Page
    {
        public StylePage()
        {
            this.InitializeComponent();
            base.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                this.AlignmentGrid.RebuildWithInterpolation(e.NewSize);
            };
            this.BackButton.Click += (s, e) =>
            {
                if (base.Frame.CanGoBack)
                {
                    base.Frame.GoBack();
                }
            };
            this.ShowDialogButton.Click += async (s, e) => await this.ContentDialog.ShowAsync();
        }
    }
}