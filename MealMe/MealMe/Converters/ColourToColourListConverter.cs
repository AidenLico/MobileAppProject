// Converter for converting a colour (string) to a colour model (ColourModel) \\
// Converter based on the App_Demos converter \\


// Used for culture info for Convert method \\
using System.Globalization;
// Use models as uses the ColourModel \\
using MealMe.Models;

namespace MealMe.Converters;

public class ColourToColourListConverter : IValueConverter
{
    // Convert method takes the value to be converted, and the target output type. parameter and culture should be set to null on call of this \\
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Accesses string as value is an object, stores this string. Defaults to Green \\
        string currentValue = (string)value ?? "Green";
        // Returns the correct ColourModel based on the passed value \\
        return currentValue switch
        {
            "Red" => new ColourModel
            {
                Shell = "#ff6666",
                Menu = "#ff3333",
                Button = "#ff9999"
            },
            "Blue" => new ColourModel
            {
                Shell = "#99b3ff",
                Menu = "#4d79ff",
                Button = "#b3c6ff"
            },
            "Green" => new ColourModel
            {
                Shell = "#b4d5b4",
                Menu = "#cdffcc",
                Button = "#9bff99"
            },
            "Yellow" => new ColourModel
            {
                Shell = "#ffff66",
                Menu = "#ffff33",
                Button = "#ffff99"
            }
        };
    }

    // Does not require convert back \\
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
