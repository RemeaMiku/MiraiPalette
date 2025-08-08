using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using MiraiPalette.Avalonia.ViewModels;

namespace MiraiPalette.Avalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetRequiredService<MainWindowViewModel>();
    }
}
