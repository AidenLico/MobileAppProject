// View model for sign up page \\

// Required packages \\
using CommunityToolkit.Mvvm.Input;
// Uses authentication model \\
using MealMe.Models;

namespace MealMe.ViewModels;

// Inherits authentication view model for shared constructor and methods \\
public partial class SignUpVM : AuthenticationVM
{
    // Command for registering a user \\
    [RelayCommand]
    private async Task RegisterUser()
    {
        // Checks inputs are valid \\
        bool checkResult = InputCheck();
        // If inputs invalid, return \\
        if (!checkResult)
        {
            return;
        }
        // Creates authentication model with Email and Password from entries \\
        AuthenticationModel user = new AuthenticationModel
        {
            Email = Email,
            Password = Password
        };

        // Try register user \\
        try
        {
            // Awaits the response from the firebase service \\
            string response = await firebaseService.Register(user);
            // Extracts result from response, and sets idTokne and refreshToken as preferences if found \\
            string result = ExtractResult(response);
            // If the email already in use \\
            if (result == "EMAIL_EXISTS")
            {
                Alert = "Email already exists, try logging in.";
                return;
            }
            // If the email is invalid format \\
            else if (result == "INVALID_EMAIL" || result == "MISSING_EMAIL")
            {
                Alert = "Email Invalid";
                return;
            }
            // If password is too weak \\
            else if (result == "WEAK_PASSWORD : Password should be at least 6 characters" || result == "MISSING_PASSWORD")
            {
                Alert = "Invalid Password";
                return;
            }
            // If there was an error getting the Tokens/Error message \\
            else if (result == "PARSE_ERROR")
            {
                await App.Current.MainPage.DisplayAlert("Unsuccessful Token", "An error occured, please login.", "OK");
                return;
            }
            // If account was created \\
            else if (result == "SUCCESS")
            {
                // Display that account created as popup alert \\
                await App.Current.MainPage.DisplayAlert("Successful Registration", "Your account was registered successfully!", "OK");
                // Reset inputs and UI alert \\
                Email = "";
                Password = "";
                Alert = "";
                // Navigate to dashboard \\
                await Shell.Current.GoToAsync("//Dashboard");
                return;
            }
            // Any other case will be caught here \\
            else
            {
                Alert = "Unknown Error: Try again.";
                return;
            }
        }
        // Catches any break errors from registering \\
        catch
        {
            await App.Current.MainPage.DisplayAlert("Unsuccessful Registration", "An error occurred registering your account.", "OK");
        }
    }


    // Command for redirecting to login page if user clicks login \\
    [RelayCommand]
    public async Task LoginRedirect()
    {
        await Shell.Current.GoToAsync("//Login");
    }
}
