using Poker.Shared.Enums;
using Poker.Shared.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Shared.Models.PokerModels
{
    public class RoundStatus
    {
        public List<Card> Flop { get; set; }
        public List<Card> Turn { get; set; }
        public List<Card> River { get; set; }
        public RoundType RoundType { get; set; }
    }
}
