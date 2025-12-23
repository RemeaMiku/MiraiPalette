using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using MiraiPalette.WinUI.Services;
using MiraiPalette.WinUI.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MiraiPalette.WinUI.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();
    }

    public MainPageViewModel ViewModel { get; } = Current.Services.GetRequiredService<MainPageViewModel>();

    private async void MenuFlyout_Opened(object sender, object e)
    {
        if(sender is not MenuFlyout flyout)
            return;
        if(flyout.Items.FirstOrDefault(m => m is MenuFlyoutSubItem) is not MenuFlyoutSubItem subItem)
            return;
        subItem.Items.Clear();
        var folders = await Current.Services.GetRequiredService<IMiraiPaletteStorageService>().GetAllFoldersAsync();
        foreach(var folder in folders)
        {
            if(folder.Id == ViewModel.Folder.Id)
                continue;
            subItem.Items.Add(new MenuFlyoutItem
            {
                Icon = new SymbolIcon(Symbol.Folder),
                Text = folder.Name,
                Command = ViewModel.MovePaletteToFolderCommand,
                CommandParameter = folder.Id
            });
        }
    }
}
