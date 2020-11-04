namespace GtR
{
    public class GtrConfig
    {
        public float CardShortSideInInches { get; set; }
        public float CardLongSideInInches { get; set; }
        public float BleedSizeInInches { get; set; }
        public float BorderPaddingInInches { get; set; }
        public SaveConfiguration SaveConfiguration { get; set; }
        public CardType[] CardTypesToInclude { get; set; }
    }
}
