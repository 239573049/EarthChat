using Avalonia.Media.Transformation;
using Chat.Client.ViewModels;

namespace Chat.Client.Components;

public partial class UserManage : UserControl
{
    public UserManage()
    {
        InitializeComponent();
        DataContextChanged += (sender, args) =>
        {
            ViewModel.Users.Add(new Users()
            {
                Avatar = "",
                Description = "",
                Id = Guid.NewGuid(),
                Name = "测试",
            });
            ViewModel.Users.Add(new Users()
            {
                Avatar = "",
                Description = "",
                Id = Guid.NewGuid(),
                Name = "测试",
            });
            ViewModel.Users.Add(new Users()
            {
                Avatar = "",
                Description = "",
                Id = Guid.NewGuid(),
                Name = "测试",
            });
        };
    }

    private UserManageViewModel ViewModel => (UserManageViewModel)DataContext;


    private void UsePointerEnter(object? sender, PointerEventArgs e)
    {
        if (sender is Panel panel)
        {
            panel.Background = new SolidColorBrush(Color.Parse(ColorConstant.FriendNotification));
        }
    }

    private void UsePointerExited(object? sender, PointerEventArgs e)
    {
        if (sender is not Panel panel) return;

        panel.Background = Brushes.Transparent;
    }

    private void UserManage_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not TextBlock block) return;
        block.Foreground = new SolidColorBrush(Color.Parse("#225EC9"));
        GroupTextBlock.Foreground = new SolidColorBrush(Color.Parse("#94A9A9"));
        BorderBut.IsEnabled = false;
        BorderBut.RenderTransform = TransformOperations.Parse("translateX(0px)");
    }

    private void Group_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not TextBlock block) return;
        block.Foreground = new SolidColorBrush(Color.Parse("#225EC9"));
        UserManageTextBlock.Foreground = new SolidColorBrush(Color.Parse("#94A9A9"));
        BorderBut.IsEnabled = true;
        BorderBut.RenderTransform = TransformOperations.Parse($"translateX({block.Bounds.X - 40}px)");
    }
}