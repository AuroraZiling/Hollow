namespace Hollow.Models.Wiki;

public class WikiBangbooItemModel
{
    public required string AvatarUrl { get; set; }
    public required string Name { get; set; }
    public bool IsBRankType { get; set; }
    public bool IsARankType { get; set; }
    public bool IsSRankType { get; set; }
}