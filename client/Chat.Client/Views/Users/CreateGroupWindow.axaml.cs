using System.IO;
using System.Linq;
using Avalonia.Media.Imaging;
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

    private string fileName;
    
    private async void AccountClick_OnTapped(object? sender, TappedEventArgs e)
    {
        var storage = StorageProvider;
        var file = await storage.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            AllowMultiple = false,
        });

        if (file.Count == 0) return;

        var filePath = file.FirstOrDefault();

        // 转换base64 
        var bytes = await filePath!.OpenReadAsync();

        Avatar.Source = new Bitmap(bytes);
        fileName = filePath.Path.AbsolutePath;
        
    }

    private async void Group_OnClick(object? sender, RoutedEventArgs e)
    {
        await using var file = File.OpenRead(fileName);
        var buffer = new byte[file.Length];
        _ = file.Read(buffer, 0, (int)file.Length);
        var base64 = Convert.ToBase64String(buffer);
        var fileService = MainAppHelper.GetRequiredService<IFileService>();
        
        var info = new FileInfo(fileName);
        
        var result = await fileService.UploadBase64Async(new UploadBase64Dto()
        {
            FileName = info.Name,
            Value = base64
        });
        
        var chatService = MainAppHelper.GetRequiredService<IChatService>();
        await chatService.CreateGroupAsync(new CreateGroupDto()
        {
            Avatar = result.Data,
            Name = GroupName.Text,
            Description = GroupDescription.Text
        }, "");
        
    }

    private void OnClose(object? sender, PointerPressedEventArgs e)
    {
        Close();
    }
}