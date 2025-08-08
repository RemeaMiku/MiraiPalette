using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentAvalonia.UI.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace MiraiPalette.Avalonia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        SelectedNavigationViewItem = NavigationViewItems[0];
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentPageViewModel))]
    public partial NavigationViewItem SelectedNavigationViewItem { get; set; }

    public List<NavigationViewItem> NavigationViewItems { get; } =
        [
            new(){ Content="Palettes",Tag="Palettes" },
            new(){ Content="Image Palette",Tag="ImagePalette" },
        ];

    public PageViewModelBase CurrentPageViewModel
    {
        get
        {
            var pageName = SelectedNavigationViewItem.Tag as string;
            var type = Type.GetType($"MiraiPalette.Avalonia.ViewModels.{pageName}PageViewModel");
            var viewModel = App.Current.Services.GetRequiredService(type) as PageViewModelBase;
            return viewModel;
        }
    }
}