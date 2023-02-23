using FanKit.Transformers;
using Luo_Painter.Models;
using System;
using System.Numerics;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Luo_Painter
{
    internal sealed class MainPageToDrawPageAttribute : Attribute
    {
        readonly NavigationMode NavigationMode;
        public MainPageToDrawPageAttribute(NavigationMode navigationMode) => this.NavigationMode = navigationMode;
        public override string ToString() => $"{typeof(MainPage)} to {typeof(DrawPage)}, Parameter is {typeof(ProjectParameter)}, NavigationMode is {this.NavigationMode}";
    }
    internal sealed class DrawPageToStylePageAttribute : Attribute
    {
        readonly NavigationMode NavigationMode;
        public DrawPageToStylePageAttribute(NavigationMode navigationMode) => this.NavigationMode = navigationMode;
        public override string ToString() => $"{typeof(DrawPage)} to {typeof(StylePage)}, NavigationMode is {this.NavigationMode}";
    }

    // 1. It is a UserControl.xaml that contains a ListView.
    // <UserControl>
    //      <ListView>
    //      ...
    //      </ListView>
    // </UserControl>
    // Ok.

    // 2. It is a UserControl.xaml, RootNode is ListView.
    // <ListView>
    //      ...
    // </ListView>
    // Exception:
    // Windows.UI.Xaml.Markup.XamlParseException:
    // “XAML parsing failed.”
    // Why ?

    // 3. It is a UserControl.xaml, RootNode is XamlListView.
    // <local:XamlListView>
    //      ...
    // </local:XamlListView>
    // Ok, but why ?
    public class XamlListView : ListView
    {
    }
    public class XamlGridView : GridView
    {
    }

    internal struct TransformBase
    {
        public bool IsMove;
        public TransformerMode Mode;

        public Transformer StartingTransformer;
        public Transformer Transformer;
    }
    internal struct Transform
    {
        public bool IsMove;
        public TransformerMode Mode;

        public Matrix3x2 Matrix;
        public TransformerBorder Border;

        public Transformer StartingTransformer;
        public Transformer Transformer;
    }
    internal struct FreeTransform
    {
        public Vector2 Distance;
        public TransformerMode Mode;

        public Matrix3x2 Matrix;
        public TransformerBorder Border;

        public Transformer Transformer;
    }
}