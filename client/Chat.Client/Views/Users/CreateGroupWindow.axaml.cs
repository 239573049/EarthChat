using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Chat.Client.ViewModels.Users;
using Chat.Contracts.Files;

namespace Chat.Client.Views.Users;

public partial class CreateGroupWindow : Window
{
    public CreateGroupWindow()
    {
        InitializeComponent();
    }

    private CreateGroupViewModel ViewModel => DataContext as CreateGroupViewModel;
    
    private async void AccountClick_OnTapped(object? sender, TappedEventArgs e)
    {
        var storage = StorageProvider;
        var file = await storage.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            AllowMultiple = false,
            FileTypeFilter = new FilePickerFileType[]
            {
                new("png"),
                new("jpg"),
                new("jpeg")
            }
        });

        if (file.Count == 0) return;

        var filePath = file.FirstOrDefault();

        // 转换base64 
        var bytes = await filePath!.OpenReadAsync();

        var buffer = new byte[bytes.Length];
        _ = await bytes.ReadAsync(buffer, 0, (int)bytes.Length);
        var base64 = Convert.ToBase64String(buffer);
        var fileService = MainAppHelper.GetRequiredService<IFileService>();

        var result = await fileService.UploadBase64Async(new UploadBase64Dto()
        {
            FileName = filePath.Name,
            Value = base64
        });
        ViewModel.CreateGroupDto.Avatar = result.Data;
    }
}