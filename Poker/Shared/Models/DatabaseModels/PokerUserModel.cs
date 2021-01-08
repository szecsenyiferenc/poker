using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Shared.Models.DatabaseModels
{
    public class PokerUserModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Balance { get; set; }
    }
}
