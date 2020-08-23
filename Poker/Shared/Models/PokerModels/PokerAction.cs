using Poker.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Shared.Models.PokerModels
{

    public class PokerAction
    {
        public PokerActionType PokerActionType { get; set; }
        public int? Value { get; set; }

        public PokerAction()
        {

        }

        public PokerAction(PokerActionType pokerActionType)
        {
            PokerActionType = pokerActionType;
        }

        public PokerAction(PokerActionType pokerActionType, int? value) : this(pokerActionType)
        {
            Value = value;
        }
    }
}
