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
        public static int InfluenceAndPoints(this CardSuit cardSuit)
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
                    return System.Drawing.Color.FromArgb(238, 159, 46);
                case CardSuit.GreenCraftsmanWood:
                    return System.Drawing.Color.FromArgb(62, 164, 70);
                case CardSuit.GreyArchitectConcrete:
                    return System.Drawing.Color.FromArgb(148, 148, 137);
                case CardSuit.RedLegionaryBrick:
                    return System.Drawing.Color.FromArgb(217, 35, 43);
                case CardSuit.PurplePatronMarble:
                    return System.Drawing.Color.FromArgb(140, 55, 111);
                case CardSuit.BlueMerchantStone:
                    return System.Drawing.Color.FromArgb(0, 176, 203);
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
