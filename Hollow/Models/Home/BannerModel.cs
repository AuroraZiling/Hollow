using Avalonia.Media.Imaging;

namespace Hollow.Models.Home;

public class BannerModel
{
    public required string Link { get; set; }
    public required Bitmap Image { get; set; }
}