using Poker.Shared.Models.DomainModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Poker.Server.Providers
{
    public class PokerUserProvider
    {
        ConcurrentDictionary<string, PokerUser> _currentUsers;

        public PokerUserProvider()
        {
            _currentUsers = new ConcurrentDictionary<string, PokerUser>();
        }

        public string Add(PokerUser pokerUser)
        {
            string guid = Guid.NewGuid().ToString();
            var result = _currentUsers.TryAdd(guid, pokerUser);
            if (result)
            {
                Console.WriteLine($"Logged in - {guid} - {pokerUser?.Username}");
                return guid;
            }
            return null;
        }

        public void SetConnectionId(string id, string connectionId)
        {
            if(_currentUsers.TryGetValue(id, out var pokerUser))
            {
                pokerUser.ConnectionId = connectionId;
            }
        }

        public PokerUser GetUser(string id)
        {
            var found = _currentUsers.TryGetValue(id, out PokerUser pokerUser);
            return found ? pokerUser : null;
        }

        public PokerUser GetUser(PokerUser pokerUser)
        {
            return _currentUsers.FirstOrDefault(p => p.Value != null && p.Value.Username == pokerUser.Username).Value;
        }

        public string GetId(PokerUser pokerUser)
        {
            var keyValue = _currentUsers.FirstOrDefault(v => v.Value.Username == pokerUser.Username);
            return keyValue.Equals(default(KeyValuePair<string, PokerUser>)) ? keyValue.Key : null;
        }

        public void Remove(string id)
        {
            if(_currentUsers.TryRemove(id, out PokerUser pokerUser))
            {
                Console.WriteLine($"Logged out - {id} - {pokerUser?.Username}");
            }
        }

        public List<PokerUser> GetAllUsers()
        {
            return _currentUsers.Values.ToList();
        }
    }
}
