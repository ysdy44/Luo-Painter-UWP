using Windows.UI.Xaml.Controls;

namespace Luo_Painter.UI
{
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
    public class XamlGridView : GridView
    {
    }
}