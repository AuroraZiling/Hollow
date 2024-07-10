using System;
using DryIoc;
using Hollow.Models;
using WebViewCore.Configurations;
using WebViewCore.Ioc;

namespace Hollow.Controls.WebView;

public static class AvaloniaWebViewBuilder
{
    public static void Initialize(Action<WebViewCreationProperties>? configDelegate)
    {
        WebViewCreationProperties creationProperties = new();
        configDelegate?.Invoke(creationProperties);
        WebViewLocator.s_Registrator.RegisterInstance(creationProperties);
    }
}
