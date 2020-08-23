using Poker.Shared.Models.PokerModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poker.Server.Proxies
{
    public class EventProxy
    {
        public event EventHandler<Table> OnGameStarted;

        public EventProxy()
        {
        }

        public void GameStarted(Table table)
        {
            OnGameStarted.Invoke(null, table);
        }
    }
}
