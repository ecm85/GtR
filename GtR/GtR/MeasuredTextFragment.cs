namespace GtR
{
    public class MeasuredTextFragment
    {
        public TextFragment TextFragment { get; }
        public float Width { get; }
        public int Height { get; }

        public MeasuredTextFragment(float width, int height, TextFragment textFragment)
        {
            Width = width;
            Height = height;
            TextFragment = textFragment;
        }
    }
}
