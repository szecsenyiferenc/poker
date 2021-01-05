using Poker.Shared.Enums;
using Poker.Shared.Models.DomainModels;
using Poker.Shared.Models.PokerModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Shared.Models.ViewModels
{
    public class GameViewModel
    {
        public RoundType RoundType { get; set; }
        public int TableId { get; set; }
        public Player Player { get; set; }
        public int MaxRaiseValue { get; set; }
        public PokerUser NextPlayer { get; set; }
        public Winner Winner { get; set; }
        public string CurrentSessionGuid { get; private set; }
        public int MinValue { get; private set; }
        public int MaxValue { get; private set; }
        public int Balance { get => Player.Balance; }
        public int Pot { get; private set; }


        public List<Player> Players { get; set; }

        public List<Card> flop = new List<Card>();
        public List<Card> ownCards = new List<Card>();
        public List<Card> unknown = new List<Card>();
        public List<Card>[] hands = new List<Card>[6];
    }
}
