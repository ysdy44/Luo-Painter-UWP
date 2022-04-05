using Luo_Painter.Blends;
using Luo_Painter.Tools;
using Microsoft.Graphics.Canvas.Effects;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    /// <summary>
    /// 
    ///   <ContentControl Resources="ms-appx:///Luo Painter.Tools/Icons/PenIcon.xaml">
    ///       <ToolTipService.ToolTip>
    ///          <ToolTip>
    ///                Pen
    ///            </ToolTip>
    ///       </ToolTipService.ToolTip>
    ///        <ContentControl x:Name="Pen" Template="{StaticResource PenIcon}" >
    ///            <tools:ToolType>Pen</tools:ToolType>
    ///        </ContentControl>
    ///   </ContentControl>
    ///   
    /// </summary>
    internal sealed class ToolIcon : ListViewItem
    {
        #region DependencyProperty


        /// <summary> Gets or set the type for <see cref="ToolIcon"/>. </summary>
        public ToolType Type
        {
            get => (ToolType)base.GetValue(TypeProperty);
            set => base.SetValue(TypeProperty, value);
        }
        /// <summary> Identifies the <see cref = "ToolIcon.Type" /> dependency property. </summary>
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(ToolType), typeof(ToolIcon), new PropertyMetadata(ToolType.None, (sender, e) =>
        {
            ToolIcon control = (ToolIcon)sender;

            if (e.NewValue is ToolType value)
            {
                control.Content = new ContentControl
                {
                    Content = value,
                    Template = value.GetTemplate(out ResourceDictionary resource),
                    Resources = resource,
                };
                ToolTipService.SetToolTip(control, new ToolTip
                {
                    Content = value,
                    // Style = App.Current.Resources["AppToolTipStyle"] as Style
                });
            }
        }));


        #endregion
    }
    internal sealed class BlendIcon : ListViewItem
    {
        #region DependencyProperty


        /// <summary> Gets or set the type for <see cref="BlendIcon"/>. </summary>
        public BlendEffectMode Type
        {
            get => (BlendEffectMode)base.GetValue(TypeProperty);
            set => base.SetValue(TypeProperty, value);
        }
        /// <summary> Identifies the <see cref = "BlendIcon.Type" /> dependency property. </summary>
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(BlendEffectMode), typeof(BlendIcon), new PropertyMetadata(BlendEffectMode.Multiply, (sender, e) =>
        {
            BlendIcon control = (BlendIcon)sender;

            if (e.NewValue is BlendEffectMode value)
            {
                control.Content = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Children =
                    {
                        new ContentControl
                        {
                            Content = value,
                            Template = value.GetTemplate(out ResourceDictionary resource, out string title),
                            Resources = resource,
                        },
                        new ContentControl
                        {
                            Width = 12
                        },
                        new TextBlock
                        {
                            Text = title
                        }
                    }
                };
            }
        }));


        #endregion
    }

    public sealed partial class ListViewPage : Page
    {
        public ListViewPage()
        {
            this.InitializeComponent();
        }
    }
}