using MiraiPalette.Maui.PageModels;

namespace MiraiPalette.Maui.Pages;

public partial class OptionsPage : ContentPage
{
	public OptionsPage(OptionsPageModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}