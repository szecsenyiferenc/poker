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

        public async Task SendPokerAction(List<GameViewModel> gameViewModels, bool isGameNull = false)
        {
            foreach (var gameViewModel in gameViewModels)
            {
                Console.WriteLine($"SendPokerAction - {gameViewModel.Player.Username}");
                if (isGameNull)
                {
                    await _hubContext.Clients.Client(gameViewModel.Player.ConnectionId).SendAsync("SendPokerAction", null);
                }
                else
                {
                    await _hubContext.Clients.Client(gameViewModel.Player.ConnectionId).SendAsync("SendPokerAction", gameViewModel);
                }
            }
        }
    }
}
