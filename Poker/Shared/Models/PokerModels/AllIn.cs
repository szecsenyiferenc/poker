using Poker.Shared.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Shared.Models.PokerModels
{
    public class AllIn
    {
        public PokerUser PokerUser { get; set; }
        public int Value { get; set; }
    }
}
