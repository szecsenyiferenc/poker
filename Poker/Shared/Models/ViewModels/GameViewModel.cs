using Poker.Shared.Enums;
using Poker.Shared.Models.DomainModels;
using Poker.Shared.Models.PokerModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poker.Shared.Models.ViewModels
{
    public class GameViewModel
    {
        public string GameId { get; set; }
        public string RoundId { get; set; }


        public RoundType RoundType { get; set; }
        public int TableId { get; set; }
        public Player Player { get; set; }
        public int MaxRaiseValue { get; set; }
        public PokerUser NextPlayer { get; set; }
        public PokerUser CurrentPlayer { get; set; }
        public Winner Winner { get; set; }
        public int MinValue { get; set; }
        public int CurrentRaise { get; set; }
        public int AllInValue { get; set; }
        public int MaxValue { get => Player.Balance; }
        public int Balance { get => Player.Balance; }
        public int Pot { get; set; }
        public List<Card> MyCards { get; set; }
        public List<Card> CommonCards { get; set; }
        public Dictionary<string, List<Card>> OtherCards { get; set; }

        public bool IsMyTurn { get; set; }
        public bool IsRaiseInProgess { get; set; }
        public bool IsAllIn { get; set; }
        public bool LeaveTable { get; set; }
        public bool Finished { get; set; }


        public List<Player> Players { get; set; }

        public List<Card> flop = new List<Card>();
        public List<Card> ownCards = new List<Card>();
        public List<Card> unknown = new List<Card>();
        public List<Card>[] hands = new List<Card>[6];

        public override string ToString()
        {
            return $"RoundType:{RoundType}, TableId:{TableId}, Player:{Player?.Username}, " + 
                $"MaxRaise:{MaxRaiseValue}, NextPlayer:{NextPlayer?.Username}, " + 
                $"Winner:{Winner?.PokerUser.Username}" +
                $"CurrentPlayer:{CurrentPlayer}, MyCards:{MyCards.Select(c => c.ToString()).Aggregate((a, v) => a += "," + v)}, " +
                //$"CommonCards:{CommonCards.Select(c => c.ToString()).Aggregate((a, v) => a += "," + v)}" +
                $"Players:[{Players.Select(p => p.Username).Aggregate((a,v) => a += "," + v)}]" + 
                $"Cards:[{OtherCards.Select((k) => $"{k.Key}: {k.Value.FirstOrDefault()}, {k.Value.LastOrDefault()}").Aggregate((a,v) => a += "," + v)}]";
        }

    }
}
