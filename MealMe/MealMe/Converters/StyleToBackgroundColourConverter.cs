// Converter for converting a style (string) to the correct hex string for that style (string) \\
// Converter based on App_Demos converter \\

// Used for culture infor used by Convert method \\
using System.Globalization;

namespace MealMe.Converters;

public class StyleToBackgroundColourConverter : IValueConverter
{
    // Convert method, takes style as string and converts to the hex value as a string. parameter and culture should be set as null \\
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Extract string from value object. Defaults to "Light" \\
        string currentValue = (string)value ?? "Light";
        // Returns the correct hex string based on value passed \\
        return currentValue switch
        {
            "Dark" => "#737373",
            "Light" => "#d9d9d9"
        };
    }

    // No need for convert back method \\
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
