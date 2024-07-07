using System.Text.RegularExpressions;

namespace Hollow.Helpers;

public partial class HtmlFilter
{
    public static string RemoveBr(string html)
    {
        return html.Replace("<br>", "\n").Replace("<br />", "\n").Replace("<br/>", "\n");
    }
    
    public static string RemoveP(string html)
    {
        return RemovePPattern().Replace(html, "");
    }

    [GeneratedRegex(@"<.*?p.*?>", RegexOptions.Singleline)]
    private static partial Regex RemovePPattern();
}