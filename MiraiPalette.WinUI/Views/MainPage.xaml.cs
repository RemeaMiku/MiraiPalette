using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
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
        ViewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    public MainPageViewModel ViewModel { get; } = Current.Services.GetRequiredService<MainPageViewModel>();

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if(ViewModel.CurrentPalette is not null && e.PropertyName == nameof(ViewModel.FoldersForCurrentPaletteToMove))
            UpdateMoveToSubItems();
    }

    private void UpdateMoveToSubItems()
    {
        if(MiraiPaletteContextMenu.Items.FirstOrDefault(m => m is MenuFlyoutSubItem) is not MenuFlyoutSubItem subItem)
            return;
        subItem.Items.Clear();
        var folders = ViewModel.FoldersForCurrentPaletteToMove;
        if(folders is null)
            return;
        foreach(var folder in folders)
        {
            subItem.Items.Add(new MenuFlyoutItem
            {
                Icon = new SymbolIcon(Symbol.Folder),
                Text = folder.Name,
                Command = ViewModel.MovePaletteToFolderCommand,
                CommandParameter = folder.Id
            });
        }
    }

    private void PalettesView_ElementPrepared(ItemsRepeater sender, ItemsRepeaterElementPreparedEventArgs args)
    {
        var visual = ElementCompositionPreview.GetElementVisual(args.Element);
        visual.Opacity = 0;
        visual.Scale = new Vector3(0.9f);

        var compositor = visual.Compositor;

        var fadeIn = compositor.CreateScalarKeyFrameAnimation();
        fadeIn.InsertKeyFrame(1f, 1f);
        fadeIn.Duration = TimeSpan.FromMilliseconds(200);

        var scaleIn = compositor.CreateVector3KeyFrameAnimation();
        scaleIn.InsertKeyFrame(1f, Vector3.One);
        scaleIn.Duration = TimeSpan.FromMilliseconds(200);

        visual.StartAnimation(nameof(visual.Opacity), fadeIn);
        visual.StartAnimation(nameof(visual.Scale), scaleIn);
    }
}
