using CommunityToolkit.Mvvm.ComponentModel;
using MiraiPalette.Maui.Essentials;

namespace MiraiPalette.Maui.PageModels;

public partial class OptionsPageModel : ObservableObject
{
    [ObservableProperty]
    public partial LanguageOption SelectedLanguageOption { get; set; } = LanguageOption.Current;

    public List<LanguageOption> LanguageOptions { get; } = LanguageOption.LanguageOptions;

    partial void OnSelectedLanguageOptionChanged(LanguageOption value)
    {
        value.Apply();
        // 重新导航到当前页面，强制刷新
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var current = Shell.Current.CurrentPage;
            var route = Shell.Current.CurrentState.Location.OriginalString;
            await Shell.Current.GoToAsync("///" + route, true); // true 表示重置导航堆栈
        });
    }
}