using Microsoft.AspNetCore.SignalR;
using Poker.Shared;
using Poker.Shared.Models.DomainModels;
using Poker.Shared.Models.PokerModels;
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
        private ConcurrentDictionary<string, object> _answers;

        public HubEventEmitter(IHubContext<PokerHub> hubContext)
        {
            _hubContext = hubContext;
            _answers = new ConcurrentDictionary<string, object>();
        }

        public async Task GameStarted(Table table)
        {
            await _hubContext.Clients.Group(table.Id.ToString()).SendAsync("Test", String.Join(',', table.PokerUsers.Select(p => p.Username)));
        }

        public async Task<T2> SendMessageToUser<T1, T2>(PokerUser pokerUser, T1 item) where T1 : class where T2 : class
        {
            string guid = Guid.NewGuid().ToString();
            await _hubContext.Clients.Client(pokerUser.ConnectionId).SendAsync("SendMessageToUser", guid, item);
            return await ReadValueFromDictionary<T2>(guid);
        }

        public async Task FoldCards(PokerUser pokerUser)
        {
            await _hubContext.Clients.Client(pokerUser.ConnectionId.ToString()).SendAsync("Fold");
        }

        public async Task SendStatus(Table table, RoundStatus roundStatus)
        {
            await _hubContext.Clients.Group(table.Id.ToString()).SendAsync("SendStatus", roundStatus);
        }

        private async Task<T> ReadValueFromDictionary<T>(string guid) where T : class
        {
            int counter = 0;
            while (!_answers.ContainsKey(guid))
            {
                if(counter >= 15)
                {
                    return null;
                }
                await Task.Delay(1000);
                counter++;
            }
            var result = _answers[guid] as T;
            _answers.Remove(guid, out object value);
            return result;
        }

        public void SetAnswer(string guid, PokerAction value)
        {
            _answers.TryAdd(guid, value);
        }

        public async Task SendCards(PokerUser pokerUser, List<Card> cards)
        {
            await _hubContext.Clients.Client(pokerUser.ConnectionId.ToString()).SendAsync("SendCards", cards);
        }

        public async Task SendWinner(Table table, string name)
        {
            await _hubContext.Clients.Group(table.Id.ToString()).SendAsync("Test", name);
        }
    }
}
