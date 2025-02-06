using Windows.UI.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace Luo_Painter.Controls
{
    public sealed partial class LogDialog : ContentDialog
    {
        public LogDialog()
        {
            this.InitializeComponent();
            foreach (VersionType item in System.Enum.GetValues(typeof(VersionType)))
            {
                Version version = Version.Create(item);

                this.StackPanel.Children.Add(new TextBlock
                {
                    IsTextSelectionEnabled = true,
                    Inlines =
                    {
                        // 1. Version
                        new Run
                        {
                            FontSize = 20,
                            FontWeight = FontWeights.Bold,
                            Text = "・"
                        },
                        new Run
                        {
                            FontSize = 24,
                            FontWeight = FontWeights.SemiLight,
                            Text = version.VersionTitle
                        },
                        // 2. Time
                        new LineBreak(),
                        new Run
                        {
                            FontStyle = FontStyle.Italic,
                            Text = "#"
                        },
                        new Run
                        {
                            FontStyle = FontStyle.Italic,
                            Text = version.Time
                        },
                        // 3. Description
                        new LineBreak(),
                        new Run
                        {
                            Text = version.Description
                        },
                    }
                });
            }
        }
    }
}