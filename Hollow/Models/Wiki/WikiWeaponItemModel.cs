using Avalonia.Media.Imaging;

namespace Hollow.Models.Wiki;

public class WikiWeaponItemModel
{
    public required string Id { get; set; }
    public required string AvatarUrl { get; set; }
    public required string Name { get; set; }
    public required Bitmap TypeIconResBitmap { get; set; } 
    public bool IsBRankType { get; set; }
    public bool IsARankType { get; set; }
    public bool IsSRankType { get; set; }
}