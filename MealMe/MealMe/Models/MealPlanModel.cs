// Meal Plan Model class used for defining how to access the data from database so it can be accessed throughout code and in UI \\

namespace MealMe.Models;

public class MealPlanModel
{
    // Date of the meals \\
    public required string Date { get; set; }
    // The name of the breakfast \\
    public required string Breakfast { get; set; }
    // The id of breakfast \\
    public required int BreakfastID { get; set; }
    // The name of the lunch \\
    public required string Lunch { get; set; }
    // The id of the lunch \\
    public required int LunchID { get; set; }
    // The name of the dinner \\
    public required string Dinner { get; set; }
    // the id of the dinner \\
    public required int DinnerID { get; set; }
    // the name of the snack \\
    public required string Snack { get; set; }
    // the id of the snack \\
    public required int SnackID { get; set; }

}
