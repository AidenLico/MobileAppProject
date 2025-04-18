// Dashboard Model class defines how fetched more information is stored, and accessed by the CollectionView \\

namespace MealMe.Models;

public class DashboardModel
{
    // The name of the current meal \\
    public required string CurrentMeal { get; set; }
    // The servings amount \\
    public required string Servings { get; set; }
    // The time to complete the meal \\
    public required string Ready { get; set; }
    // The cooking time \\
    public required string Cooking { get; set; }
    // The prep time \\
    public required string Preparation { get; set; }
    // The recipe link \\
    public required string Recipe { get; set; }
    // The price per serving \\
    public required string Price { get; set; }
}
