// View Model for Dashboard Page \\

// Required packages \\
using System.Text.Json;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

// USes dahsboard model and meal plan model \\
using MealMe.Models;
// uses spoonacular service and database service \\
using MealMe.Services;
// USes converter for UI preferences \\
using MealMe.Converters;


namespace MealMe.ViewModels;

public partial class DashboardVM : BaseVM
{
    // Observable collection for storing info of a meal \\
    public ObservableCollection<DashboardModel> Info { get; set; }
    // Create services \\
    private readonly SpoonacularService spoonacularService;
    private readonly DatabaseService databaseService;

    // Create variables to store the IDs \\
    private int BreakfastID;
    private int LunchID;
    private int DinnerID;
    private int SnackID;

    // Constructor \\
    public DashboardVM()
    {
        // Initialise services \\
        spoonacularService = new SpoonacularService();
        databaseService = new DatabaseService();

        // Set IDs as default 0 \\
        BreakfastID = 0;
        LunchID = 0;
        DinnerID = 0;
        SnackID = 0;

        // Initialise the observable collection \\
        Info = new ObservableCollection<DashboardModel>();

    }

    // Observable properties \\
    // Stores the breakfast name \\
    [ObservableProperty]
    private string breakfast;
    // Stores the lunch name \\
    [ObservableProperty]
    private string lunch;
    // Stores the dinner name \\
    [ObservableProperty]
    private string dinner;
    // Stores the snakc name \\
    [ObservableProperty]
    private string snack;

    // Stores UI colours \\
    [ObservableProperty]
    private string menuColour;
    [ObservableProperty]
    private string buttonColour;
    [ObservableProperty]
    private string backgroundColour;

    // Method SetColour updates UI colours based on preference \\
    public async Task SetColour()
    {
        var colour = Preferences.Get("Colour", "Green");
        var converter = new ColourToColourListConverter();
        var result = (ColourModel)converter.Convert(colour.ToString(), typeof(ColourModel), null, null);
        ButtonColour = result.Button;
        MenuColour = result.Menu;

        var style = Preferences.Get("Style", "Light");
        var styleConverter = new StyleToBackgroundColourConverter();
        var styleResult = (string)styleConverter.Convert(style.ToString(), typeof(string), null, null);
        BackgroundColour = styleResult;
    }

    // Method gets the meals stored in the database entry for today \\
    public async Task GetTodayMeals()
    {
        // Get todays date \\
        DateTime today = DateTime.Today;
        // Stores the result from the database service getting database entry for today  \\
        MealPlanModel todayMeals = await databaseService.GetDateReturn(today.ToString("yyyy-MM-dd"));
        // Set IDs as the IDs from result \\
        BreakfastID = todayMeals.BreakfastID;
        LunchID = todayMeals.LunchID;
        DinnerID = todayMeals.DinnerID;
        SnackID = todayMeals.SnackID;
        // Set NAmes as the names from result \\
        Breakfast = todayMeals.Breakfast;
        Lunch = todayMeals.Lunch;
        Dinner = todayMeals.Dinner;
        Snack = todayMeals.Snack;
    }

    // Command for getting more information on a meal \\
    [RelayCommand]
    private async void MoreInfo(string type)
    {
        // Create strings to hold the current meal name and current meal ID \\
        string CurrentMeal;
        int CurrentID;
        // Determines which meal and id to use and store under current meal and current id \\
        if (type == "Breakfast")
        {
            CurrentMeal = Breakfast;
            CurrentID = BreakfastID;
        }
        else if (type == "Lunch")
        {
            CurrentMeal = Lunch;
            CurrentID = LunchID;
        }
        else if (type == "Dinner")
        {
            CurrentMeal = Dinner;
            CurrentID = DinnerID;
        }
        else if (type == "Snack")
        {
            CurrentMeal = Snack;
            CurrentID = SnackID;
        }
        // IF no parameter, return \\
        else
        {
            return;
        }
        // Clear information observable collection \\
        Info.Clear();
        // Store the response from the spoonacular service request \\
        string response = await spoonacularService.GetMealInfo(CurrentID);
        // Convert json string to json dcoument \\
        var content = JsonDocument.Parse(response);
        // get the body from the json document \\
        var root = content.RootElement;

        // Try extract the necessary properties as Json Elements \\
        if (root.TryGetProperty("servings", out JsonElement servingsJson) &&
            root.TryGetProperty("readyInMinutes", out JsonElement readyJson) &&
            root.TryGetProperty("cookingMinutes", out JsonElement cookingJson) &&
            root.TryGetProperty("preparationMinutes", out JsonElement preparationJson) &&
            root.TryGetProperty("spoonacularSourceUrl", out JsonElement recipeJson) &&
            root.TryGetProperty("pricePerServing", out JsonElement priceJson))
        {

            // Create strings to store the found JsonPRoperties \\
            string Servings;
            string Ready;
            string Cooking;
            string Preparation;
            string Recipe;
            string Price;

            // Tries to convert the JsonElements to the correct type, sets as unknown if fails \\
            // Done this way as some recipe information dont include all information causing a break \\
            try
            {
                Servings = servingsJson.GetInt32().ToString();
            } catch
            {
                Servings = "Unknown";
            }

            try
            {
                Ready = readyJson.GetDouble().ToString();
            }
            catch
            {
                Ready = "Unknown";
            }

            try
            {
                Cooking = cookingJson.GetDouble().ToString();
            }
            catch
            {
                Cooking = "Unknown";
            }

            try
            {
                Preparation = preparationJson.GetDouble().ToString();
            }
            catch
            {
                Preparation = "Unknown";
            }

            try
            {
                Recipe = recipeJson.GetString() ?? "Unknown";
            }
            catch
            {
                Recipe = "Unknown";
            }

            try
            {
                Price = priceJson.GetDouble().ToString();
            }
            catch
            {
                Price = "Unknown";
            }

            // Create the dahsboard model with the extracted information \\
            DashboardModel foundInformation = new DashboardModel
            {
                CurrentMeal = CurrentMeal,
                Servings = Servings,
                Ready = Ready,
                Cooking = Cooking,
                Preparation = Preparation,
                Recipe = Recipe,
                Price = Price
            };
            // Add to observable collection \\ 
            Info.Add(foundInformation);
        }
    }
}
