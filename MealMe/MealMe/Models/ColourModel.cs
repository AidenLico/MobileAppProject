// Colour Mdoel class defines the model for how colour hex is stored (from converter) \\

namespace MealMe.Models;

public class ColourModel
{
    // Shell Colour hex \\
    public required string Shell { get; set; }
    // Menu Colour hex \\
    public required string Menu { get; set; }
    // Button Colour hex \\
    public required string Button { get; set; }
}
