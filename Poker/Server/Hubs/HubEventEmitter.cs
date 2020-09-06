using Microsoft.AspNetCore.SignalR;
using Poker.Shared;
using Poker.Shared.Enums;
using Poker.Shared.Models.DomainModels;
using Poker.Shared.Models.PokerModels;
using Poker.Shared.Providers;
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
        private ISynchronizationContextProvider _synchronizationContextProvider;
        private ConcurrentDictionary<string, object> _answers;

        public HubEventEmitter(IHubContext<PokerHub> hubContext, 
            IEventProxy eventProxy, 
            ISynchronizationContextProvider synchronizationContextProvider)
        {
            _hubContext = hubContext;
            _synchronizationContextProvider = synchronizationContextProvider;
            _answers = new ConcurrentDictionary<string, object>();

        }


      

        public async Task SendPokerAction(PokerAction pokerAction)
        {
            if (pokerAction.PokerActionType == PokerActionType.RoundUpdate || pokerAction.PokerActionType == PokerActionType.NextPlayer)
            {
                await _hubContext.Clients.Group(pokerAction.TableId.ToString()).SendAsync("SendPokerAction", pokerAction);
            }
            if (pokerAction.PokerActionType == PokerActionType.StartingCards)
            {
                foreach (var target in pokerAction.Targets)
                {
                    var newTargets = new List<PokerUserWithCards>()
                    {
                        target
                    };

                    var newPokerAction = new PokerAction(pokerAction.RoundType,
                        pokerAction.TableId, newTargets, PokerActionType.StartingCards);
                    await _hubContext.Clients.Client(target.PokerUser.ConnectionId).SendAsync("SendPokerAction", newPokerAction);
                }
            }
        }

    }
}
