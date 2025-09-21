using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Windows.UI;

namespace MiraiPalette.WinUI.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    public ObservableCollection<PaletteViewModel> Palettes { get; } =
        [
            new()
            {
                Title = "Default",
                Description = "The default palette.",
                Colors=
                [
                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                ]
            },
        new()
            {
                Title = "Default",
                Description = "The default palette.",
                Colors=
                [
                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                                        new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                                        new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                                        new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                                        new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                ]
            },new()
            {
                Title = "Default",
                Description = "The default palette.",
                Colors=
                [
                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                                        new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                ]
            },new()
            {
                Title = "Default",
                Description = "The default palette.",
                Colors=
                [
                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                ]
            },new()
            {
                Title = "Default",
                Description = "The default palette.",
                Colors=
                [
                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                ]
            },new()
            {
                Title = "Default",
                Description = "The default palette.",
                Colors=
                [
                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                                        new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                ]
            },
        new()
            {
                Title = "Default",
                Description = "The default palette.",
                Colors=
                [
                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                                        new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                ]
            },
        new()
            {
                Title = "Default",
                Description = "The default palette.",
                Colors=
                [
                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                                        new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                ]
            },
        new()
            {
                Title = "Default",
                Description = "The default palette.",
                Colors=
                [
                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                                        new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                ]
            },
        new()
            {
                Title = "Default",
                Description = "The default palette.",
                Colors=
                [
                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                                        new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                ]
            },
        new()
            {
                Title = "Default",
                Description = "The default palette.",
                Colors=
                [
                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                                        new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                ]
            },new()
            {
                Title = "Default",
                Description = "The default palette.",
                Colors=
                [
                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                                        new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                ]
            },new()
            {
                Title = "Default",
                Description = "The default palette.",
                Colors=
                [
                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                                        new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                ]
            },new()
            {
                Title = "Default",
                Description = "The default palette.",
                Colors=
                [
                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                                        new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                ]
            },new()
            {
                Title = "Default",
                Description = "The default palette.",
                Colors=
                [
                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                                        new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                ]
            },new()
            {
                Title = "Default",
                Description = "The default palette.",
                Colors=
                [
                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                                        new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                ]
            },new()
            {
                Title = "Default",
                Description = "The default palette.",
                Colors=
                [
                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                                        new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                ]
            },new()
            {
                Title = "Default",
                Description = "The default palette.",
                Colors=
                [
                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                                        new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                ]
            },new()
            {
                Title = "Default",
                Description = "The default palette.",
                Colors=
                [
                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                                        new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                ]
            },new()
            {
                Title = "Default",
                Description = "The default palette.",
                Colors=
                [
                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                                        new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                ]
            },new()
            {
                Title = "Default",
                Description = "The default palette.",
                Colors=
                [
                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                                        new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                ]
            },new()
            {
                Title = "Default",
                Description = "The default palette.",
                Colors=
                [
                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                                        new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                ]
            },new()
            {
                Title = "Default",
                Description = "The default palette.",
                Colors=
                [
                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                                        new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                ]
            },new()
            {
                Title = "Default",
                Description = "The default palette.",
                Colors=
                [
                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                                        new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                ]
            },new()
            {
                Title = "Default",
                Description = "The default palette.",
                Colors=
                [
                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                                        new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                ]
            },new()
            {
                Title = "Default",
                Description = "The default palette.",
                Colors=
                [
                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                                        new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                ]
            },new()
            {
                Title = "Default",
                Description = "The default palette.",
                Colors=
                [
                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                                        new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                ]
            },new()
            {
                Title = "Default",
                Description = "The default palette.",
                Colors=
                [
                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                                        new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                ]
            },new()
            {
                Title = "Default",
                Description = "The default palette.",
                Colors=
                [
                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                                        new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                ]
            },new()
            {
                Title = "Default",
                Description = "The default palette.",
                Colors=
                [
                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                                        new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                ]
            },new()
            {
                Title = "Default",
                Description = "The default palette.",
                Colors=
                [
                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                                        new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },                    new() { Name = "Red", Color=Color.FromArgb(255, 0, 0, 255) },
                    new() { Name = "Green", Color=Color.FromArgb(255, 0, 255, 0) },
                    new() { Name = "Blue", Color=Color.FromArgb(255, 255, 0, 0) },
                ]
            },
        ];


}
