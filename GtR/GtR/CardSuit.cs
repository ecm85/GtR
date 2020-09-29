using System;
using System.Drawing;

namespace GtR
{
    public enum CardSuit
    {
        BlueMerchantStone,
        GreenCraftsmanWood,
        GreyArchitectConcrete,
        PurplePatronMarble,
        RedLegionaryBrick,
        YellowLaborerRubble
    }

    public static class CardSuitExtensions
    {
        private static int Value(this CardSuit cardSuit)
        {
            switch (cardSuit)
            {
                case CardSuit.YellowLaborerRubble:
                case CardSuit.GreenCraftsmanWood:
                    return 1;
                case CardSuit.GreyArchitectConcrete:
                case CardSuit.RedLegionaryBrick:
                    return 2;
                case CardSuit.PurplePatronMarble:
                case CardSuit.BlueMerchantStone:
                    return 3;
                default:
                    throw new InvalidOperationException($"Invalid card suit encountered: {cardSuit}");
            }
        }

        public static int Points(this CardSuit cardSuit) => Value(cardSuit);
        public static int Influence(this CardSuit cardSuit) => Value(cardSuit);
        public static int Cost(this CardSuit cardSuit) => Value(cardSuit);

        public static int CardCountPerDeck(this CardSuit cardSuit)
        {
            switch (cardSuit)
            {
                case CardSuit.YellowLaborerRubble:
                case CardSuit.GreenCraftsmanWood:
                    return 2;
                case CardSuit.GreyArchitectConcrete:
                case CardSuit.RedLegionaryBrick:
                case CardSuit.PurplePatronMarble:
                case CardSuit.BlueMerchantStone:
                    return 1;
                default:
                    throw new InvalidOperationException($"Invalid card suit encountered: {cardSuit}");
            }
        }

        public static string RoleName(this CardSuit cardSuit)
        {
            switch (cardSuit)
            {
                case CardSuit.YellowLaborerRubble:
                    return "Laborer";
                case CardSuit.GreenCraftsmanWood:
                    return "Craftsman";
                case CardSuit.GreyArchitectConcrete:
                    return "Architect";
                case CardSuit.RedLegionaryBrick:
                    return "Legionary";
                case CardSuit.PurplePatronMarble:
                    return "Patron";
                case CardSuit.BlueMerchantStone:
                    return "Merchant";
                default:
                    throw new InvalidOperationException($"Invalid card suit encountered: {cardSuit}");
            }
        }

        public static string ResourceName(this CardSuit cardSuit)
        {
            switch (cardSuit)
            {
                case CardSuit.YellowLaborerRubble:
                    return "Rubble";
                case CardSuit.GreenCraftsmanWood:
                    return "Wood";
                case CardSuit.GreyArchitectConcrete:
                    return "Concrete";
                case CardSuit.RedLegionaryBrick:
                    return "Brick";
                case CardSuit.PurplePatronMarble:
                    return "Marble";
                case CardSuit.BlueMerchantStone:
                    return "Stone";
                default:
                    throw new InvalidOperationException($"Invalid card suit encountered: {cardSuit}");
            }
        }

        public static Color Color(this CardSuit cardSuit)
        {
            switch (cardSuit)
            {
                case CardSuit.YellowLaborerRubble:
                    return System.Drawing.Color.FromArgb(251, 166, 28);
                case CardSuit.GreenCraftsmanWood:
                    return System.Drawing.Color.FromArgb(61, 173, 72);
                case CardSuit.GreyArchitectConcrete:
                    return System.Drawing.Color.FromArgb(155, 155, 147);
                case CardSuit.RedLegionaryBrick:
                    return System.Drawing.Color.FromArgb(238, 28, 37);
                case CardSuit.PurplePatronMarble:
                    return System.Drawing.Color.FromArgb(158, 47, 116);
                case CardSuit.BlueMerchantStone:
                    return System.Drawing.Color.FromArgb(0, 184, 223);
                default:
                    throw new InvalidOperationException($"Invalid card suit encountered: {cardSuit}");
            }
        }

        public static string ColorText(this CardSuit cardSuit)
        {
            switch (cardSuit)
            {
                case CardSuit.YellowLaborerRubble:
                    return "Yellow";
                case CardSuit.GreenCraftsmanWood:
                    return "Green";
                case CardSuit.GreyArchitectConcrete:
                    return "Grey";
                case CardSuit.RedLegionaryBrick:
                    return "Red";
                case CardSuit.PurplePatronMarble:
                    return "Purple";
                case CardSuit.BlueMerchantStone:
                    return "Blue";
                default:
                    throw new InvalidOperationException($"Invalid card suit encountered: {cardSuit}");
            }
        }
    }
}
