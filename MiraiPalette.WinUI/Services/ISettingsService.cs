using System.Diagnostics.CodeAnalysis;

namespace MiraiPalette.WinUI.Services;

public interface ISettingsService
{
    [return: NotNullIfNotNull(nameof(defaultValue))]
    T? GetValue<T>(string key, T? defaultValue = default);
    void SetValue<T>(string key, T value);
}
