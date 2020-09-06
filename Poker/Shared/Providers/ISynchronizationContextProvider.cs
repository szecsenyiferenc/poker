using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Shared.Providers
{
    public interface ISynchronizationContextProvider
    {
        public void SetContextIfNeccessary();
    }
}
