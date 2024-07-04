using System.IO;
using Avalonia.Media.Imaging;

namespace Hollow.Helpers;

public static class BitmapOperations
{
    public static Bitmap Convert(Stream stream, int height)
    {
        var image = new Bitmap(stream);
        return Decode(image, height);
    }
    
    public static Bitmap Decode(Bitmap bitmap, int height)
    {
        var memory = new MemoryStream();
        bitmap.Save(memory, 100);
        memory.Position = 0;
        return Bitmap.DecodeToHeight(memory, height, BitmapInterpolationMode.HighQuality);
    }
}