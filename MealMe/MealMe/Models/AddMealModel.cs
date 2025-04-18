// AddMeal Model for defining the class used for searching for meals. Stores the ID and Name of the meal \\
namespace MealMe.Models;

public class AddMealModel
{
    // MealID \\
    public required int Id { get; set; }
    // Meal Name \\
    public required string Name { get; set; }
}
