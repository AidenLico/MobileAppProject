// Shopping List Model class defines how data should be stored for found ingredients from the spoonacular api \\

namespace MealMe.Models;

public class ShoppingListModel
{
    // The name of the ingredient \\
    public required string IngredientName { get; set; }
    // The amount of the ingredient \\
    public double Amount { get; set; }
    // The measurement type \\
    public required string Measurement { get; set; }
}
