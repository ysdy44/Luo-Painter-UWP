using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Luo_Painter.Elements
{
    public sealed class SelectedImagesPresenter : Grid // ContentPresenter
    {

        /// <summary> Please binding the <see cref="SelectorItem.IsSelected" /> of Ancestor in SelectedImage.xaml.</summary>
        internal GridViewItem Ancestor;

        #region DependencyProperty 


        /// <summary> Gets or sets <see cref = "SelectedImagesPresenter" />'s Source1. </summary>
        public string Source1
        {
            get => (string)base.GetValue(Source1Property);
            set => base.SetValue(Source1Property, value);
        }
        /// <summary> Identifies the <see cref = "SelectedImagesPresenter.Source1" /> dependency property. </summary>
        public static readonly DependencyProperty Source1Property = DependencyProperty.Register(nameof(Source1), typeof(string), typeof(SelectedImagesPresenter), new PropertyMetadata(null));


        /// <summary> Gets or sets <see cref = "SelectedImagesPresenter" />'s Source2. </summary>
        public string Source2
        {
            get => (string)base.GetValue(Source2Property);
            set => base.SetValue(Source2Property, value);
        }
        /// <summary> Identifies the <see cref = "SelectedImagesPresenter.Source2" /> dependency property. </summary>
        public static readonly DependencyProperty Source2Property = DependencyProperty.Register(nameof(Source2), typeof(string), typeof(SelectedImagesPresenter), new PropertyMetadata(null));


        #endregion

        public SelectedImagesPresenter()
        {
            //base.Unloaded += (s, e) =>
            //{
            //    this.Remove();
            //};
            base.Loaded += (s, e) =>
            {
                if (this.FindAncestor<GridViewItem>() is GridViewItem item)
                {
                    this.Add(item);
                }
                else
                {
                    this.Remove();
                }
            };
        }
        private void Remove()
        {
            this.Ancestor = null;
            this.Children.Clear();
        }
        private void Add(GridViewItem item)
        {
            this.Ancestor = item;
            this.Children.Add(new SelectedImages(this));
        }
    }

    internal sealed partial class SelectedImages : UserControl
    {
        //@Converter
        private double BooleanToVisibilityConverter(bool value) => value ? 0.6 : 1;

        readonly SelectedImagesPresenter Presenter;

        internal SelectedImages(SelectedImagesPresenter presenter)
        {
            this.InitializeComponent();
            this.Presenter = presenter;
        }
    }
}