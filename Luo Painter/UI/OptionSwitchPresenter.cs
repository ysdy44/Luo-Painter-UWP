using Luo_Painter.Elements;
using Luo_Painter.Models;
using Windows.UI.Xaml.Markup;

namespace Luo_Painter.UI
{
    [ContentProperty(Name = nameof(SwitchCases))]
    internal sealed class OptionSwitchPresenter : SwitchPresenter<OptionType> { }
}