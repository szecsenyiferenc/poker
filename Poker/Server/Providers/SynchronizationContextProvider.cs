using Poker.Shared.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Poker.Server.Providers
{
    public class SynchronizationContextProvider : ISynchronizationContextProvider
    {
        private SynchronizationContext _synchronizationContext;

        public SynchronizationContext SynchronizationContext
        {
            get { return _synchronizationContext = _synchronizationContext ?? new SynchronizationContext(); }
        }

        public void SetContextIfNeccessary()
        {
            if(!(SynchronizationContext.Current is SynchronizationContext))
            {
                SynchronizationContext.SetSynchronizationContext(SynchronizationContext);
            }
        }
    }
}
