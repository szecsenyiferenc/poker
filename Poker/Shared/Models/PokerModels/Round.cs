using Poker.Shared.Enums;
using Poker.Shared.Models.DomainModels;
using Poker.Shared.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker.Shared.Models.PokerModels
{
    public class Round
    {
        public string Id { get; set; }
        public List<Player> Players { get; private set; }
        public RoundType RoundType { get; private set; }
        public int CurrentRaise { get; private set; }
        public AllIn AllIn { get; set; }
        public bool IsOpenRound { get => Players.All(p => p.IsInAllIn); }
        public bool IsDone { get => Players.All(p => p.IsDone); }

        private IHubEventEmitter _hubEventEmitter;
        private Game _game;
        

        public Round(Game game, RoundType roundType)
        {
            _game = game;
            _hubEventEmitter = _game.HubEventEmitter;

            Id = Guid.NewGuid().ToString();

            _game.CurrentPlayer = _game.StartingPlayer;

            Players = game.Players;
            Players.ForEach(p => p.IsDone = false);
            Players.ForEach(p => p.CurrentRaise = 0);

            AllIn = null;

            RoundType = roundType;
        }

        public async Task<bool> Next(UserAction userAction = null)
        {
            if (_game.CurrentPlayer == null)
            {
                return true;
            }

            Player nextPlayer = null;
            int tempRaise;
            var currentIndex = Players.IndexOf(_game.CurrentPlayer);

            if (userAction != null)
            {
                var player = _game.CurrentPlayer;

                if (userAction.UserActionType == UserActionType.Call && player.Balance < CurrentRaise - player.CurrentRaise)
                {
                    userAction.UserActionType = UserActionType.AllIn;
                }

                switch (userAction.UserActionType)
                {
                    case UserActionType.Fold:
                        player.IsActive = false;
                        break;
                    case UserActionType.Call:
                        tempRaise = player.CurrentRaise;

                        player.CurrentRaise = CurrentRaise;
                        player.Balance -= (player.CurrentRaise - tempRaise);

                        await UpdateBalance(player);

                        _game.Pot += (player.CurrentRaise - tempRaise);

                        await _hubEventEmitter.SendPokerAction(_game.CreateGameViewModels());
                        break;
                    case UserActionType.Raise:
                        tempRaise = player.CurrentRaise;

                        player.CurrentRaise = userAction.Value + tempRaise;
                        player.Balance -= (player.CurrentRaise - tempRaise);

                        CurrentRaise = player.CurrentRaise;

                        await UpdateBalance(player);

                        _game.Pot += (player.CurrentRaise - tempRaise);
                        Players.ForEach(p => p.IsDone = false);

                        await _hubEventEmitter.SendPokerAction(_game.CreateGameViewModels());
                        break;
                    case UserActionType.AllIn:
                        player.CurrentRaise = player.Balance;
                        player.Balance -= player.CurrentRaise;
                        player.IsInAllIn = true;
                        CurrentRaise = player.CurrentRaise;

                        await UpdateBalance(player);

                        _game.Pot += player.CurrentRaise;
                        Players.ForEach(p => p.IsDone = false);

                        AllIn = new AllIn()
                        {
                            PokerUser = player,
                            Value = CurrentRaise
                        };

                        foreach (var p in Players)
                        {
                            if (p.CurrentRaise > CurrentRaise)
                            {
                                var diff = p.CurrentRaise - CurrentRaise;
                                p.Balance += diff;
                                _game.Pot -= diff;
                                p.CurrentRaise = CurrentRaise;
                                p.IsDone = true;
                                p.IsInAllIn = true;
                                await UpdateBalance(p);
                            }
                        }

                        await _hubEventEmitter.SendPokerAction(_game.CreateGameViewModels());
                        break;
                }

                player.IsDone = true;

                if (currentIndex < Players.Count)
                {
                    currentIndex++;
                }
                else if (currentIndex == Players.Count)
                {
                    currentIndex = 0;
                }
            }


            var currentActivePlayers = Players.Where(p => p.IsActive).ToList();

            if (currentActivePlayers.Count <= 1)
            {
                return true;
            }

            var currentDonePlayers = Players.Where(p => p.IsActive && p.IsDone).ToList();

            if (currentDonePlayers.Count != currentActivePlayers.Count)
            {
                while (nextPlayer == null || !nextPlayer.IsActive)
                {
                    if (currentIndex < Players.Count)
                    {
                        nextPlayer = Players[currentIndex];
                        if (!nextPlayer.IsActive)
                        {
                            currentIndex++;
                        };
                    }
                    else if (currentIndex == Players.Count)
                    {
                        currentIndex = 0;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (nextPlayer != null)
            {
                _game.CurrentPlayer = nextPlayer;
                await _hubEventEmitter.SendPokerAction(_game.CreateGameViewModels());
                return false;
            }

            return true;





            //_eventProxy.DispatchPokerAction()

            //foreach (var player in Players)
            //{
            //    var result = await hubEventEmitter.SendMessageToUser<object, PokerAction>(player, null);
            //    if(result == null)
            //    {
            //        result = new PokerAction(PokerActionType.Fold);
            //        await hubEventEmitter.FoldCards(player);
            //    }
            //    Console.WriteLine($"{player.Username} action: {result.PokerActionType}");
            //}
        }

        private async Task UpdateBalance(PokerUser pokerUser)
        {
            await _game.Table.UpdateBalance(pokerUser);
        }
    }
}
