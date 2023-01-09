using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Elements
{
    public sealed partial class AnimationIcon : Grid
    {
        public AnimationIcon()
        {
            this.InitializeComponent();
        }
        public void Begin()=>this.SaveStoryboard.Begin(); // Storyboard
    }
}