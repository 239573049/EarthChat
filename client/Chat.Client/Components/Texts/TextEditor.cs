using Avalonia.Styling;

namespace Chat.Client.Components.Texts;

public sealed class TextEditor : TextBox, IStyleable
{
    public TextEditor()
    {
        TextWrapping = TextWrapping.WrapWithOverflow;
    }

    Type IStyleable.StyleKey => typeof(TextBox);

    public event EventHandler<KeyEventArgs>? BackKeyDown;

    protected override void OnKeyDown(KeyEventArgs e)
    {
        // 如果按下退格键，而文本框的插入符号位置为0
        // 事件不触发?所以我们自己举办活动。
        OnBackKeyDown(e);
        base.OnKeyDown(e);
    }

    private void OnBackKeyDown(KeyEventArgs e)
    {
        BackKeyDown?.Invoke(this, e);
    }
}