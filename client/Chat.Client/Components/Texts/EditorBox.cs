using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Controls.Utils;
using Avalonia.Layout;
using Chat.Client.ViewModels;

namespace Chat.Client.Components.Texts;

public class EditorBox : SelectingItemsControl
{
    private const string PART_TEXT_BOX = "PART_TextBox";

    private const string PART_SEARCH_LIST_BOX = "PART_SearchListBox";

    private TextEditor? _textEditor;

    private string _text = string.Empty;

    private string _tempText = string.Empty;

    private CancellationTokenSource? _cancellationTokenSource = new();

    private IEnumerable<string> _searchResults = new AvaloniaList<string>();

    private IEnumerable<string> _searchSource = new AvaloniaList<string>();

    private ISelectionAdapter _adapter;

    private object _selectedSearchResult;

    private static readonly FuncTemplate<Panel> DefaultPanel =
        new(() => new StackPanel { Orientation = Orientation.Horizontal });

    public static readonly DirectProperty<EditorBox, string> TextProperty =
        AvaloniaProperty.RegisterDirect<EditorBox, string>(
            nameof(Items),
            o => o.Text,
            (o, v) => o.Text = v,
            enableDataValidation: true);

    /// <summary>
    /// Defines the <see cref="SearchResults"/> property. 
    /// </summary>
    public static readonly DirectProperty<EditorBox, IEnumerable<string>> SearchResultsProperty =
        AvaloniaProperty.RegisterDirect<EditorBox, IEnumerable<string>>(
            nameof(SearchResults),
            o => o.SearchResults);

    /// <summary>
    /// Defines the <see cref="SearchSource"/> property. 
    /// </summary>
    public static readonly DirectProperty<EditorBox, IEnumerable<string>> SearchSourceProperty =
        AvaloniaProperty.RegisterDirect<EditorBox, IEnumerable<string>>(
            nameof(SearchSource),
            o => o.SearchSource,
            (o, v) => o.SearchSource = v);

    public IEnumerable<string> SearchResults
    {
        get => _searchResults;
        set => SetAndRaise(SearchResultsProperty, ref _searchResults, value);
    }

    public IEnumerable<string> SearchSource
    {
        get => _searchSource;
        set => SetAndRaise(SearchSourceProperty, ref _searchSource, value);
    }

    /// <summary>
    /// Gets or sets the input text.
    /// </summary>
    public string Text
    {
        get => _text;
        set => SetAndRaise(TextProperty, ref _text, value);
    }

    private ObservableCollection<EditorModel> _source = new();

    public ObservableCollection<EditorModel> Source
    {
        get => _source;
        set => SetAndRaise(SourceProperty, ref _source, value);
    }


    public static readonly DirectProperty<EditorBox, ObservableCollection<EditorModel>> SourceProperty =
        AvaloniaProperty.RegisterDirect<EditorBox, ObservableCollection<EditorModel>>(
            nameof(Items),
            o => o.Source,
            (o, v) => o.Source = v,
            enableDataValidation: true);

    static EditorBox()
    {
        ItemsPanelProperty.OverrideDefaultValue<EditorBox>(DefaultPanel);

        TextProperty.Changed.AddClassHandler<EditorBox>((x, e) => x.OnTextPropertyChanged(e));
    }

    private void OnTextPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue != null)
        {
            OnTextChanged((string)e.NewValue);
        }
    }

    private async void OnTextChanged(string searchText)
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
        _cancellationTokenSource = new CancellationTokenSource();

    }

    private Task<List<string>> FilterTextAsync(string searchText, CancellationToken cancellationToken)
    {
        try
        {

            List<string> items = new(SearchSource); //create local copy of SearchSource property.
            List<string> tokens = new((IEnumerable<string>)ItemsSource); //get local list of tokens already added.
            List<string> results = new();


            foreach (var item in items)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                if (item.StartsWith(searchText) && !string.IsNullOrEmpty(searchText) && !tokens.Contains(item))
                {
                    results.Add(item);
                }
            }

            return Task.FromResult(results);
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Task.FromResult(new List<string>());
        }
    }

    private void TextBox_KeyDown(object? sender, KeyEventArgs e)
    {
        int currentCursorPosition = _textEditor.SelectionStart;
        int selectionLength = currentCursorPosition + _textEditor.SelectionEnd;
        switch (e.Key)
        {
            case Key.Left when currentCursorPosition == 0 && selectionLength == 0 && ItemCount > 0:
            case Key.Back when currentCursorPosition == 0 && selectionLength == 0 && ItemCount > 0:
                var container = ItemContainerGenerator.ContainerFromIndex(ItemCount - 1);
                if (container is TokenizingTextBoxItem element)
                {
                    element.Focus();
                    SelectedIndex = ItemCount - 1;
                }

                e.Handled = true;
                break;
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_textEditor != null)
        {
            _textEditor.RemoveHandler(TextInputEvent, TextBox_TextChanged);
            _textEditor.BackKeyDown -= TextBox_KeyDown;
            _textEditor.GotFocus -= TextBox_GotFocus;
        }

        _textEditor = e.NameScope.Get<TextEditor>(PART_TEXT_BOX);
        

        if (_textEditor != null)
        {
            _textEditor.AddHandler(TextInputEvent, TextBox_TextChanged, RoutingStrategies.Tunnel);
            // 监听_textEditor的复制粘贴事件
            _textEditor.PastingFromClipboard += async (sender, args) =>
            {
                // 获取最新粘贴板
                
            };
            _textEditor.BackKeyDown += TextBox_KeyDown;
            _textEditor.GotFocus += TextBox_GotFocus;
        }
    }


    private void TextBox_GotFocus(object? sender, GotFocusEventArgs e)
    {
        if (!string.IsNullOrEmpty(_tempText))
        {
            SetAndRaise(TextProperty, ref _text, _tempText);
            _tempText = string.Empty; //Clear temp string
        }
    }

    private void TextBox_TextChanged(object? sender, TextInputEventArgs e)
    {
        if (_textEditor == null)
        {
            return;
        }

        string? textBoxText = _textEditor?.Text;
        Text = textBoxText + e.Text;
        var value = Source.FirstOrDefault(x => x.EditorType == EditorType.Text);
        if (value == null)
        {
            value = new EditorModel()
            {
                EditorType = EditorType.Text,
                Content = Text
            };
            Source.Add(value);
        }
        
        value.Content = Text;

        if (e.Text != null)
        {
            return;
        }

        e.Handled = true; //handle the event so the delimiter doesn't display
    }
}