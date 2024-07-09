using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using AvaloniaWebView.Core;
using AvaloniaWebView.Shared;
using DryIoc.Shared.Extensions;
using Hollow.Controls.WebView.Helpers;
using WebViewCore;
using WebViewCore.Configurations;
using WebViewCore.Ioc;

namespace Hollow.Controls.WebView;

public sealed partial class WebView : Control, IVirtualWebView<WebView>, IEmptyView, IWebViewEventHandler, IVirtualWebViewControlCallBack, IWebViewControl
{
    static WebView()
    {
        AffectsRender<WebView>(BackgroundProperty, BorderBrushProperty, BorderThicknessProperty, CornerRadiusProperty, BoxShadowProperty);
        AffectsMeasure<WebView>(ChildProperty, PaddingProperty, BorderThicknessProperty);
        LoadDependencyObjectsChanged();
        LoadHostDependencyObjectsChanged();
    }

    public WebView()
    {
        var properties = WebViewLocator.s_ResolverContext.GetRequiredService<WebViewCreationProperties>();
        _creationProperties = properties;

        _viewHandlerProvider = WebViewLocator.s_ResolverContext.GetRequiredService<IViewHandlerProvider>();
        ClipToBounds = false;

        ContentPresenter partEmptyViewPresenter = new()
        {
            [!ContentPresenter.ContentProperty] = this[!EmptyViewerProperty],
            [!ContentPresenter.ContentTemplateProperty] = this[!EmptyViewerTemplateProperty],
        };

        _partInnerContainer = new()
        {
            Child = partEmptyViewPresenter,
            ClipToBounds = true,
            [!Border.CornerRadiusProperty] = this[!CornerRadiusProperty]
        };
        Child = _partInnerContainer;
    }

    readonly WebViewCreationProperties _creationProperties;
    readonly BorderRenderHelper _borderRenderHelper = new();
    readonly IViewHandlerProvider _viewHandlerProvider;

    readonly Border _partInnerContainer;

    double _scale;
    Thickness? _layoutThickness;

    IPlatformWebView? _platformWebView;
    public IPlatformWebView? PlatformWebView => _platformWebView;
}
