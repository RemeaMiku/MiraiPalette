using System.Globalization;

namespace MiraiPalette.WinUI.Strings;

public static class StringsManager
{
    public static readonly string SystemLanguageCode = CultureInfo.InstalledUICulture.TwoLetterISOLanguageName;

    public static void SetCulture(string? languageCode)
    {
        var culture = string.IsNullOrWhiteSpace(languageCode) || string.Compare(languageCode, "system", true) == 0 ? CultureInfo.InstalledUICulture : new CultureInfo(languageCode);
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }
}
