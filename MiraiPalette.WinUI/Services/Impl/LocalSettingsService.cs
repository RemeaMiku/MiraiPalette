using System;
using Microsoft.Windows.Storage;

namespace MiraiPalette.WinUI.Services.Impl;

public class LocalSettingsService : ISettingsService
{
    // 修正: ApplicationData.Current は存在しないため、GetDefault() を使用
    private readonly ApplicationDataContainer _container = ApplicationData.GetDefault().LocalSettings;

    public T? GetValue<T>(string key, T? defaultValue = default)
    {
        if(_container.Values.TryGetValue(key, out var value))
        {
            if(value is int intValue && defaultValue is Enum && Enum.IsDefined(typeof(T), intValue))
                return (T)Enum.ToObject(typeof(T), intValue);
            if(value is T tValue)
                return tValue;
        }
        SetValue(key, defaultValue);
        return defaultValue;
    }

    public void SetValue<T>(string key, T value)
    {
        if(value is Enum enumValue)
        {
            _container.Values[key] = Convert.ToInt32(enumValue);
            return;
        }
        _container.Values[key] = value;
    }
}
