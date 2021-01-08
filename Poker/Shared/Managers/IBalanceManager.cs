using Poker.Shared.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Poker.Shared.Managers
{
    public interface IBalanceManager
    {
        Task UpdateBalance(PokerUser pokerUser);
    }
}
