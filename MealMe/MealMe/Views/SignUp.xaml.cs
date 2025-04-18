using MealMe.ViewModels;

namespace MealMe.Views;

public partial class SignUp : ContentPage
{
	public SignUp(SignUpVM signUpVM)
	{
		InitializeComponent();
        BindingContext = signUpVM;
    }
}