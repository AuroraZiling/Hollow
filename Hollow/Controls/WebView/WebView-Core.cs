using System;
using System.Threading.Tasks;

namespace Hollow.Controls.WebView;

partial class WebView
{
    async Task Navigate(Uri? uri)
    {
        if (uri is null) return;

        if (_platformWebView is null) return;

        if (!_platformWebView.IsInitialized)
        {
            var bRet = await _platformWebView.Initialize();
            if (!bRet) return;
        }

        if (_platformWebView is null) return;
        try
        {
            _platformWebView.Navigate(uri);
        }catch (Exception)
        {
            // ignored
        }
    }

    async Task NavigateToString(string? htmlContent)
    {
        if (string.IsNullOrWhiteSpace(htmlContent)) return;

        if (_platformWebView is null) return;

        if (!_platformWebView.IsInitialized)
        {
            var bRet = await _platformWebView.Initialize();
            if (!bRet) return;
        }

        _platformWebView.NavigateToString(htmlContent);
    }

}
