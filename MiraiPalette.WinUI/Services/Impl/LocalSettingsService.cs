using System;
using Microsoft.Windows.Storage;

namespace MiraiPalette.WinUI.Services.Impl;

public class LocalSettingsService : ISettingsService
{
    // 修正: ApplicationData.Current は存在しないため、GetDefault() を使用
    private readonly ApplicationDataContainer _container = ApplicationData.GetDefault().LocalSettings;

    public T? GetValue<T>(string key, T? defaultValue = default)
    {
        if(defaultValue is Enum)
            return GetEnum(key, defaultValue);
        if(!_container.Values.TryGetValue(key, out var value) || value is not T tValue)
        {
            SetValue(key, defaultValue);
            return defaultValue;
        }
        return tValue;
    }

    private TEnum GetEnum<TEnum>(string key, TEnum defaultValue) where TEnum : struct, Enum
    {
        if(_container.Values.TryGetValue(key, out var raw) && raw is int intValue && Enum.IsDefined(typeof(TEnum), intValue))
        {
            return (TEnum)Enum.ToObject(typeof(TEnum), intValue);
        }
        SetValue(key, Convert.ToInt32(defaultValue));
        return defaultValue;
    }

    public void SetValue<T>(string key, T value)
    {
        _container.Values[key] = value;
    }
}
