using MiraiPalette.Maui.Resources.Globalization;

namespace MiraiPalette.Maui.Essentials;
public class LanguageOption(string language, string code)
{
    public string Language { get; set; } = language;
    public string Code { get; set; } = code;

    public override string ToString() => Language;

    public static List<LanguageOption> LanguageOptions { get; } =
    [
        new("中文", "zh"),
        new("日本語", "ja"),
    ];

    public static LanguageOption Current
    {
        get
        {
            var currentCode = StringResource.Culture?.TwoLetterISOLanguageName ?? Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            return LanguageOptions.First(option => option.Code == currentCode) ?? LanguageOptions[0];
        }
    }

    public void Apply()
    {
        if(Current.Code != Code)        
            StringResource.Culture = new(Code);       
    }    
}
