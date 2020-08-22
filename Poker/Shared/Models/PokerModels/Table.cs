using Poker.Shared.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Poker.Shared.Models.PokerModels
{
    public class Table
    {
        public List<Player> Players { get; private set; }
        public Game Game { get; private set; }

        public int Id { get; set; }
        public int PlayerNumber { get => Players.Count; }
        public int MaxNumber { get; set; }
        public string Name { get; set; }

        public Table(int id, string name)
        {
            Id = id;
            Name = name;
            Players = new List<Player>();
            MaxNumber = 6;
        }

        public bool AddPlayer(Player player)
        {
            if (Players.Count == 6)
            {
                return false;
            }

            Players.Add(player);
            return true;
        }

        public async Task Next()
        {
            Game = new Game(Players);
            await Game.Next();
        }

    }
}
