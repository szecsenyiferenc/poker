using Microsoft.AspNetCore.SignalR;
using Poker.Shared;
using Poker.Shared.Enums;
using Poker.Shared.Models.DomainModels;
using Poker.Shared.Models.PokerModels;
using Poker.Shared.Models.ViewModels;
using Poker.Shared.Proxies;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poker.Server.Hubs
{
    public class HubEventEmitter : IHubEventEmitter
    {
        private IHubContext<PokerHub> _hubContext;
        private IEventProxy _eventProxy;
        private ConcurrentDictionary<string, object> _answers;

        public HubEventEmitter(IHubContext<PokerHub> hubContext, 
            IEventProxy eventProxy)
        {
            _hubContext = hubContext;
            _answers = new ConcurrentDictionary<string, object>();

        }

        public async Task SendPokerAction(PokerAction pokerAction)
        {
            //switch (pokerAction.PokerActionType)
            //{
            //    case PokerActionType.StartingCards:
            //        foreach (var target in pokerAction.Targets)
            //        {
            //            var newTargets = new List<PokerUserWithCards>()
            //            {
            //                target
            //            };

            //            var newPokerAction = new PokerAction(pokerAction.RoundType,
            //                pokerAction.TableId, newTargets, PokerActionType.StartingCards);
            //            await _hubContext.Clients.Client(target.PokerUser.ConnectionId).SendAsync("SendPokerAction", newPokerAction);
            //        }
            //        break;
            //    default:
            //        await _hubContext.Clients.Group(pokerAction.TableId.ToString()).SendAsync("SendPokerAction", pokerAction);
            //        break;
<<<<<<< HEAD
            //}
            
                
            
=======
            //} 
>>>>>>> cd72d80 (WIP)
        }

        public async Task SendPokerAction(List<GameViewModel> gameViewModels)
        {
            foreach (var gameViewModel in gameViewModels)
            {
<<<<<<< HEAD
<<<<<<< HEAD
=======
                Console.WriteLine($"SendPokerAction - {gameViewModel.Player.Username}");
>>>>>>> ba7cff5... WIP
=======
                Console.WriteLine($"SendPokerAction - {gameViewModel.Player.Username}");
>>>>>>> cd72d80 (WIP)
                await _hubContext.Clients.Client(gameViewModel.Player.ConnectionId).SendAsync("SendPokerAction", gameViewModel);
            }
        }
    }
}
