// UserSettings View Model

// Required Packages \\
using System.Text.Json;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
// Uses firebase service \\
using MealMe.Services;
// Uses authentication model \\
using MealMe.Models;
// Uses UI converters \\
using MealMe.Converters;

namespace MealMe.ViewModels;

// UserSettings View Model Class, inher
public partial class UserSettingsVM : BaseVM
{
    // Create firebase service \\
    private readonly FirebaseService firebaseService;

    // Constructor \\
    public UserSettingsVM()
    {
        // Initialises firebase service \\
        firebaseService = new FirebaseService();
        // Calls GetEmail so that Email binding displays the users email address \\
    }

    //Observable properties 
    // Stores input password \\
    [ObservableProperty]
    private string password;
    // Stores user new password \
    [ObservableProperty]
    private string newPassword;
    // Holds any UI alerts \\
    [ObservableProperty]
    private string passwordAlert;
    [ObservableProperty]
    private string emailAlert;
    [ObservableProperty]
    private string email;

    // UI colours \\
    [ObservableProperty]
    private string buttonColour;
    [ObservableProperty]
    private string backgroundColour;

    // Sets UI colours \\
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
    // Method gets user's email \\
    public async Task GetEmail()
    {
        // Gets the current users idToken from preferences \\
        string idToken = Preferences.Get("idToken", null);
        // If idToken an empty string or null display UI alert \\
        if (string.IsNullOrEmpty(idToken))
        {
            EmailAlert = "Error Getting Email";
        }
        // Stores the response from getting user info \\
        string response = await firebaseService.GetUser(idToken);
        // converts json string to json document \\
        var content = JsonDocument.Parse(response);
        // get json document body \\
        var root = content.RootElement;
        // Tries to get the property users from response \\
        if (root.TryGetProperty("users", out JsonElement usersJson))
        {
            // Gets first user from array \\
            JsonElement user = usersJson[0];
            // tries to get the property email from the user Json Element
            if (user.TryGetProperty("email", out JsonElement emailJson))
            {
                // Convert email property to string \\
                string email = emailJson.GetString();
                // Sets UI email as the found email \\
                Email = email;
            }
        } else
        {
            // If any of the properties cannot be acccessed, display error \\
            EmailAlert = "Error Getting Email";
        }
    }


    // Method checks password input meet requirements \\
    private bool PasswordCheck()
    {
        // Password Regex, needs atleast 8 characters, atleast 1 capital, number and symbol \\
        string passwordRegex = @"^(?=.*[A-Z])(?=.*\d)(?=.*[\W]).{8,}$";
        // If password inputs empty , display UI alert and return false \\
        if (string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(NewPassword))
        {
            PasswordAlert = "Please Fill Both Fields.";
            return false;
        }
        // If either password doesnt meet password Regex, display alert and return false \\
        if (!Regex.IsMatch(Password, passwordRegex) || !Regex.IsMatch(NewPassword, passwordRegex)) {
            PasswordAlert = "Please ensure password 8 characters or more, and includes atleast 1 capital letter, 1 number and 1 symbol.";
            return false;
        }
        // If passwords meet checks, return true \\
        return true;
    }

    // Command for chnaging user password \\
    [RelayCommand]
    private async Task ChangePassword()
    {
        // Check passwords are valid \\
        bool checkresult = PasswordCheck();
        // If valid \\
        if (checkresult)
        {
            // Create authentication model for firebase service \\
            AuthenticationModel user = new AuthenticationModel
            {
                Email = Email,
                Password = Password
            };
            // Get response from login with the details \\
            string loginResponse = await firebaseService.Login(user);
            // convert json string to json document \\
            var loginResponseContent = JsonDocument.Parse(loginResponse);
            // get json document body \\
            var loginRoot = loginResponseContent.RootElement;
            // Try get the error property (only sent if login failed) \\
            if (loginRoot.TryGetProperty("error", out JsonElement errorJson))
            {
                // Try get the message property from the errorJson \\
                if (errorJson.TryGetProperty("message", out JsonElement messageJson))
                {
                    // Convert error message to string \\
                    string message = messageJson.ToString();
                    // If the error is because of invalid login credentials \\
                    if (message == "INVALID_LOGIN_CREDENTIALS")
                    {
                        // Display incorrect details UI alert \\
                        PasswordAlert = "Error: Email and Password don't match.";
                        // return \\
                        return;
                    }
                }
            }
            else
            {
                // Try get idToken property from login response \\
                if (loginRoot.TryGetProperty("idToken", out JsonElement idTokenJson))
                {
                    // Convert idToken to string \\
                    string idToken = idTokenJson.ToString();
                    // Get response from password change \\
                    string passwordResponse = await firebaseService.ChangePassword(idToken, NewPassword);
                    // Convert json string to json document \\
                    var passwordResetContent = JsonDocument.Parse(passwordResponse);
                    // Get json document body \\
                    var passwordResetRoot = passwordResetContent.RootElement;
                    // Try get idToken again (indicates password changed \\
                    if (passwordResetRoot.TryGetProperty("idToken", out JsonElement idTokenJson2nd))
                    {
                        // Convert new idTOken to string \\
                        string idToken2nd = idTokenJson2nd.ToString();
                        // Set new idToken as the idToken in preferences \\
                        Preferences.Set("idToken", idToken2nd);
                        // Remove values from input fields \\
                        Password = "";
                        NewPassword = "";
                        // Alert user that it was a success! \\
                        await App.Current.MainPage.DisplayAlert("Success", "Password Successfully Changed", "OK");
                        // remove any UI alerts \\
                        PasswordAlert = "";
                        // return to prevent further execution
                        return;
                    } else
                    {
                        // If the idToken could not be gathered again \\
                        PasswordAlert = "An error occured, please try again.";
                    }
                } else
                {
                    // If the idToken could not be gathered \\
                    PasswordAlert = "An error occurred, please try again.";
                }
            }

        }
    }

    // Logout command clears users token and refreshtoken \\
    [RelayCommand]
    private async Task Logout()
    {
        // Clear idToken and refreshToken \\
        Preferences.Clear();
        // Alerts user they have logged out \\
        await App.Current.MainPage.DisplayAlert("Logged Out", "Your account is now logged out.", "OK");
        // Redirects to dashboard \\
        await Shell.Current.GoToAsync("//Dashboard");
        return;
    }
}


