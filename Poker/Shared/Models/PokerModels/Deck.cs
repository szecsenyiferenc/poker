using Poker.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poker.Shared.Models.DomainModels
{
    public class Deck
    {
        public List<Card> Cards { get; set; }

        public Deck()
        {
            Cards = FillDeck();
        }

        public void Shuffle()
        {
            Cards = Cards.OrderBy(a => new Random().Next(100000)).ToList();
        }

        public List<Card> GetFlop()
        {
            var returnedCards = Cards.Take(5).ToList();
            Cards = Cards.Skip(5).ToList();
            return returnedCards;
        }

        public List<Card> GetCards()
        {
            var returnedCards = Cards.Take(2).ToList();
            Cards = Cards.Skip(2).ToList();
            return returnedCards;
        }

        private List<Card> FillDeck()
        {
            var cards = new List<Card>();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 2; j < 15; j++)
                {
                    cards.Add(new Card((CardColor)i, (CardType)j));
                }
            }
            return cards;
        }
    }
}
