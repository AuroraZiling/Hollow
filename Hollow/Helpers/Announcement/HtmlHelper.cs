namespace Hollow.Helpers.Announcement;

public static class HtmlHelper
{
    const string HtmlTemplate = """
                                <!DOCTYPE html>
                                <html lang="en">
                                <head><meta charset="UTF-8">
                                <!-- HTTP 1.1 -->
                                <meta http-equiv="pragma" content="no-cache">
                                <!-- HTTP 1.0 -->
                                <meta http-equiv="cache-control" content="no-cache">
                                <meta name="applicable-device" content="mobile">
                                <meta name="viewport" content="width=device-width,minimum-scale=1.0,maximum-scale=1.0,user-scalable=no,viewport-fit=cover">
                                <meta http-equiv="Content-Type" content="text/html; charset=utf-8">
                                <meta name="Copyright" content="miHoYo">
                                <meta name="Description" content="">
                                <meta name="Keywords" content="">
                                <meta name="format-detection" content="telephone=no">
                                <meta name="format-detection" content="email=no">
                                <meta name="apple-mobile-web-app-capable" content="yes">
                                <meta name="apple-mobile-web-status-bar-style" content="black-translucent">
                                <meta http-equiv="X-UA-Compatible" content="IE=edge">
                                <title></title>
                                <link href="https://sdk.mihoyo.com/nap/announcement/bundle_3001fcd95019308e5247.css" rel="stylesheet"><link rel="stylesheet" type="text/css" href="https://sdk.mihoyo.com/nap/announcement/2_cf1c29ddf581f3a08c31.css"><script charset="utf-8" src="2_f0494879b40836121213.js"></script>
                                
                                </head>
                                <body>
                                {0}
                                </body>
                                </html>
                                """;
    public static string GetHtml(string content)
    {
        return string.Format(HtmlTemplate, content);
    }
}