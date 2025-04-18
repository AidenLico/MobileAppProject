using MealMe.ViewModels;

namespace MealMe.Views;

public partial class CloudShoppingList : ContentPage
{
	public CloudShoppingList(CloudShoppingListVM cloudShoppingListVM)
	{
		InitializeComponent();
        BindingContext = cloudShoppingListVM;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        CloudShoppingListVM cloudShoppingListVM = (CloudShoppingListVM)BindingContext;
        await cloudShoppingListVM.SetColour();
        await cloudShoppingListVM.GetList();
    }
}