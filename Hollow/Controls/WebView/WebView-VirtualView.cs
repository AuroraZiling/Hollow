using WebViewCore;

namespace Hollow.Controls.WebView;

partial class WebView
{
    WebView IVirtualWebView<WebView>.VirtualView => this;

    object IVirtualWebView.VirtualViewObject => this;

    IPlatformWebView? IVirtualWebView.PlatformView => PlatformWebView;
}
