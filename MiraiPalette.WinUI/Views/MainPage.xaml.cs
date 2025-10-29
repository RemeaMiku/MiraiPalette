using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.Storage.Pickers;
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

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        ViewModel.LoadCommand.Execute(null);
    }

    public MainPageViewModel ViewModel { get; } = Current.Services.GetRequiredService<MainPageViewModel>();

    private async void OnExtractFromImageButtonClick(object sender, RoutedEventArgs e)
    {
        var picker = new FileOpenPicker(XamlRoot.ContentIslandEnvironment.AppWindowId)
        {
            CommitButtonText = "选择",
            FileTypeFilter = { ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tiff" },
            SuggestedStartLocation = PickerLocationId.PicturesLibrary,
            ViewMode = PickerViewMode.Thumbnail
        };
        var result = await picker.PickSingleFileAsync();
        if(result is not null)
            Current.NavigateTo(NavigationTarget.ImagePalette, result.Path);
    }
}
