// Service for handling Spoonacular Api \\
// Made use of the following documentation resources \\
// https://learn.microsoft.com/en-us/dotnet/api/system.net.http.json?view=net-9.0 \\
// https://learn.microsoft.com/en-us/dotnet/api/system.text.json?view=net-9.0 \\
// https://spoonacular.com/food-api/docs \\

// Used for making http requests/response \\
using System.Net.Http.Json;
// Used for extracting from response \\
using System.Text.Json;
// Used for storing get ingredients results in ShoppingListModel \\
using MealMe.Models;

namespace MealMe.Services;

public class SpoonacularService
{
    // My API Key for spoonacular api \\
    private const string APIKEY = "e3a8bedc72684236aa1763b76f6b5fec";
    // Http client for sending/receiving http messages \\
    private HttpClient httpClient;

    // Constructor \\
    public SpoonacularService()
    {
        // Initialise http client \\
        httpClient = new HttpClient();
    }

    // Method for searching for meals \\
    public async Task<string> SearchMeals(string query)
    {
        // Endpoint for searching for meals \\
        // https://spoonacular.com/food-api/docs#Search-Recipes-Complex \\
        string url = $"https://api.spoonacular.com/recipes/complexSearch?apiKey={APIKEY}&query={query}";
        // Create the http request, as a post request to the endpoint \\
        // https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httprequestmessage?view=net-9.0 \\
        HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, url);
        // Receive the http response, awaits the http client sending the request \\
        // https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpresponsemessage?view=net-9.0 \\
        HttpResponseMessage response = await httpClient.SendAsync(message);
        // Return response as string \\
        return await response.Content.ReadAsStringAsync();
    }

    // Method for returning ingredients of a meal by id \\
    public async Task<List<ShoppingListModel>> GetIngredients(int id)
    {
        // List for adding ShoppingListModels of ingredients \\
        List<ShoppingListModel> ingredients = new List<ShoppingListModel>();

        // Endpoint for getting ingredients \\
        // https://spoonacular.com/food-api/docs#Ingredients-by-ID \\
        string url = $"https://api.spoonacular.com/recipes/{id}/ingredientWidget.json?apiKey={APIKEY}";
        // Create the http request, as a post request to the endpoint \\
        // https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httprequestmessage?view=net-9.0 \\
        HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, url);
        // Receive the http response, awaits the http client sending the request \\
        // https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpresponsemessage?view=net-9.0 \\
        HttpResponseMessage response = await httpClient.SendAsync(message);
        // Convert response to string \\
        string responseString = await response.Content.ReadAsStringAsync();
        // Convert Json string to JSon document \\
        var content = JsonDocument.Parse(responseString);
        // Get document root (body) \\
        var root = content.RootElement;
        
        // Try get the property ingredients from the Json Document \\
        if (root.TryGetProperty("ingredients", out JsonElement ingredientsJson))
        {
            // For every JsonElement in the ingredients array \\
            foreach (JsonElement ingredientJson in ingredientsJson.EnumerateArray())
            {
                // Try extract the values and add them to the shopping list model list \\
                try
                {
                    string name = ingredientJson.GetProperty("name").GetString() ?? "None";
                    JsonElement amount = ingredientJson.GetProperty("amount");
                    JsonElement metric = amount.GetProperty("metric");
                    double value = metric.GetProperty("value").GetDouble();
                    string unit = metric.GetProperty("unit").GetString() ?? "None";

                    ingredients.Add(new ShoppingListModel
                    {
                        IngredientName = name,
                        Amount = value,
                        Measurement = unit
                    });
                } catch
                {
                    continue;
                }
            }
        }
        // Return the list of shopping list models \
        return ingredients;
    }

    // Method gets recipe information by id \\
    public async Task<string> GetMealInfo(int id)
    {
        // Endpoint for getting recipe info by id \\
        // https://spoonacular.com/food-api/docs#Get-Recipe-Information \\
        string url = $"https://api.spoonacular.com/recipes/{id}/information?apiKey={APIKEY}";
        // Create the http request, as a post request to the endpoint \\
        // https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httprequestmessage?view=net-9.0 \\
        HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, url);
        // Receive the http response, awaits the http client sending the request \\
        // https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpresponsemessage?view=net-9.0 \\
        HttpResponseMessage response = await httpClient.SendAsync(message);
        // Return response as string
        return await response.Content.ReadAsStringAsync();
    }
}
