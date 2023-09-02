using Avalonia;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;

namespace Chat.Client.Components.Texts;

/// <summary>
/// A control that manages as the item logic for the <see cref="TokenizingTextBox"/> control.
/// <para>Implements <see cref="Avalonia.Controls.ContentControl" />,
/// Implements <see cref="Avalonia.Controls.ISelectable" /></para>
/// </summary>
/// <seealso cref="Avalonia.Controls.ContentControl" />
/// <seealso cref="Avalonia.Controls.ISelectable" />
[PseudoClasses(":pressed", ":selected")]
public class TokenizingTextBoxItem : ContentControl, ISelectable
{
    private const string PART_BUTTON = "PART_Button";
    private Button? _button;

    /// <summary>
    /// Defines the <see cref="IsSelected"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> IsSelectedProperty =
        AvaloniaProperty.Register<TokenizingTextBoxItem, bool>(nameof(IsSelected));

    /// <summary>
    /// Gets or sets the selection state of the item.
    /// </summary>
    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    static TokenizingTextBoxItem()
    {
        SelectableMixin.Attach<TokenizingTextBoxItem>(IsSelectedProperty);
        PressedMixin.Attach<TokenizingTextBoxItem>();
        FocusableProperty.OverrideDefaultValue<TokenizingTextBoxItem>(true);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_button != null)
        {
            _button.Click -= Button_Click;
        }

        _button = (Button)e.NameScope.Get<Control>(PART_BUTTON);

        if (_button != null)
        {
            _button.Click += Button_Click;
        }
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => IsSelected = true;
}