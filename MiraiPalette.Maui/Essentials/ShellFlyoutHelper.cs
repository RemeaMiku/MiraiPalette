namespace MiraiPalette.Maui.Essentials;

public class ShellFlyoutHelper
{

    private static FlyoutBehavior? _normalBehavior;

    public const double CompactModeThresholdWidth = 600;

    private static FlyoutBehavior GetFlyoutBehaviorForWidth(double width)
    {
        return width < CompactModeThresholdWidth ? FlyoutBehavior.Flyout : FlyoutBehavior.Locked;
    }

    public static void RegisterListener(Window window)
    {
        window.SizeChanged += (_, _) =>
        {
            var normalBehavior = GetFlyoutBehaviorForWidth(window.Width);
            if(_normalBehavior is null)
            {
                Shell.Current.FlyoutBehavior = normalBehavior;
                return;
            }
            _normalBehavior = normalBehavior;
        };
    }

    public static void DisableFlyout()
    {
        if(Shell.Current.FlyoutBehavior == FlyoutBehavior.Disabled)
            return;
        _normalBehavior = Shell.Current.FlyoutBehavior;
        Shell.Current.FlyoutBehavior = FlyoutBehavior.Disabled;
    }

    public static void RestoreFlyout()
    {
        if(_normalBehavior is null)
            return;
        Shell.Current.FlyoutBehavior = _normalBehavior.Value;
        _normalBehavior = null;
    }
}
