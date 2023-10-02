using Luo_Painter.Models;
using Windows.UI.Xaml.Markup;

namespace Luo_Painter.Strings
{
    [MarkupExtensionReturnType(ReturnType = typeof(string))]
    public sealed class UIExtension : MarkupExtension
    {
        public UIType Type { get; set; }
        protected override object ProvideValue() => App.Resource.GetString(this.Type.ToString());
    }
}