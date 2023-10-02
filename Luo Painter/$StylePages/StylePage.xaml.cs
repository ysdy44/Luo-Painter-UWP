using Luo_Painter.Elements;
using Luo_Painter.UI;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Luo_Painter
{
    public sealed partial class StylePage : Page
    {
        //@String
        FlowDirection Direction => CultureInfoCollection.FlowDirection;

        public bool Disabler
        {
            get => App.SourcePageType != SourcePageType.StylePage;
            set => App.SourcePageType = value ? SourcePageType.Invalid : SourcePageType.StylePage;
        }

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
                if (this.Disabler) return;

                if (base.Frame.CanGoBack)
                {
                    base.Frame.GoBack();
                }
            };
            this.ShowDialogButton.Click += async (s, e) =>
            {
                if (ContentDialogExtensions.CanShow)
                    await this.ContentDialog.ShowInstance();
            };
        }

        //@BackRequested
        /// <summary> The current page no longer becomes an active page. </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (SystemNavigationManager.GetForCurrentView() is SystemNavigationManager manager)
            {
                manager.BackRequested -= this.BackRequested;
                manager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
        }
        /// <summary> The current page becomes the active page. </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (SystemNavigationManager.GetForCurrentView() is SystemNavigationManager manager)
            {
                manager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                manager.BackRequested += this.BackRequested;
            }

            this.Disabler = false;
        }
        private void BackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;

            if (this.Disabler) return;

            if (base.Frame.CanGoBack)
            {
                base.Frame.GoBack();
            }
        }
    }
}