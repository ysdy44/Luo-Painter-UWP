using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace Luo_Painter.TestApp
{
    internal enum Animal
    {
        Cat,
        Dog,
        Bunny,
        Llama,
        Parrot,
        Squirrel
    }

    [ContentProperty(Name = nameof(Content))]
    internal class AnimalCase : Elements.Case<Animal> { }

    [ContentProperty(Name = nameof(SwitchCases))]
    internal class AnimalSwitchPresenter : Elements.SwitchPresenter<Animal> { }

    public sealed partial class SwitchPresenterPage : Page
    {
        public SwitchPresenterPage()
        {
            this.InitializeComponent();
            this.SwitchPresenter.Value = Animal.Cat;
        }
    }
}