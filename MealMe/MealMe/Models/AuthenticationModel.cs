// Authentication Model used for defining the class used for the login/signup information. This is sent as a request to FireBase Auth \\
namespace MealMe.Models;

public class AuthenticationModel
{
    // User's Email \\
    public required string Email { get; set; }
    // User's Password \\
    public required string Password { get; set; }
    // Bool required to return the tokens when logging in/signing up \\
    public bool returnSecureToken => true;
}
