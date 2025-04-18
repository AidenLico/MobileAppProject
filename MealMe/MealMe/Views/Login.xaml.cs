using MealMe.ViewModels;

namespace MealMe.Views;

public partial class Login : ContentPage
{
	public Login(LoginVM loginVM)
	{
		InitializeComponent();
		// Sets the binding context to the view model.
		BindingContext = loginVM;
	}
}