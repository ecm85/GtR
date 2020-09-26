using System;

namespace GtR
{
    public enum CardSet
    {
        Standard,
        Imperium,
        Republic
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
                    return true;
                case CardSet.Standard:
                    return false;
                default:
                    throw new InvalidOperationException($"Invalid card set encountered: {cardSet}.");
            }
        }
    }
}
