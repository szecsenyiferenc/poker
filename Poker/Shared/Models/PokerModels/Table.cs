using Poker.Shared.Enums;
using Poker.Shared.Models.DomainModels;
using Poker.Shared.Proxies;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Linq;
using Timer = System.Timers.Timer;
using Poker.Shared.Managers;
using Poker.Shared.Models.ViewModels;

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
        private readonly IBalanceManager _balanceManager;

        public Table(IHubEventEmitter hubEventEmitter, 
            IEventProxy eventProxy,
            IBalanceManager balanceManager,
            int id, 
            string name)
        {
            Id = id;
            Name = name;
            PokerUsers = new List<PokerUser>();
            MaxNumber = 6;
            HubEventEmitter = hubEventEmitter;
            EventProxy = eventProxy;
            _balanceManager = balanceManager;

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
            if(Game != null)
            {
                if(userAction != null && userAction.GameId != Game.Id)
                {
                    return;
                }

                var result = await Game.Next(userAction);
                if (result != null)
                {
                    result.PokerUser.Balance += Game.Pot;
                    await UpdateBalance(result.PokerUser);

                    await HubEventEmitter.SendPokerAction(Game.CreateGameViewModels());

                    PokerUsers = PokerUsers.Where(p => p.Balance != 0).ToList();

                    await Task.Delay(2000);

                    if(Game != null)
                    {
                        await HubEventEmitter.SendPokerAction(Game.CreateGameViewModels(true));

                        if (PokerUsers.Count > 1)
                        {
                            await Task.Delay(2000);
                            Game = new Game(this);
                            await Game.Start();
                        }
                        else
                        {
                            await HubEventEmitter.SendPokerAction(Game.CreateGameViewModels(), true);
                            Game = null;
                        }
                    }


                }
            }
        }

        public async Task UpdateBalance(PokerUser pokerUser)
        {
            var user = PokerUsers.FirstOrDefault(p => p.Username == pokerUser.Username);
            user.Balance = pokerUser.Balance;
            await _balanceManager.UpdateBalance(pokerUser);
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
