using Avalonia.Media.Imaging;

namespace Hollow.Models.Wiki;

public class WikiCharacterItemModel
{
    public required string AvatarUrl { get; set; }
    public required string Name { get; set; }
    public required Bitmap TypeIconResBitmap { get; set; } 
    public required Bitmap ElementIconResBitmap { get; set; }
    public bool IsARankType { get; set; }
    public bool IsSRankType { get; set; }
}