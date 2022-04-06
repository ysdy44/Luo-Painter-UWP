using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Elements
{
    public static class ContentDialogExtensions
    {
        static ContentDialog Dialog;
        public static async Task<ContentDialogResult> ShowInstance(this ContentDialog contentDialog)
        {
            if (ContentDialogExtensions.Dialog != null)
            {
                ContentDialogExtensions.Dialog.Hide();
                return ContentDialogResult.None;
            }

            ContentDialogExtensions.Dialog = contentDialog ?? throw new NullReferenceException("The contentDialog is null.");

            if (ContentDialogExtensions.Dialog != null)
            {
                ContentDialogExtensions.Dialog.Closed -= ContentDialogExtensions.Dialog_Closed;
                ContentDialogExtensions.Dialog.Closed += ContentDialogExtensions.Dialog_Closed;

                return await ContentDialogExtensions.Dialog.ShowAsync(ContentDialogPlacement.InPlace);
            }

            return ContentDialogResult.None;
        }

        private static void Dialog_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
            sender.Closed -= ContentDialogExtensions.Dialog_Closed;

            ContentDialogExtensions.Dialog = null;
        }
    }
}