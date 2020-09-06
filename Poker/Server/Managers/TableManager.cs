using Poker.Server.Hubs;
using Poker.Server.Proxies;
using Poker.Shared;
using Poker.Shared.Models.DomainModels;
using Poker.Shared.Models.PokerModels;
using Poker.Shared.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poker.Server.Managers
{
    public class TableManager
    {
        private readonly IEventProxy _eventProxy;
        private readonly IHubEventEmitter _hubEventEmitter;

        public TableManager(IEventProxy eventProxy, IHubEventEmitter hubEventEmitter)
        {
            _eventProxy = eventProxy;
            _hubEventEmitter = hubEventEmitter;
        }

        //private async void StartGame(object sender, Table table)
        //{
        //    if (!table.IsRunning)
        //    {
        //        while (table.PlayerNumber >= 2)
        //        {
        //            await table.Next(_hubEventEmitter);
        //        }
        //    }
        //}
    }
}
