using Luo_Painter.Elements;
using System;
using Windows.UI.Xaml;
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
    internal class AnimalCase : DependencyObject, ICase<Animal>
    {
        public object Content
        {
            get => (object)base.GetValue(ContentProperty);
            set => base.SetValue(ContentProperty, value);
        }
        /// <summary> Identifies the <see cref="Content"/> property. </summary>
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof(Content), typeof(object), typeof(AnimalCase), new PropertyMetadata(null));

        public Animal Value
        {
            get => (Animal)base.GetValue(ValueProperty);
            set => base.SetValue(ValueProperty, value);
        }
        /// <summary> Identifies the <see cref="Value"/> property. </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(Animal), typeof(AnimalCase), new PropertyMetadata(default(Animal)));

        public void OnNavigatedTo() { }

        public void OnNavigatedFrom() { }
    }

    [ContentProperty(Name = nameof(SwitchCases))]
    internal class AnimalSwitchPresenter : Elements.SwitchPresenter<Animal> { }

    public sealed partial class SwitchPresenterPage : Page
    {
        public SwitchPresenterPage()
        {
            this.InitializeComponent();
            this.SwitchPresenter.Value = Animal.Cat;
            this.ListView.ItemsSource = Enum.GetValues(typeof(Animal));
            this.ListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is Animal item)
                {
                    this.SwitchPresenter.Value = item;
                }
            };
        }
    }
}