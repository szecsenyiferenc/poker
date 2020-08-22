using Poker.Shared.Enums;
using Poker.Shared.Models.PokerModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker.Shared.Models.DomainModels
{
    public class Game
    {
        private int _turnState;

        public List<PokerUser> PokerUsers { get; private set; }
        public Round Round { get; private set; }

        public Game(List<PokerUser> players)
        {
            _turnState = 0;
            PokerUsers = players.ToList();
        }

        public async Task Next()
        {
            while (Round.RoundType != RoundType.End)
            {
                Round = new Round(PokerUsers, (RoundType)_turnState);
                await Round.Next();
            }
        }
    }
}
