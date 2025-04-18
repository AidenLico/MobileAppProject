// Service for handling firebase authentication \\
// Made use of the following documentation resources \\
// https://learn.microsoft.com/en-us/dotnet/api/system.net.http.json?view=net-9.0 \\
// https://learn.microsoft.com/en-us/dotnet/api/system.text.json?view=net-9.0 \\
// https://firebase.google.com/docs/reference/rest/auth?_gl=1*qt6wmb*_up*MQ..*_ga*MTkzODE4MTk0MS4xNzQ0MjAyMzU1*_ga_CW55HF8NVT*MTc0NDIwMjM1NC4xLjAuMTc0NDIwMjM1NC4wLjAuMA.. \\

// Used for making http requests and receiving responses \\
using System.Net.Http.Json;
// USed for extracting properties from the response \\
using System.Text.Json;
// Uses Authentication model for making requests \\
using MealMe.Models;

namespace MealMe.Services;

public class FirebaseService
{
    // Create client for sending/receiving http messages \\
    private HttpClient httpclient;
    // API key for firebase Auth for this project \\
    // PLEASE FILL AN FIREBASE AUTHENTICATION API KEY
    private const string APIKEY = "";

    // Constructor \\
    public FirebaseService()
    {
        // Initialise the httpclient \\
        httpclient = new HttpClient();
    }

    // Method for registering user, takes an Authentication Model for making request content \\
    public async Task<string> Register(AuthenticationModel user)
    {
        // Endpoint for registering a user \\
        // https://firebase.google.com/docs/reference/rest/auth?_gl=1*qt6wmb*_up*MQ..*_ga*MTkzODE4MTk0MS4xNzQ0MjAyMzU1*_ga_CW55HF8NVT*MTc0NDIwMjM1NC4xLjAuMTc0NDIwMjM1NC4wLjAuMA.. \\
        string url = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={APIKEY}";
        // Create the http request, as a post request to the endpoint \\
        // https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httprequestmessage?view=net-9.0 \\
        HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, url);
        // Converts auth model to JSon Content and add to request\\
        message.Content = JsonContent.Create(user);
        // Receive the http response, awaits the http client sending the request \\
        // https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpresponsemessage?view=net-9.0 \\
        HttpResponseMessage response = await httpclient.SendAsync(message);
        // Returns the response as a string \\
        return await response.Content.ReadAsStringAsync();
    }

    // Method for logging in, takes auth model form making request \\
    public async Task<string> Login(AuthenticationModel user)
    {
        // Endpoint for logging in \\
        // https://firebase.google.com/docs/reference/rest/auth?_gl=1*qt6wmb*_up*MQ..*_ga*MTkzODE4MTk0MS4xNzQ0MjAyMzU1*_ga_CW55HF8NVT*MTc0NDIwMjM1NC4xLjAuMTc0NDIwMjM1NC4wLjAuMA.. \\
        string url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={APIKEY}";
        // Create the http request, as a post request to the endpoint \\
        // https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httprequestmessage?view=net-9.0 \\
        HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, url);
        // Converts user auth model to Json Content and add to request \\
        message.Content= JsonContent.Create(user);
        // Receive the http response, awaits the http client sending the request \\
        // https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpresponsemessage?view=net-9.0 \\
        HttpResponseMessage response = await httpclient.SendAsync(message);
        // Returns Athe response as a string \\
        return await response.Content.ReadAsStringAsync();
    }

    // Method gets user information by idToken \\
    public async Task<string> GetUser(string idToken)
    {
        // Endpoint for getting user data \\
        // https://firebase.google.com/docs/reference/rest/auth?_gl=1*qt6wmb*_up*MQ..*_ga*MTkzODE4MTk0MS4xNzQ0MjAyMzU1*_ga_CW55HF8NVT*MTc0NDIwMjM1NC4xLjAuMTc0NDIwMjM1NC4wLjAuMA.. \\
        string url = $"https://identitytoolkit.googleapis.com/v1/accounts:lookup?key={APIKEY}";
        // Create the http request, as a post request to the endpoint \\
        // https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httprequestmessage?view=net-9.0 \\
        HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, url);
        // Content object including idToken \\
        var postContent = new { 
            idToken = idToken 
        };
        // Convert Object to Json Content and add to the request \\
        message.Content = JsonContent.Create(postContent);
        // Receive the http response, awaits the http client sending the request \\
        // https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpresponsemessage?view=net-9.0 \\
        HttpResponseMessage response = await httpclient.SendAsync(message);
        // Returns the response as string \\
        return await response.Content.ReadAsStringAsync();
    }

    // Method changes password by idToken \\
    public async Task<string> ChangePassword(string idToken, string newPassword)
    {
        // Endpoint for updating user's password \\
        // https://firebase.google.com/docs/reference/rest/auth?_gl=1*qt6wmb*_up*MQ..*_ga*MTkzODE4MTk0MS4xNzQ0MjAyMzU1*_ga_CW55HF8NVT*MTc0NDIwMjM1NC4xLjAuMTc0NDIwMjM1NC4wLjAuMA.. \\
        string url = $"https://identitytoolkit.googleapis.com/v1/accounts:update?key={APIKEY}";
        // Create the http request, as a post request to the endpoint \\
        // https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httprequestmessage?view=net-9.0 \\
        HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, url);
        // Content object including idToken and password, ánd return secure token \\
        var postContent = new
        {
            idToken = idToken,
            password = newPassword,
            returnSecureToken = true
        };
        // Add the content object as Json Content to the http post request \\
        message.Content = JsonContent.Create(postContent);
        // Receive the http response, awaits the http client sending the request \\
        // https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpresponsemessage?view=net-9.0 \\
        HttpResponseMessage response = await httpclient.SendAsync(message);
        // Return the response as a string \\
        return await response.Content.ReadAsStringAsync();
    }

    // Method refreshes idToken with refreshToken \\
    public async void RefreshToken(string refreshToken)
    {
        // Endpoint for getting idToken by refreshToken \\
        string url = $"https://identitytoolkit.googleapis.com/v1/token?key={APIKEY}";
        // Create the http request, as a post request to the endpoint \\
        // https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httprequestmessage?view=net-9.0 \\
        HttpRequestMessage message = new HttpRequestMessage (HttpMethod.Post, url);
        // Create post object, with the grant type and the refresh token \\
        var postContent = new
        {
            grant_type = "refresh_token",
            refresh_token = refreshToken
        };
        // Add post object as Json Content to request \\
        message.Content = JsonContent.Create(postContent);
        // Receive the http response, awaits the http client sending the request \\
        // https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpresponsemessage?view=net-9.0 \\
        HttpResponseMessage response = await httpclient.SendAsync(message);
        // Convert response to string \\
        string responseString = await response.Content.ReadAsStringAsync();
        // Parse the Json string as a Json Document \\
        var content = JsonDocument.Parse(responseString);
        // Get Json Document root (body) \\
        var root = content.RootElement;

        // Try to extract the JsonElement in the "id_token" property \\
        if (root.TryGetProperty("id_token", out JsonElement idTokenJson))
        {
            // Convert the element to string \\
            string idToken = idTokenJson.ToString();
            // Store the idToken in preferences \\
            Preferences.Set("idToken", idToken);
        }
        else
        {
            // Clear preferences if refreshToken invalid \\
            Preferences.Clear();
        }
    }

}
