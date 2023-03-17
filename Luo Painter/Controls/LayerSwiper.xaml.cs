using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Microsoft.Graphics.Canvas.Effects;
using System.Windows.Input;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.Controls
{
    [ContentProperty(Name = nameof(Child))]
    public sealed partial class LayerSwiper : SwipeControl
    {

        //@Content
        public UIElement Child { get => this.ChildBorder.Child; set => this.ChildBorder.Child = value; }
        public string Text { get => this.Run1.Text; set => this.Run1.Text = value; }
        public LayerType Type
        {
            set
            {
                this.Run2.Text = App.Resource.GetString($"Layer_{value}");

                switch (value)
                {
                    case LayerType.Group:
                        this.ChildBorder.Visibility = Visibility.Collapsed;
                        this.SymbolIcon.Visibility = Visibility.Visible;
                        base.Height = 40;
                        break;
                    default:
                        this.SymbolIcon.Visibility = Visibility.Collapsed;
                        this.ChildBorder.Visibility = Visibility.Visible;
                        base.Height = 55;
                        break;
                }
            }
        }

        public BlendEffectMode BlendMode { set => this.BlendModeTextBlock.Text = value.GetIcon(); }
        public int Depth { set => base.Padding = new Thickness(value * 20, 0, 2, 0); }
        public bool IsExpand { set => this.RotateTransform.Angle = value ? 0 : 90; }
        public Visibility Visibility2
        {
            set
            {
                switch (value)
                {
                    case Visibility.Visible:
                        this.ChildBorder.Opacity = this.TextBlock.Opacity = this.BlendModeTextBlock.Opacity = this.ExpandIcon.Opacity = this.VisibilityIcon.Opacity = 1;
                        break;
                    case Visibility.Collapsed:
                        this.ChildBorder.Opacity = this.TextBlock.Opacity = this.BlendModeTextBlock.Opacity = this.ExpandIcon.Opacity = this.VisibilityIcon.Opacity = 0.5;
                        break;
                    default:
                        break;
                }
            }
        }

        public bool IsExist
        {
            get => this.isExist;
            set
            {
                if (this.isExist == value) return;
                this.isExist = value;
                if (this.Ancestor is null) return;
                this.Ancestor.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        private bool isExist;

        public TagType TagType
        {
            get => this.tagType;
            set
            {
                if (this.tagType == value) return;
                this.tagType = value;
                if (this.Ancestor is null) return;
                if (this.Ancestor.Background is SolidColorBrush item)
                {
                    item.Color = this.ToColor(value);
                }
            }
        }
        private TagType tagType;

        ListViewItem Ancestor;

        #region DependencyProperty


        /// <summary> <see cref="ButtonBase.CommandParameter"/>. </summary>
        public object CommandParameter
        {
            get => (object)base.GetValue(CommandParameterProperty);
            set => base.SetValue(CommandParameterProperty, value);
        }
        /// <summary> Identifies the <see cref = "LayerSwiper.CommandParameter" /> dependency property. </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(LayerSwiper), new PropertyMetadata(null));


        /// <summary> <see cref="ButtonBase.Command"/>. </summary>
        public ICommand ExpandCommand
        {
            get => (ICommand)base.GetValue(ExpandCommandProperty);
            set => base.SetValue(ExpandCommandProperty, value);
        }
        /// <summary> Identifies the <see cref = "LayerSwiper.ExpandCommand" /> dependency property. </summary>
        public static readonly DependencyProperty ExpandCommandProperty = DependencyProperty.Register(nameof(ExpandCommand), typeof(ICommand), typeof(LayerSwiper), new PropertyMetadata(null));


        /// <summary> <see cref="ButtonBase.Command"/>. </summary>
        public ICommand VisibilityCommand
        {
            get => (ICommand)base.GetValue(VisibilityCommandProperty);
            set => base.SetValue(VisibilityCommandProperty, value);
        }
        /// <summary> Identifies the <see cref = "LayerSwiper.VisibilityCommand" /> dependency property. </summary>
        public static readonly DependencyProperty VisibilityCommandProperty = DependencyProperty.Register(nameof(VisibilityCommand), typeof(ICommand), typeof(LayerSwiper), new PropertyMetadata(null));


        #endregion

        public LayerSwiper()
        {
            this.InitializeComponent();
            // base.Unloaded += (s, e) => this.Ancestor = null;
            base.Loaded += (s, e) =>
            {
                this.Ancestor = this.FindAncestor<ListViewItem>();
                if (this.Ancestor is null) return;

                this.Ancestor.Visibility = base.IsHitTestVisible ? Visibility.Visible : Visibility.Collapsed;
                this.Ancestor.Background = new SolidColorBrush
                {
                    Opacity = 0.5,
                    Color = this.ToColor(this.TagType)
                };
            };

            this.Item.Invoked += (s, e) =>
            {
                if (this.Ancestor is null) return;
                this.Ancestor.IsSelected = !this.Ancestor.IsSelected;
            };

            this.ExpandButton.RightTapped += (s, e) => e.Handled = true;
            this.ExpandButton.DoubleTapped += (s, e) => e.Handled = true;

            this.VisibilityButton.RightTapped += (s, e) => e.Handled = true;
            this.VisibilityButton.DoubleTapped += (s, e) => e.Handled = true;
        }

        private Color ToColor(TagType tagType)
        {
            switch (tagType)
            {
                case TagType.None: return Colors.Transparent;
                case TagType.Red: return Colors.LightCoral;
                case TagType.Orange: return Colors.Orange;
                case TagType.Yellow: return Colors.Yellow;
                case TagType.Green: return Colors.YellowGreen;
                case TagType.Blue: return Colors.SkyBlue;
                case TagType.Purple: return Colors.Plum;
                default: return Colors.Transparent;
            }
        }

    }
}