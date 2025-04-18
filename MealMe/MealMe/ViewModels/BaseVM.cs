// Base View Model, for inheriting to adhere with DRY principle \\

// Required PAckagae
using CommunityToolkit.Mvvm.ComponentModel;

namespace MealMe.ViewModels;

public class BaseVM : ObservableObject
{
    // When viewmodel appears
    public virtual Task OnAppearing()
    {
        // Return completed
        return Task.FromResult(Task.CompletedTask);
    }
    //When viewmodel disappears
    public virtual Task OnDisappearing()
    {
        // Return Completed
        return Task.FromResult(Task.CompletedTask);
    }
}
