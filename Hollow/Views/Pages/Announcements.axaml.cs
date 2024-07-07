using Avalonia.Controls;
using Hollow.Helpers;
using Hollow.ViewModels.Pages;

namespace Hollow.Views.Pages;

public partial class Announcements : UserControl
{
    private const string Script = """
                                  // Remove the mask background
                                  let home_mask = document.getElementsByClassName("home__mask");
                                  if (home_mask.length > 0) {
                                      home_mask[0].remove();
                                  } 
                                  
                                  // Apply the MiSans font
                                  var link = document.createElement('link');
                                  link.type = 'text/css';
                                  link.rel = 'stylesheet';
                                  document.head.appendChild(link);
                                  link.href = 'https://cdn.jsdelivr.net/npm/misans@4.0.0/lib/Normal/MiSans-Medium.min.css';
                                  let inner_ann = document.getElementsByClassName("inner-ann");
                                  if (inner_ann.length > 0) {
                                      inner_ann[0].style.fontFamily = 'MiSans';
                                  } 
                                  
                                  // Remove the close button
                                  let home_close = document.getElementsByClassName("home__close");
                                  if (home_close.length > 0) {
                                      home_close[0].remove();
                                  }
                                  """;
    public Announcements(AnnouncementsViewModel announcementsViewModel)
    {
        InitializeComponent();
        DataContext = announcementsViewModel;
    }

    private void GameAnnouncementWebView_OnNavigated(string url, string _)
    {
        if (url.StartsWith("https://sdk.mihoyo.com/nap/announcement/index.html"))
        {
            GameAnnouncementWebView.ExecuteScript(Script);
        }
        else if (url.StartsWith("uniwebview://open_url?url="))
        {
            HtmlHelper.OpenUrl(url.Replace("uniwebview://open_url?url=", ""));
        }
    }
}