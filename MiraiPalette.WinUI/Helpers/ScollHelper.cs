using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace MiraiPalette.WinUI.Helpers;

public static class ScrollHelper
{
    public static bool ScrollIntoView(
        Control control,
        object? item = null,
        bool animated = true,
        double verticalRatio = 0.5)
    {
        if(control == null)
            return false;

        switch(control)
        {
            // ⭐ WinUI 3 NavigationView 正解
            case NavigationView nav:
            {
                var targetItem = item ?? nav.SelectedItem;
                if(targetItem == null)
                    return false;

                var container = nav.ContainerFromMenuItem(targetItem);
                if(container is UIElement element)
                {
                    element.StartBringIntoView(new BringIntoViewOptions
                    {
                        AnimationDesired = animated,
                        VerticalAlignmentRatio = verticalRatio
                    });
                    return true;
                }
                return false;
            }

            // ⭐ ListView / GridView / ListBox
            case ListViewBase listView:
            {
                var targetItem = item ?? listView.SelectedItem;
                if(targetItem == null)
                    return false;

                listView.ScrollIntoView(targetItem, ScrollIntoViewAlignment.Default);
                return true;
            }

            // ⭐ 已经是 UIElement（兜底）
            case UIElement element:
                element.StartBringIntoView();
                return true;
        }

        return false;
    }
}



