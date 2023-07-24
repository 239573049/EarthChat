namespace Chat.Service.Infrastructure.Helper;

public static class StringHelper
{
    private const string Key = "qwertyuiopasdfghjklzxcvbnm1234567890";
    
    public static string RandomString(int size)
    {
        var random = new Random();
        var result = new string(Enumerable.Range(0, size)
            .Select(_ => Key[random.Next(Key.Length)])
            .ToArray());

        return result;
    }

}