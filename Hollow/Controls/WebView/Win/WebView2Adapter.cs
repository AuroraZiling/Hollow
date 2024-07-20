using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia.Platform;
using Microsoft.Web.WebView2.Core;

namespace Hollow.Controls.WebView.Win;

[SupportedOSPlatform("Windows")]
internal class WebView2Adapter : IWebViewAdapter
{
    private CoreWebView2Controller? _controller;
    private Action? _subscriptions;

    public WebView2Adapter(IPlatformHandle handle)
    {
        Handle = handle.Handle;

        Initialize();
    }

    public IntPtr Handle { get; }
    public string HandleDescriptor => "HWND";
    
    private async void Initialize()
    {
        var env = await CoreWebView2Environment.CreateAsync();
        var controller = await env.CreateCoreWebView2ControllerAsync(Handle);
        controller.DefaultBackgroundColor = Color.FromArgb(49, 49, 49);
        controller.IsVisible = true;
        _controller = controller;

        SizeChanged();
        
        _controller.CoreWebView2.Settings.IsStatusBarEnabled = false;
        _subscriptions = AddHandlers(_controller.CoreWebView2);

        IsInitialized = true;
        Initialized?.Invoke(this, EventArgs.Empty);
    }

    public bool IsInitialized { get; private set; }

    public bool CanGoBack => _controller?.CoreWebView2?.CanGoBack ?? false;

    public bool CanGoForward => _controller?.CoreWebView2?.CanGoForward ?? false;

    public Uri? Source
    {
        [return: MaybeNull]
        get => Uri.TryCreate(_controller?.CoreWebView2?.Source, UriKind.Absolute, out var url) ? url : null!;
        set => _controller?.CoreWebView2?.Navigate(value?.AbsoluteUri);
    }

    public event EventHandler<WebViewNavigationCompletedEventArgs>? NavigationCompleted;
    public event EventHandler<WebViewNavigationStartingEventArgs>? NavigationStarted;
    public event EventHandler<WebViewDomContentLoadedEventArgs>? DomContentLoaded;
    public event EventHandler? Initialized;

    public void Dispose()
    {
        _subscriptions?.Invoke();
        _controller?.Close();
    }

    public bool GoBack()
    {
        _controller?.CoreWebView2.GoBack();
        return true;
    }

    public bool GoForward()
    {
        _controller?.CoreWebView2.GoForward();
        return true;
    }

    public Task<string?> InvokeScript(string scriptName)
    {
        return _controller?.CoreWebView2?.ExecuteScriptAsync(scriptName) ?? Task.FromResult<string?>(null);
    }

    public void Navigate(Uri url)
    {
        _controller?.CoreWebView2?.Navigate(url.AbsolutePath);
    }

    public void NavigateToString(string text)
    {
        _controller?.CoreWebView2?.NavigateToString(text);
    }

    public bool Refresh()
    {
        _controller?.CoreWebView2?.Reload();
        return true;
    }

    public bool Stop()
    {
        _controller?.CoreWebView2?.Stop();
        return true;
    }
    
    public void SizeChanged()
    {
        WinApiHelpers.GetWindowRect(Handle, out var rect);
        
        if (_controller is not null)
        {
            _controller.BoundsMode = CoreWebView2BoundsMode.UseRawPixels;
            _controller.Bounds = new System.Drawing.Rectangle(0, 0, rect.Right - rect.Left, rect.Bottom - rect.Top);
        }
    }
    
    private Action AddHandlers(CoreWebView2 webView)
    {
        webView.DOMContentLoaded += WebViewOnDomContentLoaded;
        void WebViewOnDomContentLoaded(object? sender, CoreWebView2DOMContentLoadedEventArgs e)
        {
            DomContentLoaded?.Invoke(this, new WebViewDomContentLoadedEventArgs());
        }

        webView.WebResourceRequested += (s, e) =>
        {
            if(e.Request.Uri.Contains("aaa"))
                e.Response = webView.Environment.CreateWebResourceResponse(System.IO.Stream.Null, 404, "Not Found", "Content-Type: text/html");
        };
        
        webView.NavigationStarting += WebViewOnNavigationStarting;
        void WebViewOnNavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
        {
            var args = new WebViewNavigationStartingEventArgs { Request = new Uri(e.Uri) };
            NavigationStarted?.Invoke(this, args);
            if (args.Cancel)
            {
                e.Cancel = true;
            }
        }

        webView.NavigationCompleted += WebViewOnNavigationCompleted;
        void WebViewOnNavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            NavigationCompleted?.Invoke(this, new WebViewNavigationCompletedEventArgs
            {
                Request = new Uri(((CoreWebView2)sender!).Source),
                IsSuccess = e.IsSuccess
            });
        }

        return () =>
        {
            webView.NavigationStarting -= WebViewOnNavigationStarting;
            webView.NavigationCompleted -= WebViewOnNavigationCompleted;
            webView.DOMContentLoaded -= WebViewOnDomContentLoaded;
        };
    }
}
