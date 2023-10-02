using Luo_Painter.Models;
using Luo_Painter.Strings;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class AboutDialog : ContentDialog
    {
        //@String
        private string GithubLink => UIType.GithubLink.GetString();
        private string FeedbackLink => $"mailto:{UIType.FeedbackLink.GetString()}";

        public AboutDialog()
        {
            this.InitializeComponent();
        }
    }
}