using Poker.Shared.Enums;
using Poker.Shared.Managers;
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
        private Dictionary<PokerUser, List<Card>> _cards;
        private Dictionary<PokerUser, Result> _results;

        public Game(List<PokerUser> players)
        {
            _turnState = 0;
            PokerUsers = players.ToList();
        }

        public async Task Next(IHubEventEmitter hubEventEmitter, Table table)
        {
            _cards = new Dictionary<PokerUser, List<Card>>();
            _results = new Dictionary<PokerUser, Result>();

            var deck = new Deck();
            deck.Shuffle();

            foreach (var pokerUser in PokerUsers)
            {
                _cards[pokerUser] = deck.GetCards(2);
                await hubEventEmitter.SendCards(pokerUser, _cards[pokerUser]);
            }

            Round = new Round(PokerUsers, (RoundType)_turnState);

            var RoundStatus = new RoundStatus();

            while (Round.RoundType != RoundType.End)
            {
                Console.WriteLine($"Round type: {(RoundType)_turnState}");
                switch(Round.RoundType)
                {
                    case RoundType.Flop:
                        RoundStatus.Flop = deck.GetCards(3);
                        break;
                    case RoundType.Turn:
                    case RoundType.River:
                        RoundStatus.Flop.Add(deck.GetCards(1).First());
                        break;
                }

                RoundStatus.RoundType = (RoundType)_turnState;
                await hubEventEmitter.SendStatus(table, RoundStatus);
                await Round.Next(hubEventEmitter);
                _turnState++;
                Round = new Round(PokerUsers, (RoundType)_turnState);
            }

            var gm = new GameManager();

            foreach (var item in _cards)
            {
                var playerCards = RoundStatus.Flop.ToList();
                playerCards.AddRange(item.Value);
                _results[item.Key] = gm.GetResult(playerCards);
            }

            var winner = _results.OrderByDescending(a => a.Value).First();

            Console.WriteLine("Winner" + winner.Key.Username);

            await hubEventEmitter.SendWinner(table, $"{winner.Key.Username} with [{winner.Value.Hand}] and with the following values: {winner.Value.Values}");
        }
    }
}
