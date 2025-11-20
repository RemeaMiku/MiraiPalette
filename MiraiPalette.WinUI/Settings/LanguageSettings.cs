using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MiraiPalette.WinUI.Settings;

public static class LanguageSettings
{
    public const string SettingKey = "Language";

    public const string System = "System";

    public const string zhCN = "zh-CN";

    public const string enUS = "en-US";

    public const string jaJP = "ja-JP";

    public const string Fallback = enUS;

    public const string Default = System;

    public static readonly string[] SupportedLanguages =
    [
        zhCN,
        enUS,
        jaJP
    ];

    private static readonly Dictionary<string, string> _fallbackMap = new()
    {
        { "zh-TW", zhCN },
        { "zh-HK", zhCN },
        { "zh-MO", zhCN },
        { "en-GB", enUS },
        { "en-AU", enUS },
        { "en-CA", enUS },
    };

    /// <summary>
    /// Try to convert the language setting to an actual language.
    /// If the setting is <see cref="System"/>, the actual language will be determined based on the system language.
    /// But if the system language is not supported, the fallback language will be used.
    /// If the setting is not supported, the fallback language will be used.
    /// </summary>
    /// <param name="languageSetting"> original language setting </param>
    /// <param name="actualLanguage"> output actual language </param>
    /// <returns> true if the actual language is same as the setting; false if the actual language is different from the setting </returns>
    public static bool TryConvertSettingToActual(string languageSetting, out string actualLanguage)
    {
        if(SupportedLanguages.Contains(languageSetting))
        {
            actualLanguage = languageSetting;
            return true;
        }
        if(!languageSetting.Equals(System, StringComparison.OrdinalIgnoreCase))
        {
            actualLanguage = Fallback;
            return false;
        }
        var systemLang = CultureInfo.InstalledUICulture.Name;
        if(SupportedLanguages.Contains(systemLang))
        {
            actualLanguage = systemLang;
            return true;
        }
        if(_fallbackMap.TryGetValue(systemLang, out var mappedLang))
        {
            actualLanguage = mappedLang;
            return false;
        }
        var systemLangPrefix = CultureInfo.InstalledUICulture.TwoLetterISOLanguageName;
        var match = SupportedLanguages.FirstOrDefault(x => x.StartsWith(systemLangPrefix, StringComparison.OrdinalIgnoreCase));
        actualLanguage = match is not null ? match : Fallback;
        return false;
    }
}
