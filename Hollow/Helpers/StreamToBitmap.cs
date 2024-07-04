using System.IO;
using Avalonia.Media.Imaging;

namespace Hollow.Helpers;

public static class StreamToBitmap
{
    public static Bitmap Convert(Stream stream, int height)
    {
        var image = new Bitmap(stream);
        var memory = new MemoryStream();
        image.Save(memory, 100);
        memory.Position = 0;
        return Bitmap.DecodeToHeight(memory, height, BitmapInterpolationMode.HighQuality);
    }
    
}