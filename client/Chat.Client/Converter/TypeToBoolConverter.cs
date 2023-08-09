using System.Globalization;
using Avalonia.Data.Converters;

namespace Chat.Client.Converter;

public class TypeToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is EditorType type && parameter is string param)
        {
            if (param == "Text" && type == EditorType.Text) return true;
            if (param == "Image" && type == EditorType.Image) return true;
        }

        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}