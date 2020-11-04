using System;

namespace GtR
{
    public class OrderCard
    {
        public CardSuit CardSuit { get; set; }
        public string CardName { get; set; }
        public string CardText { get; set; }
        public CardSet CardSet { get; set; }
        public bool ImageIsRoughlyCentered { get; set; }
        public CardType CardType => CardSet switch
        {
            CardSet.Republic => CardType.RepublicOrderCard,
            CardSet.Imperium => CardType.ImperiumOrderCard,
            CardSet.Promo => CardType.PromoOrderCard,
            CardSet.Standard => CardType.StandardOrderCard,
            _ => throw new InvalidOperationException($"Invalid card set encountered: {CardSet}."),
        };
    }
}
