using System.Collections.Generic;

namespace Hollow.Models.Pages.Announcement;

public class AnnouncementModel
{
    // From ZzzAnnouncementModel
    public required int Id { get; set; }
    public required string Title { get; set; }
    public required string Subtitle { get; set; }
    public required string BannerUrl { get; set; }
    public required string TagLabel { get; set; }
    public required string TagIconUrl { get; set; }
    public required string TagIconHoverUrl { get; set; }
    public required string StartTime { get; set; }
    public required string EndTime { get; set; }
    public required bool HasContent { get; set; }
    
    // From ZzzAnnouncementContentModel
    public required string Content { get; set; }
}