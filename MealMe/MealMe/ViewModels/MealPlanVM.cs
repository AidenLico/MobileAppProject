// View Model for Meal plan page \\

// Required Packages \\
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Globalization;
// Uses MealPlanModel \\
using MealMe.Models;
// Uses database service \\
using MealMe.Services;
// Uses UI converters \\
using MealMe.Converters;

namespace MealMe.ViewModels;

public partial class MealPlanVM : BaseVM
{
    // Create observable collection for storing meal plan results \\
    public ObservableCollection<MealPlanModel> MealPlans { get; set; }
    // Create database service \\
    private readonly DatabaseService databaseService;

    // constructor \\
    public MealPlanVM()
    {
        // Initialise the observable collection \\
        MealPlans = new ObservableCollection<MealPlanModel>();
        // Initialise the database service \\
        databaseService = new DatabaseService();
    }

    // Observable Properties \\
    // Stores the Date \\
    [ObservableProperty]
    private string date;

    // Stores UI colours \\
    [ObservableProperty]
    private string buttonColour;
    [ObservableProperty]
    private string menuColour;
    [ObservableProperty]
    private string backgroundColour;

    // Method for setting UI colours \\
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
    // Method gets the meal plans for this week takes a DateTime input\\
    public async Task GetMealPlans(DateTime startDay)
    {
        // Clears the observable collection \\
        MealPlans.Clear();
        Date = startDay.ToString("yyyy-MM-dd");
        // Iterates through the week days starting from today \\
        for (int i = 0; i < 7; i++)
        {
            // Gets the date as a string and adds how many days through the loop (0,1,2,..., 6)\\
            string date = startDay.AddDays(i).ToString("yyyy-MM-dd");
            // Gets result from database service at the date \\
            MealPlanModel meal = await databaseService.GetDateReturn(date);
            // Stores the Meal plan in the Observable collection \\
            MealPlans.Add(meal);
        }
    }

    // Command for going back to the previous week meal plans \\
    [RelayCommand]
    private async Task PreviousWeek()
    {
        // Clear observable collection \\
        MealPlans.Clear();
        // Parses the datetime string to a Datetime object \\
        DateTime dateObject = DateTime.ParseExact(Date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        // GO back a week \\
        dateObject = dateObject.AddDays(-7);
        // Store new starting date in Date \\
        Date = dateObject.ToString("yyyy-MM-dd");
        // Iterate for each day of the week starting the new date \\
        for (int i = 0; i < 7; i++)
        {
            // Gets the current date of the iteration \\
            string date = dateObject.AddDays(i).ToString("yyyy-MM-dd");
            // Get result from the database service at the date \\
            MealPlanModel meal = await databaseService.GetDateReturn(date);
            // Add the entry to the observable collection \\
            MealPlans.Add(meal);
        }
    }

    // Next week command, does same as above but goes to next week \\
    [RelayCommand]
    private async Task NextWeek()
    {
        MealPlans.Clear();
        DateTime dateObject = DateTime.ParseExact(Date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        dateObject = dateObject.AddDays(7);
        Date = dateObject.ToString("yyyy-MM-dd");
        for (int i = 0; i < 7; i++)
        {
            string date = dateObject.AddDays(i).ToString("yyyy-MM-dd");
            MealPlanModel meal = await databaseService.GetDateReturn(date);
            MealPlans.Add(meal);
        }
    }

    // Command removes the meal plan from the database \\
    [RelayCommand]
    private async Task Remove(MealPlanModel selectedMealPlan)
    {
        // Checks if there is an entry at the requested date to remove \\
        bool exists = await databaseService.GetDate(selectedMealPlan.Date);
        // If it doesnt exist, return \\
        if (!exists)
        {
            return;
        } else
        {
            // Clear the observable collection \\
            MealPlans.Clear();
            // Get result from removing from database \\
            bool result = await databaseService.Remove(selectedMealPlan.Date);
            // If remove successful, display popup alert \\
            if (result)
            {
                await App.Current.MainPage.DisplayAlert("Success", "Removed Meals.", "OK");
            }
            // If unsuccessful \\
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", "Error Removing Meals.", "OK");
            }
        }
        // Gets the meal plans for the week \\
        DateTime dateObject = DateTime.ParseExact(Date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        for (int i = 0; i < 7; i++)
        {
            string date = dateObject.AddDays(i).ToString("yyyy-MM-dd");
            MealPlanModel meal = await databaseService.GetDateReturn(date);
            MealPlans.Add(meal);
        }
    }

}
