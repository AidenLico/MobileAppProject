// View Model for the AddMeal Page \\

// Required packages \\
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Net;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

// Uses AddMealModel \\
using MealMe.Models;
// Uses spoonacular service and database service \
using MealMe.Services;
// Used for converting colour and style preference for UI \\
using MealMe.Converters;

namespace MealMe.ViewModels;

public partial class AddMealVM : BaseVM
{
    // Observable Collection stores the found meals from search \\
    public ObservableCollection<AddMealModel> FoundMeals { get; set; }
    // Observable collection stores the meal types \\
    public ObservableCollection<string> MealType { get; set; }
    // Create the spoonacular service \\
    private readonly SpoonacularService spoonacularService;
    // Create the database service \\
    private readonly DatabaseService databaseService;

    // Constructor \\
    public AddMealVM()
    {
        // Initialise Observable collections \\
        FoundMeals = new ObservableCollection<AddMealModel>();
        MealType = new ObservableCollection<string>
        {
            "breakfast","lunch","dinner","snack"
        };
        // Initilaise the services \\
        spoonacularService = new SpoonacularService();
        databaseService = new DatabaseService();
        // Set day sets the current day to the date select in UI \\
        SetDay();
    }

    // Observable properties for UI updates \\
    // Holds the search query \\
    [ObservableProperty]
    private string query;
    // Holds selected date \\
    [ObservableProperty]
    private DateTime date;
    // Holds errors for search \\
    [ObservableProperty]
    private string searchAlert;
    // Holds errors for results \\
    [ObservableProperty]
    private string resultsAlert;
    // Holds the ID of meal selected \\
    [ObservableProperty]
    private string selectedId;
    // Holds the Name of the meal seelected \\
    [ObservableProperty]
    private string selectedName;
    // Holds the type selected \\
    [ObservableProperty]
    private string selectedType;
    // Holds alerts from adding to meal plan \\
    [ObservableProperty]
    private string addAlert;

    // UI properties \\
    [ObservableProperty]
    private string buttonColour;
    [ObservableProperty]
    private string backgroundColour;

    // Gets the users UI preferences, converts, and sets the UI properties to the correct colours \\
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

    // Gets the day today and sets this to the Date property \\
    public void SetDay()
    {
        DateTime today = DateTime.Today;
        Date = today;
    }

    // Method extracts the results from spoonacular search into a list of AddMealModels \\
    private List<AddMealModel> ExtractResults(string response)
    {
        // Create the list \\
        List<AddMealModel> results = new List<AddMealModel>();
        // Convert the Json string to a Json Document \\
        var content = JsonDocument.Parse(response);
        // Get the body of the Json Document \\
        var root = content.RootElement;

        // Try extarct the results from the response \\
        if (root.TryGetProperty("results", out JsonElement resultsJson))
        {
            // For every result from the search \\
            foreach (var result in resultsJson.EnumerateArray())
            {
                // Create a add meal model with the result name and id \\
                var model = new AddMealModel
                {
                    Id = result.GetProperty("id").GetInt32(),
                    Name = result.GetProperty("title").GetString() ?? "None"
                };
                // Add to list \\
                results.Add(model);
            }
        } else
        {
            // Display alert in UI if search extarction failed \\
            ResultsAlert = "Extraction Error.";
        }
        // Return list of results \\
        return results;
    }


    // Command for Search \\
    [RelayCommand]
    private async Task Search()
    {
        // Clear the list of search results \\
        FoundMeals.Clear();
        // Clear alerts \\
        ResultsAlert = "";
        SearchAlert = "";
        // Regex for search (only letters and spaces) \\
        string searchRegex = @"^[a-zA-Z ]*$";
        // If no search input, display UI alert \\
        if (string.IsNullOrEmpty(Query))
        {
            SearchAlert = "Search is empty.";
            return;
        } 
        // If search does not match Regex, display UI alert \\
        else if (!Regex.IsMatch(Query, searchRegex))
        {
            SearchAlert = "Ensure search is only letters and spaces.";
            return;
        }

        // Try search and extract \\
        try
        {
            // Encode URL so spaces become %20 \\
            // https://learn.microsoft.com/en-us/dotnet/api/system.net.webutility.urlencode?view=net-9.0 \\
            string apiQuery = WebUtility.UrlEncode(Query); 
            // Await the result of search from spoonacular service \\
            string response = await spoonacularService.SearchMeals(apiQuery);
            // Gets the result from the API response \\
            List<AddMealModel> results = ExtractResults(response);
            // If results \\
            if (results.Count > 0)
            {
                // For every result in the list 
                foreach (AddMealModel result in results)
                {
                    // Add it to the Observable collection \\
                    FoundMeals.Add(result);
                }
            } else
            {
                // If no results, display UI alert \\
                ResultsAlert = "No results.";
            }
        } catch
        {
            // Catch search/extraction errors, display UI alert \\
            ResultsAlert = "Error getting results";
        }
    }

    // Command for selecting the meal from serach results \\
    [RelayCommand]
    private async Task Select(AddMealModel selectedMeal)
    {
        // Sets the Selected properties as the selected meal \\
        SelectedId = selectedMeal.Id.ToString();
        SelectedName = selectedMeal.Name;
        return;
    }

    // Command for adding meal to meal plan \\
    [RelayCommand]
    private async Task Add()
    {
        // Result stored from adding/updating meal plan entry at date \\
        bool result = await databaseService.DirectItem(SelectedType, Date.ToString("yyyy-MM-dd"), SelectedName, Convert.ToInt32(SelectedId));
        // If successful, display a popup alert to say it succeeded \\
        if (result == true)
        {
            await App.Current.MainPage.DisplayAlert("Success", "Added your meal to the plan.", "OK");
        } 
        // If unsuccessful, display UI alert \\
        else
        {
            AddAlert = "Error";
        }
    }

}
