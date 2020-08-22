using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Shared.Models.DomainModels
{
    public class LoggedInUser
    {
        public string Id { get; set; }
        public PokerUser PokerUser { get; set; }

        public LoggedInUser()
        {

        }

        public LoggedInUser(string id, PokerUser pokerUser)
        {
            Id = id;
            PokerUser = pokerUser;
        }
    }
}
