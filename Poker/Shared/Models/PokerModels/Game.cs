﻿using Poker.Shared.Enums;
using Poker.Shared.Managers;
using Poker.Shared.Models.PokerModels;
using Poker.Shared.Models.ViewModels;
using Poker.Shared.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Poker.Shared.Models.DomainModels
{
    public class Game : IDisposable
    {
        private int _turnState;

        public List<Player> Players { get; private set; }
        public Player CurrentPlayer { get; set; }
        public Player StartingPlayer { get; set; }
        public Round Round { get; private set; }
        public IHubEventEmitter HubEventEmitter { get; private set; }
        public IEventProxy EventProxy { get; private set; }
        public Table Table { get; private set; }
        public int Pot { get; set; }
        public Winner Winner { get; set; }

        private Dictionary<PokerUser, List<Card>> _cards;
        private List<Card> _commonCards;
        private Deck _deck;
        private Timer _gameTimer;

        


        public Game(Table table)
        {
            _turnState = 0;
            Table = table;
            _cards = new Dictionary<PokerUser, List<Card>>();

            HubEventEmitter = table.HubEventEmitter;
            EventProxy = table.EventProxy;
            Players = table.PokerUsers.Select(p => new Player(p)).ToList();

            //_gameTimer = new Timer();
            //_gameTimer.Interval = 5000;
            //_gameTimer.Elapsed += StartGame;
        }

        public async Task Start()
        {

            if (Round == null)
            {
                _deck = new Deck();
                _deck.Shuffle();

<<<<<<< HEAD
<<<<<<< HEAD
                var gameViewModels = new List<GameViewModel>();
=======
                Winner = null;
>>>>>>> cd72d80 (WIP)

                foreach (var pokerUser in Players)
                {
                    _cards[pokerUser] = _deck.GetCards(2);
                }

<<<<<<< HEAD
                //var sendCardsToAll = new PokerAction(RoundType.Start, Table.Id, pokerUsersWithCards, PokerActionType.StartingCards);
=======
                Winner = null;

                foreach (var pokerUser in Players)
                {
                    _cards[pokerUser] = _deck.GetCards(2);
                }

=======
>>>>>>> cd72d80 (WIP)
                CurrentPlayer = Players[0];
                StartingPlayer = CurrentPlayer;

                var gameViewModels = CreateGameViewModels();
<<<<<<< HEAD
>>>>>>> ba7cff5... WIP
=======
>>>>>>> cd72d80 (WIP)

                await HubEventEmitter.SendPokerAction(gameViewModels);

                await Task.Delay(100);

                Round = new Round(this, RoundType.Start);

                await Round.Next();
            }
        }

        public List<GameViewModel> CreateGameViewModels()
        {
            var gameViewModels = new List<GameViewModel>();

            foreach (var pokerUser in Players)
            {
                var gmvm = new GameViewModel()
                {
                    RoundType = Round != null ? Round.RoundType : RoundType.Start,
                    TableId = Table.Id,
                    CurrentPlayer = CurrentPlayer,
                    Player = pokerUser,
                    MyCards = _cards[pokerUser],
                    OtherCards = Players.ToDictionary(p => p.Username, k => new List<Card>() { new UnknownCard(), new UnknownCard() }),
                    CommonCards = _commonCards,
                    Players = Players,
                    IsMyTurn = CurrentPlayer == pokerUser,
                    Winner = Winner
                };

                if (pokerUser.IsActive)
                {
                    gmvm.OtherCards[pokerUser.Username] = gmvm.MyCards;
                }

                if(Winner != null)
                {
                    CurrentPlayer = null;
                    gmvm.OtherCards = Players.ToDictionary(p => p.Username, k => _cards[k]);
                }

                gameViewModels.Add(gmvm); 
            }

            return gameViewModels;
        }

        public async Task<Winner> Next(UserAction userAction = null)
        {
            var result = await Round.Next(userAction);

            if (result)
            {
                int nextRoundType = (int)Round.RoundType;
                nextRoundType++;

                int currentPlayerNumber = Players.Where(p => p.IsActive).ToList().Count;

                if(currentPlayerNumber == 1)
                {
                    var player = Players.FirstOrDefault(p => p.IsActive);
                    Winner = new Winner(player, null);
                    return Winner;
                }

                if (nextRoundType == (int)RoundType.End)
                {
<<<<<<< HEAD
<<<<<<< HEAD
                    return GetWinner(_cards);
=======
                    Winner = GetWinner(_cards);
                    return Winner;
>>>>>>> ba7cff5... WIP
=======
                    Winner = GetWinner(_cards);
                    return Winner;
>>>>>>> cd72d80 (WIP)
                }

                Round = new Round(this, (RoundType)nextRoundType);

                List<Card> nextCards = null;

                switch (Round.RoundType)
                {
                    case RoundType.Flop:
                        nextCards = _deck.GetCards(3);
                        _commonCards = nextCards.ToList();
                        break;
                    case RoundType.Turn:
                    case RoundType.River:
                        nextCards = _deck.GetCards(1);
                        _commonCards.AddRange(nextCards);
                        break;
                }


                await HubEventEmitter.SendPokerAction(CreateGameViewModels());

                await Task.Delay(100);

                await Round.Next();
                return null;
            }
            else
            {
                return null;
            }












            //if(Round == null)
            //{
            //    var deck = new Deck();
            //    deck.Shuffle();

            //    foreach (var pokerUser in PokerUsers)
            //    {
            //        _cards[pokerUser] = deck.GetCards(2);
            //    }

            //    var sendCardsToAll = new PokerAction(RoundType.Start, _table.Id, PokerUsers, _cards, PokerActionType.User);

            //    EventProxy.DispatchPokerAction(sendCardsToAll);

            //    Round = new Round(this, RoundType.Start);
            //}


            //Round = new Round(PokerUsers, (RoundType)_turnState);

            var RoundStatus = new RoundStatus();

            //while (Round.RoundType != RoundType.End)
            //{
            //    Console.WriteLine($"Round type: {(RoundType)_turnState}");
            //    switch(Round.RoundType)
            //    {
            //        case RoundType.Flop:
            //            RoundStatus.Flop = deck.GetCards(3);
            //            break;
            //        case RoundType.Turn:
            //        case RoundType.River:
            //            RoundStatus.Flop.Add(deck.GetCards(1).First());
            //            break;
            //    }

            //    RoundStatus.RoundType = (RoundType)_turnState;
            //    await _hubEventEmitter.SendStatus(_table, RoundStatus);
            //    await Round.Next(_hubEventEmitter);
            //    _turnState++;
            //    Round = new Round(PokerUsers, (RoundType)_turnState);
            //}

            //var gm = new GameManager();

            //foreach (var item in _cards)
            //{
            //    var playerCards = RoundStatus.Flop.ToList();
            //    playerCards.AddRange(item.Value);
            //    _results[item.Key] = gm.GetResult(playerCards);
            //}

            //var winner = _results.OrderByDescending(a => a.Value).First();

            //Console.WriteLine("Winner" + winner.Key.Username);

            //await _hubEventEmitter.SendWinner(_table, $"{winner.Key.Username} with [{winner.Value.Hand}] and with the following values: {winner.Value.Values.Select(v => v.ToString())}");
        }

        public Winner GetWinner(Dictionary<PokerUser, List<Card>> cards)
        {
            var gm = new GameManager();
            var results = new Dictionary<PokerUser, Result>();

            cards = cards.Where(c => Players.Where(p => p.IsActive).ToList().Any(p => p.Username == c.Key.Username)).ToDictionary(t => t.Key, t => t.Value);

            foreach (var item in cards)
            {
                var playerCards = _commonCards.ToList();
                playerCards.AddRange(item.Value);
                results[item.Key] = gm.GetResult(playerCards);
            }
            var winner = results.OrderByDescending(a => a.Value).First();
            return new Winner(winner.Key, winner.Value);
        }

        public void Dispose()
        {
            //_gameTimer.Elapsed -= StartGame;
        }
    }
}
