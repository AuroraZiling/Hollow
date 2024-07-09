using System;
using Avalonia;

namespace Hollow.Controls.WebView;

partial class WebView
{
    static void LoadHostDependencyObjectsChanged()
    {
        async void UrlPropertyChangedClassHandler(WebView s, AvaloniaPropertyChangedEventArgs<Uri?> e)
        {
            var newValue = e.NewValue.Value;
            await s.Navigate(newValue);
        }

        UrlProperty.Changed.AddClassHandler<WebView, Uri?>(UrlPropertyChangedClassHandler);

        async void HtmlContentPropertyChangedClassHandler(WebView s, AvaloniaPropertyChangedEventArgs<string?> e)
        {
            var newValue = e.NewValue.Value;
            await s.NavigateToString(newValue);
        }

        HtmlContentProperty.Changed.AddClassHandler<WebView, string?>(HtmlContentPropertyChangedClassHandler);
    }

    public static readonly StyledProperty<Uri?> UrlProperty =
           AvaloniaProperty.Register<WebView, Uri?>(nameof(Url));

    public static readonly StyledProperty<string?> HtmlContentProperty =
           AvaloniaProperty.Register<WebView, string?>(nameof(HtmlContent));

    public Uri? Url
    {
        get => GetValue(UrlProperty);
        set => SetValue(UrlProperty, value);
    }

    public string? HtmlContent
    {
        get => GetValue(HtmlContentProperty);
        set => SetValue(HtmlContentProperty, value);
    }
  
}
