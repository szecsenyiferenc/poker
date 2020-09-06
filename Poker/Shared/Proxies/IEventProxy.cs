using Poker.Shared.Models.PokerModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Shared.Proxies
{
    public interface IEventProxy
    {
        event EventHandler<PokerAction> OnDispatcAction;
        event EventHandler OnStart;
        void DispatchPokerAction(PokerAction pokerAction);
        void Start();
    }
}
