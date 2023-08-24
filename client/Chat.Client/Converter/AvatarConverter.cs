using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using LiteDB;

namespace Chat.Client.Converter;

public class AvatarConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string str)
        {
            try
            {
                if (str.StartsWith("avares://"))
                {
                    return new Bitmap(AssetLoader.Open(new Uri(str)));
                }

                if (str.StartsWith("base64://"))
                {
                    str = str.Replace("base64://", "");
                    return new Bitmap(new MemoryStream(System.Convert.FromBase64String(str)));
                }

                if (str.StartsWith("http://") || str.StartsWith("https://"))
                {
                    var liteDb = MainAppHelper.GetRequiredService<LiteDatabase>();
                    var avatar = liteDb.GetCollection<AvatarModel>(nameof(AvatarConverter));
                    // 将str转换md5

                    var key = BitConverter
                        .ToString(MainAppHelper.GetRequiredService<MD5>().ComputeHash(Encoding.UTF8.GetBytes(str)))
                        .Replace("-", "");

                    if (avatar.Exists(x => x.Id == key))
                    {
                        // 从数据库中获取文件byte[]
                        var model = avatar.FindById(key);
                        return new Bitmap(new MemoryStream(model.Bytes));
                    }

                    var httpClientFactory = MainAppHelper.GetRequiredService<IHttpClientFactory>();
                    var httpClient = httpClientFactory.CreateClient(nameof(AvatarConverter));
                    var bytes1 = httpClient.GetByteArrayAsync(str).GetAwaiter().GetResult();
                    avatar.Insert(new AvatarModel()
                    {
                        Id = key,
                        Bytes = bytes1,
                        CreatedTime = DateTime.Now
                    });
                    return new Bitmap(new MemoryStream(bytes1));
                }

                return new Bitmap(new MemoryStream(System.Convert.FromBase64String(str)));
            }
            catch (Exception e)
            {
                return value;
            }
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}