using Chat.Client.Components;
using Chat.Client.ViewModels;

namespace Chat.Client.Views;

public partial class MainWindow : Window
{
    private Message? _message;

    private UserManage? _userManage;

    public MainWindow()
    {
        InitializeComponent();

        // 默认选择MessageBut,背景色为默认色
        MessageBut.Background = new SolidColorBrush(Color.Parse(ColorConstant.DefaultMenuButColor));

        DataContextChanged += ((sender, args) =>
        {
            ListMainPanel.Children.Add(_message ??= new Message()
            {
                DataContext = ViewModel.MessageListViewModel
            });
        });
    }

    private MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext;

    private void MessageBut_OnClick(object? sender, RoutedEventArgs e)
    {
        if (SetButSelect(sender))
        {
            // 设置UserBut背景色为透明
            UserBut.Background = Brushes.Transparent;
            ListMainPanel.Children.Clear();

            ListMainPanel.Children.Add(_message);
        }
    }

    /// <summary>
    ///  设置按钮是否被选中
    /// </summary>
    /// <param name="but"></param>
    private static bool SetButSelect(object but)
    {
        if (but is not Button button) return false;

        // 判断按钮是否选中，如何选中则不做任何操作
        if (!Equals(button.Background, new SolidColorBrush(Color.Parse(ColorConstant.DefaultMenuButColor))))
        {
            button.Background = new SolidColorBrush(Color.Parse(ColorConstant.DefaultMenuButColor));
            return true;
        }

        return false;
    }

    private void UserBut_OnClick(object? sender, RoutedEventArgs e)
    {
        if (SetButSelect(sender))
        {
            // 设置UserBut背景色为透明
            MessageBut.Background = Brushes.Transparent;
            ListMainPanel.Children.Clear();

            ListMainPanel.Children.Add(_userManage ??= new UserManage()
            {
                DataContext = new UserManageViewModel()
            });
        }
        // 设置UserBut背景色为透明
    }
}