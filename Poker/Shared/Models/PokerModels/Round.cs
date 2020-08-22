using Poker.Shared.Enums;
using Poker.Shared.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Poker.Shared.Models.PokerModels
{
    public class Round
    {
        public List<PokerUser> Players { get; private set; }
        public RoundType RoundType { get; private set; }

        public Round(List<PokerUser> players, RoundType roundType)
        {
            Players = players;
            RoundType = roundType;
        }

        public async Task Next()
        {

        }
    }
}
