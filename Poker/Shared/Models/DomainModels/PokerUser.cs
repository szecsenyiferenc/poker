using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Shared.Models.DomainModels
{
    public class PokerUser
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ConnectionId { get; set; }
        public int Balance { get; set; }
    }
}
