using Microsoft.Windows.Storage;

namespace MiraiPalette.WinUI.Services.Impl;

public class LocalSettingsService : ISettingsService
{
    // 修正: ApplicationData.Current は存在しないため、GetDefault() を使用
    private readonly ApplicationDataContainer _container = ApplicationData.GetDefault().LocalSettings;

    public T? GetValue<T>(string key, T? defaultValue = default)
    {
        return !_container.Values.TryGetValue(key, out var value) || value is not T tValue ? defaultValue : tValue;
    }

    public void SetValue<T>(string key, T value)
    {
        _container.Values[key] = value;
    }
}
