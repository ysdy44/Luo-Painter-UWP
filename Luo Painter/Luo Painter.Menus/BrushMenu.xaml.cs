using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Menus
{
    internal sealed class PaintBrushGroupingList : List<PaintBrushGrouping> { }
    internal class PaintBrushGrouping : List<PaintBrush>, IList<PaintBrush>, IGrouping<PaintBrushGroupType, PaintBrush>
    {
        public PaintBrushGroupType Key { set; get; }
    }

    internal class PaintBrushCommand : RelayCommand<PaintBrush> { }
    internal sealed class PaintBrushListView : ListView
    {

        #region DependencyProperty


        /// <summary> Gets or set the command for <see cref="PaintBrushListView"/>. </summary>
        public PaintBrushCommand Command
        {
            get => (PaintBrushCommand)base.GetValue(CommandProperty);
            set => base.SetValue(CommandProperty, value);
        }
        /// <summary> Identifies the <see cref = "PaintBrushListView.Command" /> dependency property. </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof(Command), typeof(PaintBrushCommand), typeof(PaintBrushListView), new PropertyMetadata(null));


        #endregion

        //@Static
        static readonly IList<PaintBrushListView> Children = new List<PaintBrushListView>();

        //@Construct
        ~PaintBrushListView() => PaintBrushListView.Children.Remove(this);
        public PaintBrushListView()
        {
            PaintBrushListView.Children.Add(this);
            base.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is PaintBrush item)
                {
                    this.Command.Execute(item); // Delegate
                    foreach (PaintBrushListView child in PaintBrushListView.Children)
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