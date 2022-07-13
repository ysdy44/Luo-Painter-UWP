using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Menus
{
    internal sealed class BrushGroupingList : List<BrushGrouping> { }
    internal class BrushGrouping : List<PaintBrush>, IList<PaintBrush>, IGrouping<PaintBrushGroupType, PaintBrush>
    {
        public PaintBrushGroupType Key { set; get; }
    }

    internal class BrushCommand : RelayCommand<PaintBrush> { }
    internal sealed class BrushListView : ListView
    {

        #region DependencyProperty


        /// <summary> Gets or set the command for <see cref="BrushListView"/>. </summary>
        public BrushCommand Command
        {
            get => (BrushCommand)base.GetValue(CommandProperty);
            set => base.SetValue(CommandProperty, value);
        }
        /// <summary> Identifies the <see cref = "BrushListView.Command" /> dependency property. </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof(Command), typeof(BrushCommand), typeof(BrushListView), new PropertyMetadata(null));


        #endregion

        //@Static
        static readonly IList<BrushListView> Children = new List<BrushListView>();

        //@Construct
        ~BrushListView() => BrushListView.Children.Remove(this);
        public BrushListView()
        {
            BrushListView.Children.Add(this);
            base.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is PaintBrush item)
                {
                    this.Command.Execute(item); // Delegate
                    foreach (BrushListView child in BrushListView.Children)
                    {
                        if (child == this) continue;
                        child.SelectedIndex = -1;
                    }
                }
            };
        }
    }

    public sealed partial class BrushMenu : Expander
    {
        //@Delegate
        public event EventHandler<PaintBrush> ItemClick
        {
            remove => this.Command.Click -= value;
            add => this.Command.Click += value;
        }

        //@Construct
        public BrushMenu()
        {
            this.InitializeComponent();
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

    }
}