using MealMe.ViewModels;

namespace MealMe.Views;

public partial class UISettings : ContentPage
{
	public UISettings(UISettingsVM uISettingsVM)
	{
		InitializeComponent();
		BindingContext = uISettingsVM;
	}
}