using System;
using DryIoc;
using WebViewCore.Configurations;
using WebViewCore.Ioc;

namespace Hollow.Controls.WebView.Avalonia.WebView;

public static class AvaloniaWebViewBuilder
{
    public static void Initialize(Action<WebViewCreationProperties>? configDelegate)
    {
        WebViewCreationProperties creationProperties = new();
        configDelegate?.Invoke(creationProperties);
        WebViewLocator.s_Registrator.RegisterInstance<WebViewCreationProperties>(creationProperties);
    }
}
