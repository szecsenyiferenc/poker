using Poker.Shared.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Shared.Models.PokerModels
{
    public class Player
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public List<Card> Cards { get; set; }
    }
}
