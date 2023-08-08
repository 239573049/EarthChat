namespace Chat.Client.Helpers;

public class PathIconHelper
{
    public static void Path_OnPointerEntered(object? sender, PointerEventArgs e)
    {
        if (sender is PathIcon icon)
        {
            // 修改背景色为蓝色
            icon.Foreground = new SolidColorBrush(Color.Parse(ColorConstant.PathSelectColor));
        }
    }


    public static void Path_OnPointerExited(object? sender, PointerEventArgs e)
    {
        if (sender is PathIcon icon)
        {
            // 修改背景色为ColorConstant.PathDefaultColor;
            icon.Foreground = new SolidColorBrush(Color.Parse(ColorConstant.PathDefaultColor));
        }
    }
}