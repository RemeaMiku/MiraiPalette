namespace MiraiPalette.Maui.Behaviors;

public partial class EntryRestoreBehavior : Behavior<Entry>
{
    private string _lastValidText = string.Empty;

    protected override void OnAttachedTo(Entry bindable)
    {
        base.OnAttachedTo(bindable);
        bindable.Focused += OnFocused;
        bindable.Unfocused += OnUnfocused;
        if(!string.IsNullOrWhiteSpace(bindable.Text))
            _lastValidText = bindable.Text;
    }

    private void OnFocused(object? sender, FocusEventArgs e)
    {
        if(sender is Entry entry && !string.IsNullOrEmpty(entry.Text))
        {
            _lastValidText = entry.Text;
            entry.Focused -= OnFocused;
        }
    }

    protected override void OnDetachingFrom(Entry bindable)
    {
        base.OnDetachingFrom(bindable);
        bindable.Unfocused -= OnUnfocused;
    }

    private void OnUnfocused(object? sender, FocusEventArgs e)
    {
        if(sender is Entry entry)
        {
            if(string.IsNullOrWhiteSpace(entry.Text))
                entry.Text = _lastValidText;
            else
                _lastValidText = entry.Text;
        }
    }
}