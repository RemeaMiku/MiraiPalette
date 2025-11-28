using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Windows.ApplicationModel.DataTransfer;

namespace MiraiPalette.WinUI.Controls;

[ContentProperty(Name = nameof(Content))]
public sealed partial class CopyButton : Button
{
    private const string _normalState = "CopyNormal";
    private const string _copiedState = "Copied";


    public CopyButton()
    {
        Click += OnClick;
        DefaultStyleKey = typeof(CopyButton);
    }

    #region DependencyProperties

    public static readonly DependencyProperty TextToCopyProperty =
        DependencyProperty.Register(nameof(TextToCopy), typeof(string), typeof(CopyButton), new PropertyMetadata(string.Empty));

    /// <summary>
    /// 要复制的文本。如果为空，控件会尝试把 Content.ToString() 作为备用。
    /// </summary>
    public string TextToCopy
    {
        get => (string)GetValue(TextToCopyProperty);
        set => SetValue(TextToCopyProperty, value);
    }

    public static readonly DependencyProperty CopiedTextProperty =
        DependencyProperty.Register(nameof(CopiedText), typeof(string), typeof(CopyButton), new PropertyMetadata("Copied"));

    /// <summary>
    /// 复制成功后显示的提示文字（模板可绑定）。
    /// </summary>
    public string CopiedText
    {
        get => (string)GetValue(CopiedTextProperty);
        set => SetValue(CopiedTextProperty, value);
    }

    public static readonly DependencyProperty ResetDelayProperty =
        DependencyProperty.Register(nameof(ResetDelay), typeof(TimeSpan), typeof(CopyButton), new PropertyMetadata(TimeSpan.FromSeconds(2)));

    /// <summary>
    /// 复制成功后自动恢复为 Normal 的延迟。
    /// </summary>
    public TimeSpan ResetDelay
    {
        get => (TimeSpan)GetValue(ResetDelayProperty);
        set => SetValue(ResetDelayProperty, value);
    }

    public static readonly DependencyProperty CopyCommandProperty =
        DependencyProperty.Register(nameof(CopyCommand), typeof(ICommand), typeof(CopyButton), new PropertyMetadata(null));

    /// <summary>
    /// 可选的外部命令，在点击时会先尝试执行该命令（若 CanExecute 为 true），否则执行内部复制逻辑。
    /// </summary>
    public ICommand CopyCommand
    {
        get => (ICommand)GetValue(CopyCommandProperty);
        set => SetValue(CopyCommandProperty, value);
    }

    #endregion

    #region Events

    public event EventHandler? CopySucceeded;

    private void OnCopySucceeded() => CopySucceeded?.Invoke(this, EventArgs.Empty);

    #endregion

    private bool _isInCopiedState;

    private async void OnClick(object? sender, RoutedEventArgs e)
    {
        // 如果外部绑定了命令，优先调用
        if(CopyCommand != null && CopyCommand.CanExecute(TextToCopy))
        {
            CopyCommand.Execute(TextToCopy);
            EnterCopiedState();
            return;
        }

        // 执行默认复制行为
        var text = TextToCopy;
        if(string.IsNullOrEmpty(text) && Content != null)
        {
            text = Content?.ToString() ?? string.Empty;
        }

        if(!string.IsNullOrEmpty(text))
        {
            try
            {
                var dp = new DataPackage();
                dp.SetText(text);
                Clipboard.SetContent(dp);
                // 可选：Clipboard.Flush(); // 如果想保持在系统重启后仍可粘贴（不常用）
                OnCopySucceeded();
                EnterCopiedState();
            }
            catch(Exception)
            {
                // 失败就别崩溃，简单回退到普通提示（也可扩展事件或失败回调）
            }
        }
    }

    private async void EnterCopiedState()
    {
        if(_isInCopiedState)
            return;
        _isInCopiedState = true;

        VisualStateManager.GoToState(this, _copiedState, true);

        // 等待 ResetDelay，然后恢复
        var delay = ResetDelay;
        try
        {
            await Task.Delay(delay);
        }
        catch { /* ignore */ }

        VisualStateManager.GoToState(this, _normalState, true);
        _isInCopiedState = false;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        // Ensure initial VisualState
        VisualStateManager.GoToState(this, _normalState, false);
    }
}
