using System.Drawing;

namespace GtR
{
    public interface ISaveableImage
    {
        string Name { get; }
        string Subfolder { get; }
        Bitmap Bitmap { get; }
    }
}
