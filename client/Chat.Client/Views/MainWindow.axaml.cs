using Avalonia.Layout;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using Chat.Client.Components;
using Chat.Client.ViewModels;

namespace Chat.Client.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var eventBus = MainAppHelper.GetRequiredService<IEventBus>();

        eventBus.Subscribe(EventBusConstant.ContentStackPanel, (obj) =>
        {
            if (obj is UserControl control)
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    ContentStackPanel.Children.Clear();
                    ContentStackPanel.Children.Add(control);
                });
            }
            else
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    ContentStackPanel.Children.Clear();
                    ContentStackPanel.Children.Add(new Image()
                    {
                        Source = new Bitmap(AssetLoader.Open(new Uri("avares://Chat.Client/Assets/chat.png"))),
                        VerticalAlignment = VerticalAlignment.Center,
                        Height = 100,
                        Width = 100,
                        HorizontalAlignment = HorizontalAlignment.Center,
                    });
                });
            }
        });
        
        // 默认选择MessageBut,背景色为默认色
        MessageBut.Background = new SolidColorBrush(Color.Parse(ColorConstant.DefaultMenuButColor));
        SelectMessage();

    }

    private MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext;

    private void MessageBut_OnClick(object? sender, RoutedEventArgs e)
    {
        if (SetButSelect(sender))
        {
            SelectMessage();
        }
    }

    private void SelectMessage()
    {
        // 设置UserBut背景色为透明
        UserBut.Background = Brushes.Transparent;
        ListMainPanel.Children.Clear();

        var message = MainAppHelper.GetRequiredService<Message>();
        ListMainPanel.Children.Add(message);

        message.SelectFirst();
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
        if (!SetButSelect(sender)) return;

        // 设置UserBut背景色为透明
        MessageBut.Background = Brushes.Transparent;
        ListMainPanel.Children.Clear();

        
        ListMainPanel.Children.Add(MainAppHelper.GetRequiredService<UserManage>());

        // 重置内容面板
        MainAppHelper.GetRequiredService<IEventBus>().Publish(EventBusConstant.ContentStackPanel, null);

        // 设置UserBut背景色为透明
    }
}