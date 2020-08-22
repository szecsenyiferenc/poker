using Poker.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Poker.Shared.Models.PokerModels
{
    public class Round
    {
        public List<Player> Players { get; private set; }
        public RoundType RoundType { get; private set; }

        public Round(List<Player> players, RoundType roundType)
        {
            Players = players;
            RoundType = roundType;
        }

        public async Task Next()
        {

        }
    }
}
