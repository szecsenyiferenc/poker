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
        public List<Player> Players { get; private set; }
        public RoundType RoundType { get; private set; }

        private IHubEventEmitter _hubEventEmitter;
        private Game _game;
        private int _currentPlayerIndex;

        private int currentRaise;


        public Round(Game game, RoundType roundType)
        {
            _game = game;
            _hubEventEmitter = _game.HubEventEmitter;
            _currentPlayerIndex = 0;

            Players = game.Players;
            Players.ForEach(p => p.IsDone = false);

            RoundType = roundType;
        }

        public async Task<bool> Next(UserAction userAction = null)
        {
            Player nextPlayer = null;

            if(userAction != null)
            {
                var player = Players.Find(u => u.Username == userAction.PokerUser.Username);
                switch (userAction.UserActionType)
                {
                    case UserActionType.Fold:
                        player.IsActive = false;
                        break;
                    case UserActionType.Raise:
                        currentRaise = userAction.Value;
                        Players.ForEach(p => p.IsDone = false);
                        break;
                }

                player.IsDone = true;
            }


            var currentActivePlayers = Players.Where(p => p.IsActive).ToList();

            if(currentActivePlayers.Count <= 1)
            {
                return true;
            }

            var currentDonePlayers = Players.Where(p => p.IsActive && p.IsDone).ToList();

            if (currentDonePlayers.Count != currentActivePlayers.Count)
            {
                while (nextPlayer == null || !nextPlayer.IsActive)
                {
                    if (_currentPlayerIndex < Players.Count)
                    {
                        nextPlayer = Players[_currentPlayerIndex];
                        if (!nextPlayer.IsActive)
                        {
                            _currentPlayerIndex++;
                        };
                    }
                    else if (_currentPlayerIndex == Players.Count)
                    {
                        _currentPlayerIndex = 0;
                    }
                    else
                    {
                        break;
                    }
                }
            }




            if (nextPlayer != null)
            {
                var sendCardsToAll = new PokerAction(RoundType, _game.Table.Id, null, PokerActionType.NextPlayer);
                sendCardsToAll.NextPlayer = nextPlayer;
                await _hubEventEmitter.SendPokerAction(sendCardsToAll);
                _currentPlayerIndex++;
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
    }
}
