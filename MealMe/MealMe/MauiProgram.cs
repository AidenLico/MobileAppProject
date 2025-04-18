using MealMe.Services;
using MealMe.ViewModels;
using MealMe.Views;
using Microsoft.Extensions.Logging;

namespace MealMe;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Singletons of each service and view model \\
        // Each service and view model instance looks to the singleton service/viewmodel instead of creating a new one every instance \\
        builder.Services.AddSingleton<FirebaseService>();
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<SpoonacularService>();
        builder.Services.AddSingleton<FirebaseFirestoreService>();
        builder.Services.AddSingleton<AuthenticationVM>();
        builder.Services.AddSingleton<LoginVM>();
        builder.Services.AddSingleton<SignUpVM>();
        builder.Services.AddSingleton<UserSettingsVM>();
        builder.Services.AddSingleton<MealPlanVM>();
        builder.Services.AddSingleton<AddMealVM>();
        builder.Services.AddSingleton<ShoppingListVM>();
        builder.Services.AddSingleton<CloudShoppingListVM>();
        builder.Services.AddSingleton<UISettingsVM>();
        builder.Services.AddSingleton<DashboardVM>();


#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}