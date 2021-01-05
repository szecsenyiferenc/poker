using Poker.Shared.Enums;
using Poker.Shared.Models.DomainModels;
using Poker.Shared.Models.PokerModels;
using System;
using System.Collections.Generic;
<<<<<<< HEAD
=======
using System.Linq;
>>>>>>> ba7cff5... WIP
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
<<<<<<< HEAD
=======
        public PokerUser CurrentPlayer { get; set; }
>>>>>>> ba7cff5... WIP
        public Winner Winner { get; set; }
        public string CurrentSessionGuid { get; private set; }
        public int MinValue { get; private set; }
        public int MaxValue { get; private set; }
        public int Balance { get => Player.Balance; }
        public int Pot { get; private set; }
<<<<<<< HEAD
=======
        public List<Card> MyCards { get; set; }
        public List<Card> CommonCards { get; set; }
        public Dictionary<string, List<Card>> OtherCards { get; set; }

        public bool IsMyTurn { get; set; }
>>>>>>> ba7cff5... WIP


        public List<Player> Players { get; set; }

        public List<Card> flop = new List<Card>();
        public List<Card> ownCards = new List<Card>();
        public List<Card> unknown = new List<Card>();
        public List<Card>[] hands = new List<Card>[6];
<<<<<<< HEAD
=======

        public override string ToString()
        {
            return $"RoundType:{RoundType}, TableId:{TableId}, Player:{Player?.Username}, " + 
                $"MaxRaise:{MaxRaiseValue}, NextPlayer:{NextPlayer?.Username}, " + 
                $"Winner:{Winner?.PokerUser.Username},CurrentSessionGuid: {CurrentSessionGuid}" +
                $"CurrentPlayer:{CurrentPlayer}, MyCards:{MyCards.Select(c => c.ToString()).Aggregate((a, v) => a += "," + v)}, " +
                //$"CommonCards:{CommonCards.Select(c => c.ToString()).Aggregate((a, v) => a += "," + v)}" +
                $"Players:[{Players.Select(p => p.Username).Aggregate((a,v) => a += "," + v)}]" + 
                $"Cards:[{OtherCards.Select((k) => $"{k.Key}: {k.Value.FirstOrDefault()}, {k.Value.LastOrDefault()}").Aggregate((a,v) => a += "," + v)}]";
        }

>>>>>>> ba7cff5... WIP
    }
}
