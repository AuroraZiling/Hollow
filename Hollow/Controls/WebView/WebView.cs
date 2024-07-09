using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using AvaloniaWebView.Core;
using AvaloniaWebView.Shared;
using DryIoc.Shared.Extensions;
using Hollow.Controls.WebView.Avalonia.WebView.Helpers;
using WebViewCore;
using WebViewCore.Configurations;
using WebViewCore.Ioc;

namespace Hollow.Controls.WebView.Avalonia.WebView;

public sealed partial class WebView : Control, IVirtualWebView<Hollow.Controls.WebView.Avalonia.WebView.WebView>, IEmptyView, IWebViewEventHandler, IVirtualWebViewControlCallBack, IWebViewControl
{
    static WebView()
    {
        AffectsRender<Hollow.Controls.WebView.Avalonia.WebView.WebView>(global::Hollow.Controls.WebView.Avalonia.WebView.WebView.BackgroundProperty, global::Hollow.Controls.WebView.Avalonia.WebView.WebView.BorderBrushProperty, global::Hollow.Controls.WebView.Avalonia.WebView.WebView.BorderThicknessProperty, global::Hollow.Controls.WebView.Avalonia.WebView.WebView.CornerRadiusProperty, global::Hollow.Controls.WebView.Avalonia.WebView.WebView.BoxShadowProperty);
        AffectsMeasure<Hollow.Controls.WebView.Avalonia.WebView.WebView>(global::Hollow.Controls.WebView.Avalonia.WebView.WebView.ChildProperty, global::Hollow.Controls.WebView.Avalonia.WebView.WebView.PaddingProperty, global::Hollow.Controls.WebView.Avalonia.WebView.WebView.BorderThicknessProperty);
        global::Hollow.Controls.WebView.Avalonia.WebView.WebView.LoadDependencyObjectsChanged();
        global::Hollow.Controls.WebView.Avalonia.WebView.WebView.LoadHostDependencyObjectsChanged();
    }

    public WebView()
        : this(default)
    {

    }

    public WebView(IServiceProvider? serviceProvider = default)
    {
        var properties = WebViewLocator.s_ResolverContext.GetRequiredService<WebViewCreationProperties>();
        _creationProperties = properties;

        _viewHandlerProvider = WebViewLocator.s_ResolverContext.GetRequiredService<IViewHandlerProvider>();
        ClipToBounds = false;

        _partEmptyViewPresenter = new()
        {
            [!ContentPresenter.ContentProperty] = this[!global::Hollow.Controls.WebView.Avalonia.WebView.WebView.EmptyViewerProperty],
            [!ContentPresenter.ContentTemplateProperty] = this[!global::Hollow.Controls.WebView.Avalonia.WebView.WebView.EmptyViewerTemplateProperty],
        };

        _partInnerContainer = new()
        {
            Child = _partEmptyViewPresenter,
            ClipToBounds = true,
            [!Border.CornerRadiusProperty] = this[!global::Hollow.Controls.WebView.Avalonia.WebView.WebView.CornerRadiusProperty]
        };
        Child = _partInnerContainer;
    }

    readonly WebViewCreationProperties _creationProperties;
    readonly BorderRenderHelper _borderRenderHelper = new();
    readonly IViewHandlerProvider _viewHandlerProvider;

    readonly Border _partInnerContainer;
    readonly ContentPresenter _partEmptyViewPresenter;

    double _scale;
    Thickness? _layoutThickness;

    IPlatformWebView? _platformWebView;
    public IPlatformWebView? PlatformWebView => _platformWebView;
}
