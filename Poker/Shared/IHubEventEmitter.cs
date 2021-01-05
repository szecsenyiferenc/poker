using Poker.Shared.Models.DomainModels;
using Poker.Shared.Models.PokerModels;
using Poker.Shared.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Poker.Shared
{
    public interface IHubEventEmitter
    {
        //Task<T2> SendMessageToUser<T1, T2>(PokerUser pokerUser, T1 item) where T1 : class where T2 : class;
        //Task FoldCards(PokerUser pokerUser);
        //Task SendStatus(Table table, RoundStatus roundStatus);
        //Task SendCards(PokerUser pokerUser, List<Card> cards);
        //Task SendWinner(Table table, string name);
        Task SendPokerAction(PokerAction pokerAction);
        Task SendPokerAction(List<GameViewModel> gameViewModel);
    }
}
