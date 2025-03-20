using CommunityToolkit.Maui.Behaviors;

namespace MiraiPalette.Maui.Behaviors;

public partial class ButtonSwitchToEntryBehavior : Behavior<Button>
{
    public static readonly BindableProperty SwitchToProperty =
    BindableProperty.Create(nameof(SwitchTo), typeof(Entry), typeof(Entry), default(Entry), propertyChanged: OnSwitchToChanged);

    private static void OnSwitchToChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var behavior = (ButtonSwitchToEntryBehavior)bindable;
        if(behavior._button is null)
            return;
        if(oldValue is Entry oldEntry)
            Unregister(behavior._button, oldEntry);
        if(newValue is not Entry newEntry)
            return;
        Register(behavior._button, newEntry);
    }

    public Entry SwitchTo
    {
        get => (Entry)GetValue(SwitchToProperty);
        set => SetValue(SwitchToProperty, value);
    }

    private Button? _button;

    protected override void OnAttachedTo(Button button)
    {
        _button = button;
        Register(button, SwitchTo);
        base.OnAttachedTo(button);
    }

    protected override void OnDetachingFrom(Button button)
    {
        Unregister(button, SwitchTo);
        base.OnDetachingFrom(button);
        _button = default;
    }

    private static readonly Dictionary<Button, Entry> _buttonEntryPairs = [];

    private static readonly Dictionary<Entry, Button> _entryButtonPairs = [];

    private static void Register(Button button, Entry entry)
    {
        if(button is null || entry is null)
            return;
        _buttonEntryPairs.Add(button, entry);
        _entryButtonPairs.Add(entry, button);
        button.Clicked += OnButtonClicked;
        entry.Unfocused += OnEntryUnfocused;
        entry.Behaviors.Add(new SelectAllTextBehavior());
        entry.IsVisible = false;
    }

    private static void Unregister(Button button, Entry entry)
    {
        if(button is null || entry is null)
            return;
        if(_buttonEntryPairs.TryGetValue(button, out var entryValue) && entryValue == entry)
            _buttonEntryPairs.Remove(button);
        if(_entryButtonPairs.TryGetValue(entry, out var buttonValue) && buttonValue == button)
            _entryButtonPairs.Remove(entry);
        button.Clicked -= OnButtonClicked;
        entry.Unfocused -= OnEntryUnfocused;
    }

    private static void OnEntryUnfocused(object? sender, FocusEventArgs e)
    {
        if(sender is not Entry entry)
            return;
        if(!_entryButtonPairs.TryGetValue(entry, out var button))
            return;
        if(button is null)
            return;
        button.IsVisible = true;
        entry.IsVisible = false;
    }

    private static void OnButtonClicked(object? sender, EventArgs e)
    {
        if(sender is not Button button)
            return;
        if(!_buttonEntryPairs.TryGetValue(button, out var entry))
            return;
        entry.Text = button.Text;
        button.IsVisible = false;
        entry.IsVisible = true;
        entry.Focus();
    }
}
