// View model for shopping list page \\

// Required packages \\
using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

// Uses shopping lust model \\
using MealMe.Models;
// Uses database, spoonacular and database services \\
using MealMe.Services;
// USes UI converters \\
using MealMe.Converters;

namespace MealMe.ViewModels;

public partial class ShoppingListVM : BaseVM
{
    // Create observable collections for Created shopping list of Shopping list model \\
    public ObservableCollection<ShoppingListModel> ShoppingList { get; set; }

    // Create services \\
    private readonly DatabaseService databaseService;
    private readonly SpoonacularService spoonacularService;
    private readonly FirebaseFirestoreService firebaseFirestoreService;

    // Constructor \\
    public ShoppingListVM()
    {
        // Initialise the observable collection \\
        ShoppingList = new ObservableCollection<ShoppingListModel>();
        // Initialise the services \\
        databaseService = new DatabaseService();
        spoonacularService = new SpoonacularService();
        firebaseFirestoreService = new FirebaseFirestoreService();
        // Set the current date week starting date \\
        setDate();
    }

    // Observable properties \\
    // Stores the starting date of week \\
    [ObservableProperty]
    private string date;
    // Stores UI update for loading \\
    [ObservableProperty]
    private string loading;
    // Stores UI alert text \\
    [ObservableProperty]
    private string cloudError;

    // STores UI colours \\
    [ObservableProperty]
    private string buttonColour;
    [ObservableProperty]
    private string backgroundColour;

    // Method sets UI colours \\
    public async Task SetColour()
    {
        var colour = Preferences.Get("Colour", "Green");
        var converter = new ColourToColourListConverter();
        var result = (ColourModel)converter.Convert(colour.ToString(), typeof(ColourModel), null, null);
        ButtonColour = result.Button;

        var style = Preferences.Get("Style", "Light");
        var styleConverter = new StyleToBackgroundColourConverter();
        var styleResult = (string)styleConverter.Convert(style.ToString(), typeof(string), null, null);
        BackgroundColour = styleResult;
    }

    // Method sets the current week starting date \\
    private void setDate()
    {
        DateTime today = DateTime.Today;
        int position = (int)today.DayOfWeek;
        DateTime startDay = today.AddDays(-(position - 1));
        Date = startDay.ToString("yyyy-MM-dd");
    }

    // Method for getting ingredients of the current week meals \\
    public async Task GetIngredients()
    {
        // Clear observable collections \\
        ShoppingList.Clear();
        // Create a dictionary for storing the name of the ingredient as the key, and the shoppuing list model as the value \\
        Dictionary<string, ShoppingListModel> ingredientsDictionary = new Dictionary<string, ShoppingListModel>();
        // Convert the starting date to a datetime object \\
        DateTime currentDate = DateTime.ParseExact(Date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        // Set loading status \\
        Loading = "Loading";
        // For each day of the week \\
        for (int i = 0; i < 7; i++)
        {
            Loading = "Loading.";
            // Get the meal plan for the date \\
            MealPlanModel currentMeal = await databaseService.GetDateReturn(currentDate.ToString("yyyy-MM-dd"));
            // Check the meal IDs, if they are not zero \\
            if (currentMeal.BreakfastID != 0)
            {
                // Get the list of ingredients as shopping list models from spoonacular service \\
                List<ShoppingListModel> ingredients = await spoonacularService.GetIngredients(currentMeal.BreakfastID);
                // for each ingredient \\
                foreach (ShoppingListModel ingredient in ingredients)
                {
                    // Check if the ingredient is already stored in the dictionary (prevents duplicate entries \\
                    if (ingredientsDictionary.ContainsKey(ingredient.IngredientName))
                    {
                        // If the ingredient is already stored, add the amount of the current ingredient to the amount of the stored ingredientn \\
                        ingredientsDictionary[ingredient.IngredientName].Amount += ingredient.Amount;
                    }
                    // If the ingredient doesnt exist add it to the dictionary \\
                    else
                    {
                        ingredientsDictionary.Add(ingredient.IngredientName, ingredient);
                    }
                }
            }
            Loading = "Loading..";
            if (currentMeal.LunchID != 0)
            {
                List<ShoppingListModel> ingredients = await spoonacularService.GetIngredients(currentMeal.LunchID);
                foreach (ShoppingListModel ingredient in ingredients)
                {
                    if (ingredientsDictionary.ContainsKey(ingredient.IngredientName))
                    {
                        ingredientsDictionary[ingredient.IngredientName].Amount += ingredient.Amount;
                    }
                    else
                    {
                        ingredientsDictionary.Add(ingredient.IngredientName, ingredient);
                    }
                }
            }
            Loading = "Loading...";
            if (currentMeal.DinnerID != 0)
            {
                List<ShoppingListModel> ingredients = await spoonacularService.GetIngredients(currentMeal.DinnerID);
                foreach (ShoppingListModel ingredient in ingredients)
                {
                    if (ingredientsDictionary.ContainsKey(ingredient.IngredientName))
                    {
                        ingredientsDictionary[ingredient.IngredientName].Amount += ingredient.Amount;
                    }
                    else
                    {
                        ingredientsDictionary.Add(ingredient.IngredientName, ingredient);
                    }
                }
            }
            Loading = "Loading....";
            if (currentMeal.SnackID != 0)
            {
                List<ShoppingListModel> ingredients = await spoonacularService.GetIngredients(currentMeal.SnackID);
                foreach (ShoppingListModel ingredient in ingredients)
                {
                    if (ingredientsDictionary.ContainsKey(ingredient.IngredientName))
                    {
                        ingredientsDictionary[ingredient.IngredientName].Amount += ingredient.Amount;
                    }
                    else
                    {
                        ingredientsDictionary.Add(ingredient.IngredientName, ingredient);
                    }
                }
            }
            // Add a day to the date before next loop \\
            currentDate = currentDate.AddDays(1);
        }

        // For each shopping list model in the values of the dictionary \\
        foreach (ShoppingListModel ingredient in ingredientsDictionary.Values)
        {
            // Add the shopping list model ingredient to the observable collection \\
            ShoppingList.Add(ingredient);
        }
        // Remove loading status \\
        Loading = "";
    }

    // Command stores list to cloud \\
    [RelayCommand]
    public async Task StoreList()
    {
        // Clear cloud error \\
        CloudError = "";
        // Create a list of shopping list models \\
        List<ShoppingListModel> currentList = new List<ShoppingListModel>();
        // For every model in the observable collection \\
        foreach (ShoppingListModel ingredient in ShoppingList)
        {
            // Add each model to the new list \\
            currentList.Add(ingredient);
        }
        // Await the result of adding to the cloud \\
        await firebaseFirestoreService.CreateList(currentList);

    }

    // Command for going to previous week, takes the Date and goes back 7 and calls GetIngredients \\
    [RelayCommand]
    private async Task PreviousWeek()
    {
        DateTime currentDate = DateTime.ParseExact(Date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        DateTime newDate = currentDate.AddDays(-7);
        Date = newDate.ToString("yyyy-MM-dd");
        await GetIngredients();
    }

    // Command for going to next week, same as above but goes forward 7 days \\
    [RelayCommand]
    private async Task NextWeek()
    {
        DateTime currentDate = DateTime.ParseExact(Date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        DateTime newDate = currentDate.AddDays(7);
        Date = newDate.ToString("yyyy-MM-dd");
        await GetIngredients();
    }
}
