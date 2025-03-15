
using CommunityToolkit.Maui.Behaviors;

namespace MiraiPalette.Maui.AttachedProperties;

public static class InputViewSelectAllBehavior
{
    public static readonly BindableProperty EnabledProperty =
    BindableProperty.CreateAttached("Enabled", typeof(bool), typeof(InputView), false, propertyChanged: OnEnabledChanged);

    public static bool GetEnabled(BindableObject bindable) => (bool)bindable.GetValue(EnabledProperty);

    public static void SetEnabled(BindableObject bindable, bool value) => bindable.SetValue(EnabledProperty, value);

    private readonly static SelectAllTextBehavior _behavior = new();

    private static void OnEnabledChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if(bindable is not InputView inputView)
            return;
        if((bool)newValue)
            inputView.Behaviors.Add(_behavior);
        else
        {
            var behavior = inputView.Behaviors.FirstOrDefault(b => b is SelectAllTextBehavior);
            if(behavior is not null)
                inputView.Behaviors.Remove(behavior);
        }
    }
}
