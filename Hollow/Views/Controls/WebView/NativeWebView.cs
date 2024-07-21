using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using Hollow.Extensions;

namespace Hollow.Views.Controls.WebView;

public class NativeWebView : NativeControlHost, IWebView, IDisposable
{
    private static readonly Uri EmptyPageLink = new("about:blank");
    
    private readonly IWebViewAdapter _webViewAdapter = App.GetService<IWebViewAdapter>();

    public event EventHandler<WebViewNavigationCompletedEventArgs>? NavigationCompleted;
    public event EventHandler<WebViewNavigationStartingEventArgs>? NavigationStarted;
    public event EventHandler<WebViewDomContentLoadedEventArgs>? DomContentLoaded;

    public static readonly StyledProperty<Uri?> SourceProperty = AvaloniaProperty.Register<NativeWebView, Uri?>(nameof(Source));

    public Uri? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }
    
    public bool CanGoBack => _webViewAdapter.CanGoBack;

    public bool CanGoForward => _webViewAdapter.CanGoForward;

    public bool GoBack() => _webViewAdapter.GoBack();

    public bool GoForward() => _webViewAdapter.GoForward();

    public NativeWebView()
    {
        _webViewAdapter.NavigationStarted += WebViewAdapterOnNavigationStarted;
        _webViewAdapter.NavigationCompleted += WebViewAdapterOnNavigationCompleted;
        _webViewAdapter.DomContentLoaded += WebViewAdapterOnDomContentLoaded;
    }
    
    ~NativeWebView()
    {
        _webViewAdapter.NavigationStarted -= WebViewAdapterOnNavigationStarted;
        _webViewAdapter.NavigationCompleted -= WebViewAdapterOnNavigationCompleted;
        _webViewAdapter.DomContentLoaded -= WebViewAdapterOnDomContentLoaded;
    }

    public Task<string?> InvokeScript(string scriptName)
    {
        return _webViewAdapter is null
            ? throw new InvalidOperationException("Control was not initialized")
            : _webViewAdapter.InvokeScript(scriptName);
    }

    public void Navigate(Uri url)
    {
        (_webViewAdapter ?? throw new InvalidOperationException("Control was not initialized"))
            .Navigate(url);
    }

    public void NavigateToString(string text)
    {
        (_webViewAdapter ?? throw new InvalidOperationException("Control was not initialized"))
            .NavigateToString(text);
    }

    public bool Refresh() => _webViewAdapter.Refresh();

    public bool Stop() => _webViewAdapter.Stop();

    protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
    {
        _webViewAdapter.SetParentAsync(parent.Handle).WaitOnDispatcherFrame();
        return _webViewAdapter;
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        // 先设置父句柄为空，防止WebView被销毁
        _webViewAdapter.SetParentAsync(IntPtr.Zero).WaitOnDispatcherFrame();
        base.OnDetachedFromVisualTree(e);
    }

    private void WebViewAdapterOnDomContentLoaded(object? sender, WebViewDomContentLoadedEventArgs e)
    {
        DomContentLoaded?.Invoke(this, e);
    }

    private void WebViewAdapterOnNavigationStarted(object? sender, WebViewNavigationStartingEventArgs e)
    {
        NavigationStarted?.Invoke(this, e);
    }

    private void WebViewAdapterOnNavigationCompleted(object? sender, WebViewNavigationCompletedEventArgs e)
    {
        SetCurrentValue(SourceProperty, e.Request);
        NavigationCompleted?.Invoke(this, e);
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == SourceProperty)
        {
            _webViewAdapter.Source = change.GetNewValue<Uri?>() ?? EmptyPageLink;
        }
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        _webViewAdapter.HandleSizeChanged(e.NewSize);
        
        base.OnSizeChanged(e);
    }

    public void Dispose()
    {
        _webViewAdapter.Dispose();
        GC.SuppressFinalize(this);
    }
}
