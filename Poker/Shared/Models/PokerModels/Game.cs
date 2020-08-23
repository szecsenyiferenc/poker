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
        //private Dictionary<PokerUser, List<Card>> _cards;

        public Game(List<PokerUser> players)
        {
            _turnState = 0;
            PokerUsers = players.ToList();
        }

        public async Task Next(IHubEventEmitter hubEventEmitter)
        {
            var cards = new Dictionary<PokerUser, List<Card>>();

            var deck = new Deck();
            deck.Shuffle();

            foreach (var pokerUser in PokerUsers)
            {
                cards[pokerUser] = deck.GetCards(2);
            }

            Round = new Round(PokerUsers, (RoundType)_turnState);

            while (Round.RoundType != RoundType.End)
            {
                Console.WriteLine($"Round type: {(RoundType)_turnState}");
                if(Round.RoundType == RoundType.Flop)
                {
                    // await hubEventEmitter
                }
                await Round.Next(hubEventEmitter);
                _turnState++;
                Round = new Round(PokerUsers, (RoundType)_turnState);
            }
        }
    }
}
