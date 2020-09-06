using Poker.Shared.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Shared.Models.PokerModels
{
    public class PokerUserWithCards
    {
        public PokerUser PokerUser { get; set; }
        public List<Card> Cards { get; set; }
    }
}
