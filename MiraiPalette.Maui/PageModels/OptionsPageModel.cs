using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using LanguageOption = (string Language, string Code);

namespace MiraiPalette.Maui.PageModels;

public partial class OptionsPageModel:ObservableObject
{
    public List<LanguageOption> Languages { get; } =
    [
        ("简体中文", "zh"),
        ("日本語", "ja"),
    ];

    
}
