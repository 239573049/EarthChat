using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Chat.Client.Services;

public class StorageService
{
    private readonly Dictionary<string, object> _items = new(50);

    public void SetToken(string token)
    {
        _items.TryAdd("token", token);
        Save();
    }

    public string GetToken()
    {
        _items.TryGetValue("token", out var token);
        return token?.ToString() ?? string.Empty;
    }

    private void Save()
    {
        try
        {
            // 系统Data目录
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Chat",
                "storage.json");
            var info = new FileInfo(path);
            if (info.Directory?.Exists == false)
            {
                info.Directory.Create();
            }

            File.WriteAllText(path, JsonSerializer.Serialize(_items));
        }
        catch (Exception e)
        {
            MainAppHelper.Logger().LogError("保存Storage失败 Message:{e}", e);
        }
    }
}