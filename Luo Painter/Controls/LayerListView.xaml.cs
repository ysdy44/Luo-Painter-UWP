using Luo_Painter.Elements;
using Luo_Painter.Layers;
using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.Controls
{
    internal class LayerCommand : RelayCommand<ILayer> { }

    public sealed partial class LayerListView : XamlListView
    {
        //@Delegate
        public event EventHandler<ILayer> VisualClick
        {
            remove => this.VisualCommand.Click -= value;
            add => this.VisualCommand.Click += value;
        }
        public event RoutedEventHandler AddClick
        {
            remove => this.AddButton.Click -= value;
            add => this.AddButton.Click += value;
        }
        public event RoutedEventHandler PropertyClick
        {
            remove => this.PropertyButton.Click -= value;
            add => this.PropertyButton.Click += value;
        }

        //@Content
        public FrameworkElement PlacementTarget => this.PropertyButton;
        public ImageSource MarqueeSource
        {
            get => this.Image.Source;
            set => this.Image.Source = value;
        }

        //@Construct
        public LayerListView()
        {
            this.InitializeComponent();
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

    }
}