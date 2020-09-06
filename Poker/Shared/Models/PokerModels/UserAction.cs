using Poker.Shared.Enums;
using Poker.Shared.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Shared.Models.PokerModels
{
    public class UserAction
    {
        public UserActionType UserActionType { get; set; }
        public PokerUser PokerUser { get; set; }

        public int Value { get; set; }

        public UserAction()
        {

        }

        public UserAction(PokerUser pokerUser, UserActionType userActionType)
        {
            PokerUser = pokerUser;
            UserActionType = userActionType;
        }

        public UserAction(PokerUser pokerUser, UserActionType userActionType, int value) : this(pokerUser, userActionType)
        {
            Value = value;
        }
    }
}
