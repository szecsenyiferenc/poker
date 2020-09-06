using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;
using Poker.Server.Managers;
using Poker.Server.Providers;
using Poker.Server.Proxies;
using Poker.Server.Services;
using Poker.Shared;
using Poker.Shared.Models.DomainModels;
using Poker.Shared.Models.PokerModels;
using Poker.Shared.Proxies;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Poker.Server.Hubs
{
    public class PokerHub : Hub
    {
        private Lobby _lobby;
        private DatabaseService _databaseService;
        private TableProvider _tableProvider;
        private PokerUserProvider _pokerUserProvider;
        private IEventProxy _eventProxy;
        private TableManager _tableManager;
        private IHubEventEmitter _hubEventEmitter;

        List<string> ids = new List<string>();

        public PokerHub( 
            DatabaseService databaseService,
            TableProvider tableProvider,
            PokerUserProvider pokerUserProvider,
            IEventProxy eventProxy,
            TableManager tableManager,
            IHubEventEmitter hubEventEmitter)
        {
            _databaseService = databaseService;
            _tableProvider = tableProvider;
            _pokerUserProvider = pokerUserProvider;
            _eventProxy = eventProxy;
            _tableManager = tableManager;
            _hubEventEmitter = hubEventEmitter;


        }



        public async Task InitConnectionWithClient()
        {
            await Clients.Caller.SendAsync("GetTables", _tableProvider.GetAllTableViews());
        }

        public async Task SendUserAction(UserAction userAction)
        {
            var pokerUser = _pokerUserProvider.GetUser(userAction.PokerUser);
            var currentTable = _tableProvider.GetCurrentTable(pokerUser);
            await currentTable.Next(userAction);
        }

        #region Table actions

        public async Task AddTable()
        {
            _tableProvider.IncrementTables();
            await Clients.All.SendAsync("GetTables", _tableProvider.GetAllTableViews());
        }

        public async Task GetTables()
        {
            await Clients.Caller.SendAsync("GetTables", _tableProvider.GetAllTableViews());
        }

        public async Task JoinToTable(int tableId, PokerUser rawPokerUser)
        {
            var pokerUser = _pokerUserProvider.GetUser(rawPokerUser);
            _tableProvider.JoinToTable(tableId, pokerUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, tableId.ToString());
            await Clients.All.SendAsync("GetTables", _tableProvider.GetAllTableViews());

            var currentTable = _tableProvider.GetCurrentTable(tableId);
            await Clients.Group(tableId.ToString()).SendAsync("PlayerStatus", currentTable.PokerUsers);

            await Task.Delay(5000);

            if (currentTable.PokerUsers.Count >= 2 && !currentTable.IsRunning)
            {
                await currentTable.Start();
            }
        }

        public async Task LeaveTable(int tableId, PokerUser rawPokerUser)
        {
            var pokerUser = _pokerUserProvider.GetUser(rawPokerUser);
            if(pokerUser == null)
            {
                return;
            }
            _tableProvider.LeaveTable(tableId, pokerUser);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, tableId.ToString());
            await Clients.All.SendAsync("GetTables", _tableProvider.GetAllTableViews());
        }

        #endregion

        #region ConnectionFunctions

        // Connection functions
        public override async Task OnConnectedAsync()
        {
            var currentUser = GetCurrentUser();

            // await InitConnectionWithClient();

            Console.WriteLine("SignalR - connected " + currentUser?.Username);

            currentUser.ConnectionId = Context.ConnectionId;

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var currentUser = GetCurrentUser();

            if (currentUser != null)
            {
                var tableId = _tableProvider.LeaveTable(currentUser);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, tableId.ToString());

                await Clients.All.SendAsync("GetTables", _tableProvider.GetAllTableViews());
            }

            Console.WriteLine("SignalR - disconnect " + currentUser?.Username);

            await base.OnDisconnectedAsync(exception);
        }

        #endregion

        #region Private functions
        // private methods 
        private PokerUser GetCurrentUser()
        {
            var param = Context.GetHttpContext().Request.Query.FirstOrDefault(v => v.Key == "auth");

            if (!param.Equals(default(KeyValuePair<string, StringValues>)))
            {
                var user = _pokerUserProvider.GetUser(param.Value);
                _pokerUserProvider.SetConnectionId(param.Value, Context.ConnectionId);
                return user;
            }
            return null;
        }

        #endregion
    }
}
