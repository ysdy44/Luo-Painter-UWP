using System;
using Windows.ApplicationModel.Resources;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.Blends
{
    /// <summary>
    /// Segmented of <see cref="TagType"/>
    /// </summary>
    public sealed partial class TagTypeSegmented : UserControl
    {

        //@Delegate
        public event EventHandler<TagType> TypeChanged;

        //@VisualState
        TagType _vsTagType;
        /// <summary> 
        /// Represents the visual appearance of UI elements in a specific state.
        /// </summary>
        public VisualState VisualState
        {
            get
            {
                switch (this._vsTagType)
                {
                    case TagType.None: return this.NoneState;
                    case TagType.Red: return this.RedState;
                    case TagType.Orange: return this.OrangeState;
                    case TagType.Yellow: return this.YellowState;
                    case TagType.Green: return this.GreenState;
                    case TagType.Blue: return this.BlueState;
                    case TagType.Purple: return this.PurpleState;
                    default: return this.Normal;
                }
            }
            set => VisualStateManager.GoToState(this, value.Name, false);
        }

        #region DependencyProperty


        /// <summary> Gets or sets the tag type. </summary>
        public TagType Type
        {
            get => (TagType)base.GetValue(TypeProperty);
            set => base.SetValue(TypeProperty, value);
        }
        /// <summary> Identifies the <see cref = "TagTypeSegmented.Type" /> dependency property. </summary>
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(TagType), typeof(TagTypeSegmented), new PropertyMetadata(TagType.None, (sender, e) =>
        {
            TagTypeSegmented control = (TagTypeSegmented)sender;

            if (e.NewValue is TagType value)
            {
                control._vsTagType = value;
                control.VisualState = control.VisualState; // State
            }
        }));


        #endregion


        //@Construct
        /// <summary>
        /// Initializes a TagTypeControl. 
        /// </summary>
        public TagTypeSegmented()
        {
            this.InitializeComponent();
            this.ConstructGroup();

            this.Loaded += (s, e) => this.VisualState = this.VisualState; // State
        }


        // Strings
        public void ConstructStrings(ResourceLoader resource)
        {

        }


        //@Group
        private void ConstructGroup()
        {
            foreach (UIElement child in this.RootGrid.Children)
            {
                if (child is RadioButton button)
                {
                    string key = button.Name;
                    TagType type = TagType.None;
                    try
                    {
                        type = (TagType)Enum.Parse(typeof(TagType), key);
                    }
                    catch (Exception) { }


                    // Button
                    button.Click += (s, e) => this.TypeChanged?.Invoke(this, type); // Delegate


                    Color color = type.ToColor();
                    button.Background = new SolidColorBrush(color);
                }
            }
        }
    }
}