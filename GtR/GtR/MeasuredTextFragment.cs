namespace GtR
{
    public class MeasuredTextFragment
    {
        public TextFragment TextFragment { get; }
        public int Width { get; }
        public int Height { get; }

        public MeasuredTextFragment(int width, int height, TextFragment textFragment)
        {
            Width = width;
            Height = height;
            TextFragment = textFragment;
        }
    }
}
