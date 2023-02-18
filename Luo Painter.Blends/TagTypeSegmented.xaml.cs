using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Blends
{
    /// <summary>
    /// Segmented of <see cref="TagType"/>
    /// </summary>
    public sealed partial class TagTypeSegmented : UserControl
    {

        //@Delegate
        public event EventHandler<TagType> TypeChanged;

        //@Converter
        private bool NoneConverter(TagType value) => value != TagType.None;
        private bool RedConverter(TagType value) => value != TagType.Red;
        private bool OrangeConverter(TagType value) => value != TagType.Orange;
        private bool YellowConverter(TagType value) => value != TagType.Yellow;
        private bool GreenConverter(TagType value) => value != TagType.Green;
        private bool BlueConverter(TagType value) => value != TagType.Blue;
        private bool PurpleConverter(TagType value) => value != TagType.Purple;

        #region DependencyProperty

        /// <summary> Gets or sets the tag type. </summary>
        public TagType Type
        {
            get => (TagType)base.GetValue(TypeProperty);
            set => base.SetValue(TypeProperty, value);
        }
        /// <summary> Identifies the <see cref = "TagTypeSegmented.Type" /> dependency property. </summary>
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(TagType), typeof(TagTypeSegmented), new PropertyMetadata(TagType.None));

        #endregion

        //@Construct
        /// <summary>
        /// Initializes a TagTypeControl. 
        /// </summary>
        public TagTypeSegmented()
        {
            this.InitializeComponent();
            base.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                this.StackPanel.Width = e.NewSize.Width;
                this.None.Width =
                this.Red.Width =
                this.Orange.Width =
                this.Yellow.Width =
                this.Green.Width =
                this.Blue.Width =
                this.Purple.Width =
                e.NewSize.Width / 7;
            };

            this.None.Click += (s, e) => this.OnTypeChanged(TagType.None);
            this.Red.Click += (s, e) => this.OnTypeChanged(TagType.Red);
            this.Orange.Click += (s, e) => this.OnTypeChanged(TagType.Orange);
            this.Yellow.Click += (s, e) => this.OnTypeChanged(TagType.Yellow);
            this.Green.Click += (s, e) => this.OnTypeChanged(TagType.Green);
            this.Blue.Click += (s, e) => this.OnTypeChanged(TagType.Blue);
            this.Purple.Click += (s, e) => this.OnTypeChanged(TagType.Purple);
        }
        private void OnTypeChanged(TagType value)
        {
            this.Type = value;
            this.TypeChanged?.Invoke(this, value); // Delegate
        }
    }
}