using Luo_Painter.Elements;
using Luo_Painter.Layers;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.Controls
{
    public sealed partial class LayerListView : XamlListView
    {
        //@Delegate
        public event EventHandler<ILayer> VisualClick { remove => this.VisualCommand.Click -= value; add => this.VisualCommand.Click += value; }

        //@Content
        public bool IsOpen => this.RenamePopup.IsOpen;

        public ImageSource Source { get; set; }

        readonly Popup RenamePopup = new Popup();
        readonly TextBox RenameTextBox = new TextBox();

        //@Construct
        public LayerListView()
        {
            this.InitializeComponent();
            this.RenamePopup.Child = this.RenameTextBox;

            this.ConstructRenames();
            this.ConstructRename();
        }
    }
}