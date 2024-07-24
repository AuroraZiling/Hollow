using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using Hollow.Views.Controls.WebView;
using Microsoft.Web.WebView2.Core;
using Size = Avalonia.Size;

namespace Hollow.Windows.Services;

public class WebView2Adapter : IWebViewAdapter
{
    private CoreWebView2Controller? _controller;
    private Action? _subscriptions;
    private InvisibleWindow? _invisibleWindow;

    public IntPtr Handle { get; private set; }
    public string HandleDescriptor => nameof(HWND);
    
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

    public async Task SetParentAsync(IntPtr handle)
    {
        if (handle == IntPtr.Zero)
        {
            // 如果父窗口为空，说明TabControl切换到了其他Tab，这时候需要隐藏WebView2，牺牲内存换取性能
            // 如果父窗口被关闭，那么WebView2也会被关闭，导致下一次显示时需要重新初始化
            _invisibleWindow ??= new InvisibleWindow(); 
            handle = _invisibleWindow.HWnd;
        }
        else
        {
            // Avalonia提供的宿主窗口默认为黑色背景，设置为透明
            // 由于 WebView2 不是GDI+，所以不会影响 WebView2 的渲染
            await Task.Delay(150);
            PInvoke.SetLayeredWindowAttributes((HWND)handle, new COLORREF(1), 0, LAYERED_WINDOW_ATTRIBUTES_FLAGS.LWA_COLORKEY); 
        }
        
        Handle = handle;

        if (_controller == null)
        {
            var env = await CoreWebView2Environment.CreateAsync();
            _controller = await env.CreateCoreWebView2ControllerAsync(Handle);
            _controller.DefaultBackgroundColor = Color.Transparent;  // 这里的文档提到 WebView2 是支持透明背景的
            _controller.IsVisible = true;
        }
        else
        {
            _controller.ParentWindow = Handle;  // 重新设置父窗口
        }
        
        _controller.CoreWebView2.Settings.IsStatusBarEnabled = false;
        _subscriptions = AddHandlers(_controller.CoreWebView2);
    }

    public void HandleSizeChanged(Size newSize)
    {
        if (_controller is not null)
        {
            _controller.BoundsMode = CoreWebView2BoundsMode.UseRasterizationScale;
            _controller.Bounds = new Rectangle(0, 0, (int)newSize.Width, (int)newSize.Height);
        }
    }
    
    private Action AddHandlers(CoreWebView2 webView)
    {
        webView.DOMContentLoaded += WebViewOnDomContentLoaded;
        void WebViewOnDomContentLoaded(object? sender, CoreWebView2DOMContentLoadedEventArgs e)
        {
            DomContentLoaded?.Invoke(this, new WebViewDomContentLoadedEventArgs());
        }
        
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
    
    public void Dispose()
    {
        _invisibleWindow?.Dispose();
        _subscriptions?.Invoke();
        _controller?.Close();
    }
    
    private unsafe class InvisibleWindow : IDisposable
    {
        public HWND HWnd { get; private set; }
        
        // 防止委托被GC回收
        // ReSharper disable once NotAccessedField.Local
        private readonly WNDPROC _wndProcDelegate;

        public InvisibleWindow()
        {
            fixed (char* invisible = "InvisibleWindow")
            {
                // Define window class
                var wndClass = new WNDCLASSEXW
                {
                    cbSize = (uint)Marshal.SizeOf<WNDCLASSEXW>(),
                    lpfnWndProc = _wndProcDelegate = WndProc,
                    hInstance = PInvoke.GetModuleHandle(new PCWSTR()),
                    lpszClassName = invisible
                };

                PInvoke.RegisterClassEx(wndClass);

                // Create window
                HWnd = PInvoke.CreateWindowEx(
                    0,
                    wndClass.lpszClassName,
                    invisible,
                    0,
                    0,
                    0,
                    0,
                    0,
                    HWND.Null,
                    HMENU.Null,
                    wndClass.hInstance,
                    null);
            }
        }

        private LRESULT WndProc(HWND hWnd, uint msg, WPARAM wParam, LPARAM lParam)
        {
            switch (msg)
            {
                case PInvoke.WM_DESTROY:
                    PInvoke.PostQuitMessage(0);
                    break;
            }
            
            return PInvoke.DefWindowProc(hWnd, msg, wParam, lParam);
        }

        public void Dispose()
        {
            if (HWnd == IntPtr.Zero) return;
            
            PInvoke.DestroyWindow(HWnd);
            HWnd = default;
        }
    }
}
