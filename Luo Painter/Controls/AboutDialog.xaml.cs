using Luo_Painter.Models;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class AboutDialog : ContentDialog
    {
        //@String
        private string GithubLink => App.Resource.GetString(UIType.GithubLink.ToString());
        private string FeedbackLink => $"mailto:{App.Resource.GetString(UIType.FeedbackLink.ToString())}";

        public AboutDialog()
        {
            this.InitializeComponent();
        }
    }
}