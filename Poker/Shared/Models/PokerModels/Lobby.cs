using Poker.Shared.Models.PokerModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Shared.Models.DomainModels
{
    public class Lobby
    {
        public List<Table> Tables { get; set; }

        public Lobby()
        {
            Tables = new List<Table>();
            // Tables.Add(new Table());
        }

    }
}
