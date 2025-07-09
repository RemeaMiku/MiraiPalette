using MiraiPalette.Maui.Essentials;

namespace MiraiPalette.Maui.Pages.Controls;

public partial class ColorPicker : ContentView
{
    enum ColorModel
    {
        RGB,
        HSL
    }

    private ColorModel _colorModel;
    private readonly Dictionary<ColorModel, VisualElement> _modelComponents = [];

    private void InitModelComponents()
    {
        _modelComponents.Add(ColorModel.RGB, _rgbComponents);
        _modelComponents.Add(ColorModel.HSL, _hslComponents);
    }

    public ColorPicker()
    {
        InitializeComponent();
        InitModelComponents();
        _modelPicker.ItemsSource = Enum.GetValues<ColorModel>();
        _modelPicker.SelectedIndex = 0;
        Picker_SelectedIndexChanged(_modelPicker, EventArgs.Empty);
    }

    public static readonly BindableProperty ColorProperty =
    BindableProperty.Create(nameof(Color), typeof(Color), typeof(Color), default, propertyChanged: OnColorChanged);

    private static void OnColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not ColorPicker colorPicker)
            return;
        colorPicker.UpdateView();
    }

    private void UpdateView()
    {
        UnRegisterEntryTextChangedEvents();
        if (Color == default)
        {
            _colorRect.BackgroundColor = Colors.Transparent;
            _hexEntry.Text = string.Empty;
            ClearRgbComponents();
            ClearHslComponents();
        }
        else
        {
            _colorRect.BackgroundColor = Color;
            _hexEntry.Text = Color.ToHex();
            SetRgbComponents();
            SetHslComponents();
        }
        RegisterEntryTextChangedEvents();
    }

    private void UnRegisterEntryTextChangedEvents()
    {
        _redEntry.TextChanged -= RgbComponentEntry_TextChanged;
        _greenEntry.TextChanged -= RgbComponentEntry_TextChanged;
        _blueEntry.TextChanged -= RgbComponentEntry_TextChanged;
        _hueEntry.TextChanged -= HslComponentEntry_TextChanged;
        _saturationEntry.TextChanged -= HslComponentEntry_TextChanged;
        _lightnessEntry.TextChanged -= HslComponentEntry_TextChanged;
    }

    private void RegisterEntryTextChangedEvents()
    {
        _redEntry.TextChanged += RgbComponentEntry_TextChanged;
        _greenEntry.TextChanged += RgbComponentEntry_TextChanged;
        _blueEntry.TextChanged += RgbComponentEntry_TextChanged;
        _hueEntry.TextChanged += HslComponentEntry_TextChanged;
        _saturationEntry.TextChanged += HslComponentEntry_TextChanged;
        _lightnessEntry.TextChanged += HslComponentEntry_TextChanged;
    }

    private void ClearRgbComponents()
    {
        _redEntry.Text = string.Empty;
        _greenEntry.Text = string.Empty;
        _blueEntry.Text = string.Empty;
    }

    private void SetRgbComponents()
    {
        Color.ToRgb(out var r, out var g, out var b);
        _redEntry.Text = r.ToString();
        _greenEntry.Text = g.ToString();
        _blueEntry.Text = b.ToString();
    }

    private void ClearHslComponents()
    {
        _hueEntry.Text = string.Empty;
        _saturationEntry.Text = string.Empty;
        _lightnessEntry.Text = string.Empty;
    }

    private void SetHslComponents()
    {
        Color.ToHsl(out var h, out var s, out var l);
        _hueEntry.Text = Math.Round(h * 360).ToString();
        _saturationEntry.Text = Math.Round(s * 100).ToString();
        _lightnessEntry.Text = Math.Round(l * 100).ToString();
    }

    private void SetColorFromComponents()
    {
        switch (_colorModel)
        {
            case ColorModel.RGB:
                SetColorFromRgbComponents();
                break;
            case ColorModel.HSL:
                SetColorFromHslComponents();
                break;
        }
    }

    private void SetColorFromRgbComponents(bool rollback = true)
    {
        if (!int.TryParse(_redEntry.Text, out var r) || !int.TryParse(_greenEntry.Text, out var g) || !int.TryParse(_blueEntry.Text, out var b))
        {
            if (rollback)
                SetRgbComponents();
            return;
        }
        Color = Color.FromRgb(r, g, b);
    }

    private void SetColorFromHslComponents(bool rollback = true)
    {
        if (!float.TryParse(_hueEntry.Text, out var h) || !float.TryParse(_saturationEntry.Text, out var s) || !float.TryParse(_lightnessEntry.Text, out var l))
        {
            if (rollback)
                SetHslComponents();
            return;
        }
        Color = Color.FromHsla(h / 360f, s / 100f, l / 100f);
    }

    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    private void UpdateComponentsVisibility()
    {
        foreach (var componets in _modelComponents.Values)
        {
            componets.IsVisible = false;
        }
        _modelComponents[_colorModel].IsVisible = true;
    }

    private void Picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (sender is not Picker picker)
            return;
        _colorModel = (ColorModel)picker.SelectedItem;
        UpdateComponentsVisibility();
    }

    private void ComponentEntry_Unfocused(object sender, FocusEventArgs e)
    {
        SetColorFromComponents();
    }

    private void SetColorFromHexEntry(bool rollback = true)
    {
        if (!ColorUtilities.TryParseRgb(_hexEntry.Text, out var color))
        {
            if (rollback)
                _hexEntry.Text = Color.ToHex();
            return;
        }
        Color = color;
    }

    private void HexEntry_Unfocused(object sender, FocusEventArgs e)
    {
        SetColorFromHexEntry();
    }

    private void HexEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        SetColorFromHexEntry(false);
    }

    private void RgbComponentEntry_TextChanged(object? sender, TextChangedEventArgs e)
    {
        SetColorFromRgbComponents(false);
    }

    private void HslComponentEntry_TextChanged(object? sender, TextChangedEventArgs e)
    {
        SetColorFromHslComponents(false);
    }
}