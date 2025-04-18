// View Model for Login Page \\

// Required PAckages \\
using CommunityToolkit.Mvvm.Input;
// Uses Authentication model \\
using MealMe.Models;

namespace MealMe.ViewModels;

// Inherits from Authentication Model for signup/login viewmodel shared methods and construction \\
public partial class LoginVM : AuthenticationVM
{
    // Command for logging in \\
    [RelayCommand]
    private async Task LoginUser()
    {

        // Check inputs are valid \\
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
        // Try sign the user in \\
        try
        {
            // Response from firbase service for log in \\
            string response = await firebaseService.Login(user);
            // Extract the result and store the idToken and refreshToken if they exist \\
            string result = ExtractResult(response);
            // If the account does not exist, or password is incorrect for that email \\
            if (result == "INVALID_LOGIN_CREDENTIALS")
            {
                Alert = "Invalid email and password combination.";
                return;
            }
            // If the email format is invalid, should not occur due to input check \\
            else if (result == "INVALID_EMAIL")
            {
                Alert = "Invalid Email";
                return;
            }
            // If there was errors extracting tokens or error message \\
            else if (result == "PARSE_ERROR")
            {
                await App.Current.MainPage.DisplayAlert("Unsuccessful Token", "An error occured, please login.", "OK");
                return;
            }
            // If login successful \\
            else if (result == "SUCCESS")
            {
                // Display popup alert saying successful login \\
                await App.Current.MainPage.DisplayAlert("Successful Login", "Your account was logged in successfully!", "OK");
                // Reset Inputs \\
                Email = "";
                Password = "";
                Alert = "";
                // Navigate to the dashboard \\
                await Shell.Current.GoToAsync("//Dashboard");
                return;
            }
            // Any other cases caught here \\
            else
            {
                Alert = "Unknown Error: Try again.";
                return;
            }
        }
        // If anything fails during login, display fail alert to user. \\
        catch
        {
            await App.Current.MainPage.DisplayAlert("Unsuccessful Login", "An error occurred logging into your account.", "OK");
        }
    }

    // Command for redirecting user if the click Register \\
    [RelayCommand]
    public async Task RegisterRedirect()
    {
        await Shell.Current.GoToAsync("//SignUp");
    }
}
