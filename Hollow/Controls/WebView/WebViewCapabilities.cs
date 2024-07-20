using System;

namespace Hollow.Controls.WebView;

public class WebViewCapabilities
{
    private static bool? _isMsWebView2Available;
    public static bool IsMsWebView2Available => _isMsWebView2Available ??= IsMsWebView2AvailableInternal();
    
    private static bool IsMsWebView2AvailableInternal()
    {
        try
        {
            var versionString = Microsoft.Web.WebView2.Core.CoreWebView2Environment.GetAvailableBrowserVersionString();
            return !string.IsNullOrWhiteSpace(versionString);
        }
        catch (Exception)
        {
            return false;
        }
    }
}
