using Poker.Shared.Models.PokerModels;
using Poker.Shared.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poker.Server.Proxies
{
    public class EventProxy : IEventProxy
    {
        public event EventHandler<PokerAction> OnDispatcAction;
        public event EventHandler OnStart;

        public EventProxy()
        {
        }

        public void DispatchPokerAction(PokerAction pokerAction)
        {
            OnDispatcAction?.Invoke(null, pokerAction);
        }

        public void Start()
        {
            OnStart?.Invoke(null, null);
        }
    }
}
