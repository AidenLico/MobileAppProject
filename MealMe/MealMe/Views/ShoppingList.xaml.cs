using MealMe.ViewModels;

namespace MealMe.Views;

public partial class ShoppingList : ContentPage
{
	public ShoppingList(ShoppingListVM shoppingListVM)
	{
		InitializeComponent();
		BindingContext = shoppingListVM;
	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        ShoppingListVM shoppingListVM = (ShoppingListVM)BindingContext;
        DateTime today = DateTime.Now;
        await shoppingListVM.GetIngredients();
        await shoppingListVM.SetColour();
    }
}