using Poker.Server.Hubs;
using Poker.Server.Proxies;
using Poker.Shared.Models.DomainModels;
using Poker.Shared.Models.PokerModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poker.Server.Managers
{
    public class TableManager
    {
        private readonly EventProxy _eventProxy;
        private readonly HubEventEmitter _hubEventEmitter;

        public TableManager(EventProxy eventProxy, HubEventEmitter hubEventEmitter)
        {
            _eventProxy = eventProxy;
            _hubEventEmitter = hubEventEmitter;

            _eventProxy.OnGameStarted += StartGame;
        }

        private async void StartGame(object sender, Table table)
        {
            if (!table.IsRunning)
            {
                while (table.PlayerNumber >= 2)
                {
                    await table.Next(_hubEventEmitter);
                }
            }

        }
    }
}
