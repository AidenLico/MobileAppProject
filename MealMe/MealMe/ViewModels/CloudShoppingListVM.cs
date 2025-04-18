// View Model for Cloud SHopping List Page \\

// Required packages \\
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

// USes converter to convert UI preference to colour hexes \\
using MealMe.Converters;
// Uses firebaseFirestore service \\
using MealMe.Services;
// Uses shopping list model \\
using MealMe.Models;

namespace MealMe.ViewModels;

public partial class CloudShoppingListVM : BaseVM
{
    // Observable collection for storing the shopping list models for UI \\
    public ObservableCollection<ShoppingListModel> CloudList { get; set; }
    // Create firebasefirestore service \\
    private readonly FirebaseFirestoreService firebaseFirestoreService;

    // COnstructor \\
    public CloudShoppingListVM()
    {
        // Initialise Observable collection \\
        CloudList = new ObservableCollection<ShoppingListModel>();
        // Initialise service \\
        firebaseFirestoreService = new FirebaseFirestoreService();
    }

    // Observable properties \\
    // For background Colour UI \\
    [ObservableProperty]
    public string backgroundColour;

    // Method for setting the colours of the UI based on preference \\
    public async Task SetColour()
    {
        var style = Preferences.Get("Style", "Light");
        var styleConverter = new StyleToBackgroundColourConverter();
        var styleResult = (string)styleConverter.Convert(style.ToString(), typeof(string), null, null);
        BackgroundColour = styleResult;
    }

    // Method Gets the list from the cloud \\
    public async Task GetList()
    {
        // Clear observable collection \\
        CloudList.Clear();
        // Store the list fo shopping list models returned from the cloud service \\
        List<ShoppingListModel> foundList = await firebaseFirestoreService.GetList();
        // For each shopping list model found, add to observable collection \\
        foreach (ShoppingListModel ingredient in foundList)
        {
            CloudList.Add(ingredient);
        }
    }

    // Command for deleting the preference containing the name of the cloud document \\
    [RelayCommand]
    private async Task Delete()
    {
        Preferences.Set("listname", null);
        await GetList();
    }
}
