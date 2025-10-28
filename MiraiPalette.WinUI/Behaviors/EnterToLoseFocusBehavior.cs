using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace MiraiPalette.WinUI.Behaviors;

public static class EnterToLoseFocusBehavior
{
    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(EnterToLoseFocusBehavior),
            new PropertyMetadata(false, OnIsEnabledChanged));

    public static void SetIsEnabled(UIElement element, bool value)
        => element.SetValue(IsEnabledProperty, value);

    public static bool GetIsEnabled(UIElement element)
        => (bool)element.GetValue(IsEnabledProperty);

    private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if(d is TextBox tb)
        {
            if((bool)e.NewValue)
                tb.KeyDown += TextBox_KeyDown;
            else
                tb.KeyDown -= TextBox_KeyDown;
        }
    }

    private static void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if(e.Key == Windows.System.VirtualKey.Enter)
        {
            e.Handled = true;
            // 将焦点移动到下一个可聚焦元素（会触发当前 TextBox LostFocus）
            FocusManager.TryMoveFocus(FocusNavigationDirection.Down, new()
            {
                SearchRoot = Current.MainWindow.NavigationFrame.Content as DependencyObject
            });
        }
    }
}

