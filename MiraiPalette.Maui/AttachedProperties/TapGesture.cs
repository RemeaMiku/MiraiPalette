using System.Windows.Input;

namespace MiraiPalette.Maui.AttachedProperties;

public static class TapGesture
{
    public static readonly BindableProperty CommandProperty =
    BindableProperty.CreateAttached("Command", typeof(ICommand), typeof(ICommand), default(ICommand), propertyChanged: OnCommandChanged);

    private static void OnCommandChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if(bindable is not View view)
            throw new ArgumentException("Bindable object must be a view");
        var gesture = (TapGestureRecognizer?)view.GestureRecognizers.FirstOrDefault(g => g is TapGestureRecognizer);
        if(newValue is ICommand command)
        {
            if(gesture is not null)
                gesture.Command = command;
            else
            {
                gesture = new()
                {
                    Command = command,
                };
                view.GestureRecognizers.Add(gesture);
            }
        }
        else
        {
            if(gesture is not null)
                view.GestureRecognizers.Remove(gesture);
        }
    }

    public static ICommand GetCommand(BindableObject view)
    {
        return (ICommand)view.GetValue(CommandProperty);
    }

    public static void SetCommand(BindableObject view, ICommand value)
    {
        view.SetValue(CommandProperty, value);
    }




    public static readonly BindableProperty CommandParameterProperty =
    BindableProperty.CreateAttached("CommandParameter", typeof(object), typeof(object), default, propertyChanged: OnCommandParameterChanged);

    private static void OnCommandParameterChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if(bindable is not View view)
            throw new ArgumentException("Bindable object must be a view");
        var gesture = (TapGestureRecognizer?)view.GestureRecognizers.FirstOrDefault(g => g is TapGestureRecognizer);
        if(gesture is null)
        {
            gesture = new();
            view.GestureRecognizers.Add(gesture);
        }
        gesture.CommandParameter = newValue;
    }

    public static object GetCommandParameter(BindableObject view)
    {
        return view.GetValue(CommandParameterProperty);
    }

    public static void SetCommandParameter(BindableObject view, object value)
    {
        view.SetValue(CommandParameterProperty, value);
    }

}
