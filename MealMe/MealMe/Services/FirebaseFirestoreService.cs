// Service for handling firebase firestorage \\
// Made use of the following documentation resources \\
//https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-9.0\\
//https://learn.microsoft.com/en-us/dotnet/api/system.text.json?view=net-9.0\\
//https://firebase.google.com/docs/firestore/reference/rest/v1/projects.databases/create?_gl=1*munndj*_up*MQ..*_ga*MTkzODE4MTk0MS4xNzQ0MjAyMzU1*_ga_CW55HF8NVT*MTc0NDIwMjM1NC4xLjAuMTc0NDIwMjM1NC4wLjAuMA..\\
// I also used Gemini AI for determining what content I had to add to my request for my use case. I will specify where this is \\


// Used for making requests receiving responses \\
using System.Net.Http.Json;
// USed for extracting JsonElements from Response \\
using System.Text.Json;
// Used for shopping list model for receiving the shopping list from cloud \\
using MealMe.Models;

namespace MealMe.Services;

public class FirebaseFirestoreService
{
    // Creates client for sending and receiving http requests/responses. \\
    private HttpClient httpclient;

    // Constructor \\
    public FirebaseFirestoreService()
    {
        // Initialise the httpclient \\
        httpclient = new HttpClient();
    }

    // C - rud \\
    // Method for creating a shopping list document in firestore, takes a list of shopping list models and turns each model into an object \\
    public async Task<bool> CreateList(List<ShoppingListModel> ingredients)
    {
        // Endpoint for creating a document \\
        string url = $"https://firestore.googleapis.com/v1/projects/mealme-94939/databases/(default)/documents/shoppinglists";
        // Create List of objects to add to post request \\
        List<object> ingredientsPost = new List<object>();
        // For every model in the ingredients list \\
        foreach (ShoppingListModel ingredient in ingredients)
        {
            // Turn the model into an object with the correct properties \\
            // Gemini helped me to determine exactly what properties i needed \\
            object ingredientObject = new
            {
                mapValue = new
                {
                    fields = new
                    {
                        IngredientName = new
                        {
                            stringValue = ingredient.IngredientName
                        },
                        Amount = new
                        {
                            doubleValue = ingredient.Amount
                        },
                        Measurement = new
                        {
                            stringValue = ingredient.Measurement
                        }
                    }
                }
            };
            // Add ingredients to the list of objects \\
            ingredientsPost.Add(ingredientObject);
        }
        // Create object with the list of objects created \\
        // Gemini helped me to determine exactly what properties i needed for the post request \\
        object postContent = new
        {
            fields = new
            {
                ingredients = new
                {
                    arrayValue = new
                    {
                        values = ingredientsPost
                    }
                }
            }
        };

        // Create the http request, as a popst request to the endpoint \\
        // https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httprequestmessage?view=net-9.0 \\
        HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, url);
        // Add the content to the request, converts to Json Content\\
        // https://learn.microsoft.com/en-us/dotnet/api/system.net.http.json.jsoncontent?view=net-9.0 \\
        message.Content = JsonContent.Create(postContent);
        // Receive the http response, awaits the http client sending the request \\
        // https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpresponsemessage?view=net-9.0 \\
        HttpResponseMessage response = await httpclient.SendAsync(message);

        // Checks if the request was successful \\
        if (response.IsSuccessStatusCode)
        {
            // Convert to a string \\
            string responseString = await response.Content.ReadAsStringAsync();
            // Convert the JSon string to a Json document \\
            // https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsondocument?view=net-9.0 \\
            var content = JsonDocument.Parse(responseString);
            // Gets the Json document root element (content body)
            var root = content.RootElement;

            // attempt to get the property under name as a Json Element \\
            // https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonelement.trygetproperty?view=net-9.0#system-text-json-jsonelement-trygetproperty(system-readonlyspan((system-byte))-system-text-json-jsonelement@) \\
            if (root.TryGetProperty("name", out JsonElement nameJson))
            {
                // COnvert the JsonElement to string
                string namePath = nameJson.ToString();
                // Extract the id from the name (name is the full path to the document created in cloud) \\
                // https://firebase.google.com/docs/firestore/reference/rest/Shared.Types/Operation?_gl=1*vczeeq*_up*MQ..*_ga*MTkzODE4MTk0MS4xNzQ0MjAyMzU1*_ga_CW55HF8NVT*MTc0NDIwMjM1NC4xLjAuMTc0NDIwMjM1NC4wLjAuMA.. \\
                string name = namePath.Substring(namePath.LastIndexOf('/') + 1);
                // Store the name of the cloud stored shopping list in preferences \\
                Preferences.Set("listname", name);
            }
            else
            {
                // If no name property, return fal
                return false;
            }
            return true;
        }
        else
        {
            // If response is not a success status code \\
            return false;
        }
    }

    // c - R - ud \\
    // Method gets the list from the cloud and returns a list of shopping list model (reverses create operation) \\
    public async Task<List<ShoppingListModel>> GetList()
    {
        // Create a list which stores shopping list models \\
        List<ShoppingListModel> shoppingList = new List<ShoppingListModel>();
        // get the list name from preferences \\
        var listname = Preferences.Get("listname", null);
        // if no stored listname \\
        if (listname == null)
        {
            // return empty list (no stored shopping list)
            return shoppingList;
        }
        // Endpoint for accessing the cloud stored document \\
        string url = $"https://firestore.googleapis.com/v1/projects/mealme-94939/databases/(default)/documents/shoppinglists/{listname}";

        // Receive the http response, awaits the http client sending the get request to endpoint \\
        // https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpresponsemessage?view=net-9.0 \\
        HttpResponseMessage response = await httpclient.GetAsync(url);
        // If the response is notr successful \\
        if (!response.IsSuccessStatusCode)
        {
            // return empty list (could not fetch the stored shopping list \\
            return shoppingList;
        }
        // Convert response to a string \\
        string responseString = await response.Content.ReadAsStringAsync();
        // Convert JsonString to JsonDocument \\
        // https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsondocument?view=net-9.0 \\
        var content = JsonDocument.Parse(responseString);
        // Get the root element of the json document (document body) \\
        var root = content.RootElement;

        // Attempts to extract the JsonELements from the rsponse (following structure from create) \\
        // https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonelement?view=net-9.0 \\
        if (root.TryGetProperty("fields", out JsonElement fieldsJson))
        {
            if (fieldsJson.TryGetProperty("ingredients", out JsonElement ingredientsJson))
            {
                if (ingredientsJson.TryGetProperty("arrayValue", out JsonElement arrayValueJson))
                {
                    if (arrayValueJson.TryGetProperty("values", out JsonElement valuesJson))
                    {
                        // For each stored object in the values JsonELement (array) \\
                        foreach (JsonElement ingredient in valuesJson.EnumerateArray())
                        {
                            // Get the mapValue and fields (the values of each ingredient stored here) \\
                            JsonElement map = ingredient.GetProperty("mapValue").GetProperty("fields");
                            // Get the values from each field and add to shopping list model, defaults values to None if not found \\
                            ShoppingListModel model = new ShoppingListModel
                            {
                                IngredientName = map.GetProperty("ingredientName").GetProperty("stringValue").GetString() ?? "None",
                                Amount = map.GetProperty("amount").GetProperty("doubleValue").GetDouble(),
                                Measurement = map.GetProperty("measurement").GetProperty("stringValue").GetString() ?? "None"
                            };
                            // Add the created model to the list \\
                            shoppingList.Add(model);
                        }
                    }
                }
            }
        }
        // Return the list of shopping list models \\
        return shoppingList;
    }
}
