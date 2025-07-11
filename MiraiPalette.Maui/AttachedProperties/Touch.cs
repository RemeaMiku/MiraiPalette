using CommunityToolkit.Maui.Behaviors;

namespace MiraiPalette.Maui.AttachedProperties;

public static class Touch
{
    public static readonly BindableProperty HoveredBackgroundColorProperty =
    BindableProperty.CreateAttached("HoveredBackgroundColor", typeof(Color), typeof(Color), default(Color), propertyChanged: OnHoveredBackgroundColorChanged);

    private static void OnHoveredBackgroundColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if(bindable is not View view)
            throw new ArgumentException("Bindable object must be a view");
        var behavior = (TouchBehavior?)view.Behaviors.FirstOrDefault(b => b is TouchBehavior);
        if(newValue is Color color)
        {
            if(behavior is not null)
                behavior.HoveredBackgroundColor = color;
            else
            {
                behavior = new()
                {
                    HoveredBackgroundColor = color,
                };
                view.Behaviors.Add(behavior);
            }
        }
        else
        {
            if(behavior is not null)
                view.Behaviors.Remove(behavior);
        }
    }

    public static Color GetHoveredBackgroundColor(BindableObject view)
    {
        return (Color)view.GetValue(HoveredBackgroundColorProperty);
    }

    public static void SetHoveredBackgroundColor(BindableObject view, Color value)
    {
        view.SetValue(HoveredBackgroundColorProperty, value);
    }
}