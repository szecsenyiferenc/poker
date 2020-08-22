using Poker.Shared.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Poker.Shared.Models.PokerModels
{
    public class Table
    {
        public List<PokerUser> PokerUsers { get; private set; }
        public Game Game { get; private set; }

        public int Id { get; set; }
        public int PlayerNumber { get => PokerUsers.Count; }
        public int MaxNumber { get; set; }
        public string Name { get; set; }

        public Table(int id, string name)
        {
            Id = id;
            Name = name;
            PokerUsers = new List<PokerUser>();
            MaxNumber = 6;
        }

        public bool AddPlayer(PokerUser player)
        {
            if (PokerUsers.Count == 6)
            {
                return false;
            }

            PokerUsers.Add(player);
            return true;
        }

        public async Task Next()
        {
            Game = new Game(PokerUsers);
            await Game.Next();
        }

    }
}
