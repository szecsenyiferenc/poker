using Poker.Shared.Enums;
using Poker.Shared.Models.DomainModels;
using Poker.Shared.Proxies;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Poker.Shared.Models.PokerModels
{
    public class Table : IDisposable
    {
        public List<PokerUser> PokerUsers { get; private set; }
        public Game Game { get; private set; }
        public IHubEventEmitter HubEventEmitter { get; private set; }
        public IEventProxy EventProxy { get; private set; }

        public int Id { get; set; }
        public int PlayerNumber { get => PokerUsers.Count; }
        public int MaxNumber { get; set; }
        public string Name { get; set; }
        public bool IsRunning { get => Game != null; }

        private Timer _gameTimer;

        public Table(IHubEventEmitter hubEventEmitter, 
            IEventProxy eventProxy,
            int id, 
            string name)
        {
            Id = id;
            Name = name;
            PokerUsers = new List<PokerUser>();
            MaxNumber = 6;
            HubEventEmitter = hubEventEmitter;
            EventProxy = eventProxy;

            //_gameTimer = new Timer();
            //_gameTimer.Elapsed += StartGame;
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

        public bool RemovePlayer(PokerUser player)
        {
            PokerUsers.Remove(player);
            return true;
        }

        private async void StartGame(object source, EventArgs e)
        {
            try
            {
                await Start();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }

        public async Task Start()
        {
            Game = new Game(this);
            await Game.Start();
        }

        public void Stop()
        {
            Game = null;
        }

        public async Task Next(UserAction userAction = null)
        {
            var result = await Game.Next(userAction);
            if (result != null)
            {
                result.PokerUser.Balance += Game.Pot;

                await HubEventEmitter.SendPokerAction(Game.CreateGameViewModels());

                await Task.Delay(5000);
                Game = new Game(this);
                await Game.Start();
            } 
        }

        public void Dispose()
        {
            //if (_gameTimer?.Enabled != null )
            //{
            //    _gameTimer.Elapsed -= StartGame;
            //}
        }
    }
}
