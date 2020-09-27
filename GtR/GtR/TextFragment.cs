using System.Drawing;

namespace GtR
{
    public class TextFragment
    {
        public Font Font { get; set; }
        public Brush Brush { get; set; }
        public string Text { get; set; }
        public bool ForcesNewline { get; set; }
    }
}
