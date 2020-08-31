using Poker.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Shared.Models.DomainModels
{
    public class Card : IComparable
    {
        public CardColor CardColor { get; set; }
        public CardType CardType { get; set; }

        public Card()
        {

        }

        public Card(CardColor cardColor, CardType cardType)
        {
            CardColor = cardColor;
            CardType = cardType;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            Card otherCard = obj as Card;
            if (otherCard != null)
            {
                return CardType.CompareTo(otherCard.CardType);
            }
            else
            {
                throw new ArgumentException("Object is not a Card");
            }
        }

        public override string ToString()
        {
            return $"{CardColor}_{CardType}";
        }
    }

    public class UnknownCard : Card
    {
        public UnknownCard()
        {

        }

        public override string ToString()
        {
            return "Unknown";
        }
    }

}
