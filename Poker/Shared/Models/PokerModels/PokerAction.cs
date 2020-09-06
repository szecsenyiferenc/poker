using Newtonsoft.Json;
using Poker.Shared.Enums;
using Poker.Shared.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Shared.Models.PokerModels
{

    public class PokerAction
    {
        public RoundType RoundType { get => (RoundType) RoundTypeIntValue; }
        public PokerActionType PokerActionType { get => (PokerActionType)PokerActionTypeIntValue; }

        public int RoundTypeIntValue { get; set; }
        public int PokerActionTypeIntValue { get; set; }
        public int TableId { get; set; }
        public int CurrentRaise { get; set; }
        public PokerUser NextPlayer { get; set; }
        public Winner Winner { get; set; }
        public List<PokerUserWithCards> Targets { get; set; }
        public List<Card> Cards { get; set; }
        public Player PlayerWithRaise { get; set; }
        public int Pot { get; set; }

        public PokerAction()
        {
            Targets = new List<PokerUserWithCards>();
            Cards = new List<Card>();
        }

        public PokerAction(
            RoundType roundType,
            int tableId,
            List<PokerUserWithCards> targets,
            PokerActionType pokerActionType = 0)
        {
            RoundTypeIntValue = (int)roundType;
            TableId = tableId;
            Targets = targets;
            Cards = new List<Card>();
            PokerActionTypeIntValue = (int)pokerActionType;
        }

       

    }
}
