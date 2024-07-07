namespace Hollow.Helpers;

public class HtmlFilter
{
    public static string FilterBr(string html)
    {
        return html.Replace("<br>", "\n").Replace("<br />", "\n").Replace("<br/>", "\n");
    }
}