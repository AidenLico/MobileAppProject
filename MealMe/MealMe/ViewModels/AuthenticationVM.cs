// View Model For Authentication Page \\

// Required Packages \\
using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.RegularExpressions;
using System.Text.Json;

// Uses firebase service \\
using MealMe.Services;
// USes authentication model \\
using MealMe.Models;
// Converts UI preference for colour to hex colours \\
using MealMe.Converters;


namespace MealMe.ViewModels;

public partial class AuthenticationVM : BaseVM
{
    // Create firebase service \\
    public readonly FirebaseService firebaseService;

    // Constructor \\
    public AuthenticationVM()
    {
        // Initialise the firebase service \\
        firebaseService = new FirebaseService();
        // Set UI colours \\
        SetColour();
    }

    // Observable properties for UI updates \\
    // stores email input \\
    [ObservableProperty]
    private string email;
    // stores password input \\
    [ObservableProperty]
    private string password;
    // stores auth UI alert \\
    [ObservableProperty]
    private string alert;

    // Colour UI properties \\
    [ObservableProperty]
    private string buttonColour;
    [ObservableProperty]
    private string backgroundColour;

    // Method sets the colours of UI items based on preferences \\
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

    // Method Checks Inputs are Valid \\
    public bool InputCheck()
    {
        // Clear alert \\
        Alert = "";
        // Email regex, checks in format abc@dfg.hij or abc@dfg.hi.jk \\
        string emailRegex = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        // Password regex, checks password 8 characters, includes 1 capital, number and symbol \\
        string passwordRegex = @"^(?=.*[A-Z])(?=.*\d)(?=.*[\W]).{8,}$";
        // If email and password entries empty, set UI alert and return false \\
        if (Email == null || Password == null)
        {
            Alert = "Please ensure you have filled the Email and Password Fields.";
            return false;
        }
        // If email is not valid (does not match regex), set UI alert and return false \\
        if (!Regex.IsMatch(Email, emailRegex))
        {
            Alert = "Invalid email address.";
            return false;
        }
        // If Password does not valid, Set UI alert and return false \\
        if (!Regex.IsMatch(Password, passwordRegex))
        {
            Alert = "Please ensure password 8 characters or more, and includes atleast 1 capital letter, 1 number and 1 symbol.";
            return false;
        }

        // IF all checks pass, return true \\
        return true;
    }

    // Method for extracting authentication response \\
    public string ExtractResult(string response)
    {
        // Convert Json string to JSon document \\
        var content = JsonDocument.Parse(response);
        // Get json document body \\
        var root = content.RootElement;
        // Try get the property error out from the Json documnent \\
        if (root.TryGetProperty("error", out JsonElement errorJson))
        {
            // Extract the error message from the error JsonElement \\
            if (errorJson.TryGetProperty("message", out JsonElement messageJson))
            {
                // Convert error message to string and return this \\
                string message = messageJson.ToString();
                return message;
            }
            else
            {
                // Catch unknown errors \\
                return "UNKNOWN_ERROR";
            }
        }
        // Extract idToken and refreshToken from the response \\
        else if (root.TryGetProperty("idToken", out JsonElement idTokenJson) && root.TryGetProperty("refreshToken", out JsonElement refreshTokenJson))
        {
            // Convert tokens to string \\
            string idToken = idTokenJson.ToString();
            string refreshToken = refreshTokenJson.ToString();
            // Store tokens in preferences \\
            Preferences.Set("idToken", idToken);
            Preferences.Set("refreshToken", refreshToken);

            // Return success code \\
            return "SUCCESS";
        }
        else
        {
            // Return error due to parsing the json \\
            return "PARSE_ERROR";
        }
    }
}
