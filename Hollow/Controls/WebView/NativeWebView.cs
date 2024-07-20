using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using Hollow.Controls.WebView.Win;

namespace Hollow.Controls.WebView;

public class NativeWebView : NativeControlHost, IWebView
{
    private static readonly Uri EmptyPageLink = new("about:blank");
    
    private IWebViewAdapter? _webViewAdapter;
    private TaskCompletionSource _webViewReadyCompletion = new();

    public event EventHandler<WebViewNavigationCompletedEventArgs>? NavigationCompleted;

    public event EventHandler<WebViewNavigationStartingEventArgs>? NavigationStarted;
    public event EventHandler<WebViewDomContentLoadedEventArgs>? DomContentLoaded;

    public static readonly StyledProperty<Uri?> SourceProperty = AvaloniaProperty.Register<NativeWebView, Uri?>(nameof(Source));

    public Uri? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }
    
    public bool CanGoBack => _webViewAdapter?.CanGoBack ?? false;

    public bool CanGoForward => _webViewAdapter?.CanGoForward ?? false;

    public bool GoBack() => _webViewAdapter?.GoBack() ?? false;

    public bool GoForward() => _webViewAdapter?.GoForward() ?? false;

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

    public bool Refresh() => _webViewAdapter?.Refresh() ?? false;

    public bool Stop() => _webViewAdapter?.Stop() ?? false;

    protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && WebViewCapabilities.IsMsWebView2Available)
        {
            _webViewAdapter = new WebView2Adapter(base.CreateNativeControlCore(parent));
        }
        else
        {
            return base.CreateNativeControlCore(parent);
        }

        SubscribeOnEvents();
        
        if (_webViewAdapter.IsInitialized)
        {
            WebViewAdapterOnInitialized(_webViewAdapter, EventArgs.Empty);
        }
        else
        {
            _webViewAdapter.Initialized += WebViewAdapterOnInitialized;
        }

        return _webViewAdapter;
    }

    private void SubscribeOnEvents()
    {
        if (_webViewAdapter is not null)
        {
            _webViewAdapter.NavigationStarted += WebViewAdapterOnNavigationStarted;
            _webViewAdapter.NavigationCompleted += WebViewAdapterOnNavigationCompleted;
            _webViewAdapter.DomContentLoaded += WebViewAdapterOnDomContentLoaded;
        }
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
    
    private void WebViewAdapterOnInitialized(object? sender, EventArgs e)
    {
        var adapter = (IWebViewAdapter)sender!;
        adapter.Initialized -= WebViewAdapterOnInitialized;
        if (Source is not null)
        {
            adapter.Source = Source;
        }

        _webViewReadyCompletion.TrySetResult();
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == SourceProperty)
        {
            if (_webViewAdapter is { IsInitialized: true })
            {
                _webViewAdapter.Source = change.GetNewValue<Uri?>() ?? EmptyPageLink;
            }
        }
        else if (change.Property == BoundsProperty)
        {
            _webViewAdapter?.SizeChanged();
        }
    }

    protected override void DestroyNativeControlCore(IPlatformHandle control)
    {
        DestroyWebViewAdapter();
    }

    private void DestroyWebViewAdapter()
    {
        if (_webViewAdapter is not null)
        {
            _webViewReadyCompletion = new TaskCompletionSource();
            var adapter = _webViewAdapter;
            _webViewAdapter = null;
            adapter.DomContentLoaded -= WebViewAdapterOnDomContentLoaded;
            adapter.NavigationStarted -= WebViewAdapterOnNavigationStarted;
            adapter.NavigationCompleted -= WebViewAdapterOnNavigationCompleted;
            adapter.Initialized -= WebViewAdapterOnInitialized;
            adapter.Dispose();
        }
    }
}
