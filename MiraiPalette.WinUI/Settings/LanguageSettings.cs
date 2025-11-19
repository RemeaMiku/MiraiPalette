using System.Globalization;

namespace MiraiPalette.WinUI.Settings;

public static class LanguageSettings
{
    public const string SettingKey = "Language";

    public const string System = "System";

    public const string zhCN = "zh-CN";

    public const string enUS = "en-US";

    public const string jaJP = "ja-JP";

    public static string ConvertToActualLanguage(string languageSetting)
    {
        return languageSetting == System ? CultureInfo.InstalledUICulture.Name : languageSetting;
    }
}
