using System;

namespace GtR
{
    public enum CardSet
    {
        Standard,
        Imperium,
        Republic,
        Promo
    }

    public static class CardSetExtensions
    {
        public static string SetText(this CardSet cardSet)
        {
            switch(cardSet)
            {
                case CardSet.Republic:
                    return "Republic";
                case CardSet.Imperium:
                    return "Imperium";
                case CardSet.Promo:
                    return "Promo";
                default:
                    throw new InvalidOperationException($"Invalid card set encountered: {cardSet}.");
            }
        }

        public static bool HasSetImage(this CardSet cardSet)
        {
            switch (cardSet)
            {
                case CardSet.Republic:
                case CardSet.Imperium:
                case CardSet.Promo:
                    return true;
                case CardSet.Standard:
                    return false;
                default:
                    throw new InvalidOperationException($"Invalid card set encountered: {cardSet}.");
            }
        }
    }
}
