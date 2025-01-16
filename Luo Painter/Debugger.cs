using Luo_Painter.Models;
using Luo_Painter.Strings;
using System;
using System.Text;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    /// <summary>
    /// Hook the <see cref="Application.UnhandledException"/>.
    /// </summary>
    public class Debugger : ICommand
    {
        /// <summary>
        /// <see cref="ContentDialog.Title"/>.
        /// </summary>
        public readonly string Title;
        /// <summary>
        /// <see cref="ContentControl.Content"/>.
        /// </summary>
        public readonly string Content;

        /// <summary>
        /// Construct a <see cref="Debugger"/> by a <see cref="UnhandledExceptionEventArgs"/>.
        /// </summary>
        public Debugger(Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            this.Title = e.Message;
            StringBuilder sb = new StringBuilder();

            // 2. Version
            sb.Append(UIType.Name.GetString());
            sb.Append(": ");
            sb.AppendLine(UIType.Version.GetString());

            // 3. Github
            sb.Append(UIType.Github.GetString());
            sb.Append(": ");
            sb.AppendLine(UIType.GithubLink.GetString());

            // 4. Feedback
            sb.Append(UIType.Feedback.GetString());
            sb.Append(": ");
            sb.AppendLine(UIType.FeedbackLink.GetString());

            // 5. Exception
            sb.AppendLine();
            sb.AppendLine(e.Message);
            sb.AppendLine();

            this.Content = sb.ToString();
        }

        //@Command 
        public ICommand Command => this;
        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter) => true;

        /// <summary>
        /// The parameter is <see cref="ContentDialogButton"/>.
        /// <br/>
        /// <br/> Email: <see cref="ContentDialogButton.Primary"/>
        /// <br/> Clipboard: <see cref="ContentDialogButton.Secondary"/>
        /// </summary>
        /// <param name="parameter"></param>
        public async void Execute(object parameter)
        {
            if (parameter is ContentDialogButton button)
            {
                switch (button)
                {
                    case ContentDialogButton.None:
                        break;
                    case ContentDialogButton.Primary:
                        string messageBody = Uri.EscapeDataString(this.Content);
                        string url = $"mailto:{UIType.FeedbackLink.GetString()}?subject={UIType.Name.GetString()} {UIType.Version.GetString()}&body={messageBody}";
                        await Launcher.LaunchUriAsync(new Uri(url));
                        break;
                    case ContentDialogButton.Secondary:
                        DataPackage content = new DataPackage();
                        content.SetText(this.Content);
                        Clipboard.SetContent(content);
                        break;
                    case ContentDialogButton.Close:
                        break;
                    default:
                        break;
                }
            }
        }
    }
}