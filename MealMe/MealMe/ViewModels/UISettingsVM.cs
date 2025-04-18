// View model for UI settings page \\

// Required PAckages \\
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

// Uses UI converters \\
using MealMe.Converters;
// Uses Colour model \\
using MealMe.Models;

namespace MealMe.ViewModels;

public partial class UISettingsVM : BaseVM
{
    // Observable collections for colours and style choices \\
    public ObservableCollection<string> Colours { get; set; }
    public ObservableCollection<string> Styles { get; set; }

    // Constructor \\
    public UISettingsVM()
    {
        // Initialise the observable collections with the values to chose from \\
        Colours = new ObservableCollection<string>
        {
            "Red", "Blue", "Green", "Yellow"
        };
        Styles = new ObservableCollection<string>
        {
            "Light", "Dark"
        };
        // Calls set colour \\
        SetColour();
    }

    
    // Observable properties \\
    // Stores the selected colour \\
    [ObservableProperty]
    private string selectedColour;
    // Stores the selected style \\
    [ObservableProperty]
    private string selectedStyle;

    // UI colours \\
    [ObservableProperty]
    public string buttonColour;
    [ObservableProperty]
    public string menuColour;
    [ObservableProperty]
    public string shellColour;
    [ObservableProperty]
    public string backgroundColour;

    // Method set colour gets the colour preferences and sets ui colours \\
    public async Task SetColour()
    {
        // Get colour preference, default green \\
        var colour = Preferences.Get("Colour", "Green");
        
        // Convert colour to a ColourModel with the values for each UI colour \\
        var converter = new ColourToColourListConverter();
        // Get the colour model from the converter result \\
        var result = (ColourModel)converter.Convert(colour.ToString(), typeof(ColourModel), null, null);
        // Set UI colours to the respective colour model colour hex string \\
        ButtonColour = result.Button;
        ShellColour = result.Shell;
        MenuColour = result.Menu;

        // Same as above for style \\
        var style = Preferences.Get("Style", "Light");
        var styleConverter = new StyleToBackgroundColourConverter();
        var styleResult = (string)styleConverter.Convert(style.ToString(), typeof(string), null, null);
        BackgroundColour = styleResult;
    }

    // Command update, sets the new preference of colour and style and calls the set colour method \\
    [RelayCommand]
    private async Task Update()
    {
        Preferences.Set("Colour", SelectedColour);
        Preferences.Set("Style", SelectedStyle);
        await SetColour();
    }
}
