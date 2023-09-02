using System;
using System.Collections.Concurrent;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace Chat.Client.Helpers;

public class DragControlHelper
{
    private static readonly ConcurrentDictionary<Control, DragModule> DragModules = new();

    public static void StartDrag(Control userControl)
    {
        DragModules.TryAdd(userControl, new DragModule(userControl));
    }

    public static void StopDrag(Control control)
    {
        if (DragModules.TryRemove(control, out var dragModule))
        {
            dragModule.Dispose();
        }
    }
}

class DragModule : IDisposable
{
    /// <summary>
    /// 记录上一次鼠标位置
    /// </summary>
    private Point? lastMousePosition;

    /// <summary>
    /// 用于平滑更新坐标的计时器
    /// </summary>
    private DispatcherTimer _timer;

    /// <summary>
    /// 标记是否先启动了拖动
    /// </summary>
    private bool isDragging = false;

    /// <summary>
    /// 需要更新的坐标点
    /// </summary>
    private PixelPoint? _targetPosition;

    public Control UserControl { get; set; }

    public DragModule(Control userControl)
    {
        UserControl = userControl;
        // 添加当前控件的事件监听
        UserControl.PointerPressed += OnPointerPressed;
        UserControl.PointerMoved += OnPointerMoved;
        UserControl.PointerReleased += OnPointerReleased;
        
        // 初始化计时器
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(1)
        };
        _timer.Tick += OnTimerTick;
    }


    /// <summary>
    /// 计时器事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnTimerTick(object sender, EventArgs e)
    {
        var window = UserControl.FindAncestorOfType<Window>();
        if (window != null && window.Position != _targetPosition && _targetPosition != null)
        {
            // 更新坐标
            window.Position = (PixelPoint)_targetPosition;
        }
    }

    private void OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(UserControl).Properties.IsLeftButtonPressed) return;
        // 启动拖动
        isDragging = true;
        // 记录当前坐标
        lastMousePosition = e.GetPosition(UserControl);
        e.Handled = true;
        // 启动计时器
        _timer.Start();
    }

    private void OnPointerReleased(object sender, PointerReleasedEventArgs e)
    {
        if (!isDragging) return;
        // 停止拖动
        isDragging = false;
        e.Handled = true;
        // 停止计时器
        _timer.Stop();
    }

    private void OnPointerMoved(object sender, PointerEventArgs e)
    {
        if (!e.GetCurrentPoint(UserControl).Properties.IsLeftButtonPressed) return;

        // 如果没有启动拖动，则不执行
        if (!isDragging) return;

        var currentMousePosition = e.GetPosition(UserControl);
        var offset = currentMousePosition - lastMousePosition.Value;
        var window = UserControl.FindAncestorOfType<Window>();
        if (window != null)
        {
            // 记录当前坐标
            _targetPosition = new PixelPoint(window.Position.X + (int)offset.X,
                window.Position.Y + (int)offset.Y);
        }
    }

    public void Dispose()
    {
        _timer.Stop();
        _targetPosition = null;
        lastMousePosition = null;
    }
}