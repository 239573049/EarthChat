using System.Globalization;
using System.IO;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Chat.Client.ViewModels;

namespace Chat.Client.Converter;

public class Base64Converter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is EditorModel model && model.EditorType == EditorType.Image)
        {
            return new Bitmap(new MemoryStream(System.Convert.FromBase64String(model.Content)));
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}