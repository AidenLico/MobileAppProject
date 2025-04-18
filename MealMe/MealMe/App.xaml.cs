using MealMe.Services;
using MealMe.Models;
using MealMe.ViewModels;

namespace MealMe;

public partial class App : Application
{
    public App(DatabaseService databaseService, FirebaseService firebaseService, UISettingsVM uISettingsVM)
    {
        var refreshToken = Preferences.Get("refreshToken", null);
        firebaseService.RefreshToken(refreshToken);

        InitializeComponent();
        uISettingsVM.SetColour();
        BindingContext = uISettingsVM;

    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}