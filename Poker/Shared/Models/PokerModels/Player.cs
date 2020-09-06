using Poker.Shared.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Shared.Models.PokerModels
{
    public class Player : PokerUser
    {
        public bool IsActive { get; set; }
        public bool IsDone { get; set; }
        public int CurrentRaise { get; set; }
        public List<Card> Cards { get; set; }

        public Player()
        {

        }

        public Player(PokerUser pokerUser)
        {
            Username = pokerUser.Username;
            FirstName = pokerUser.FirstName;
            LastName = pokerUser.LastName;
            ConnectionId = pokerUser.ConnectionId;
            Balance = pokerUser.Balance;
            IsActive = true;
            IsDone = false;
            CurrentRaise = 0;
            Cards = new List<Card>();
        }
    }
}
