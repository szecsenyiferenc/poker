using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;
using Poker.Server.Managers;
using Poker.Server.Providers;
using Poker.Server.Proxies;
using Poker.Server.Services;
using Poker.Shared.Models.DomainModels;
using Poker.Shared.Models.PokerModels;
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
        private EventProxy _eventProxy;
        private TableManager _tableManager;
        private HubEventEmitter _hubEventEmitter;

        List<string> ids = new List<string>();

        public PokerHub( 
            DatabaseService databaseService,
            TableProvider tableProvider,
            PokerUserProvider pokerUserProvider,
            EventProxy eventProxy,
            TableManager tableManager,
            HubEventEmitter hubEventEmitter)
        {
            _databaseService = databaseService;
            _tableProvider = tableProvider;
            _pokerUserProvider = pokerUserProvider;
            _eventProxy = eventProxy;
            _tableManager = tableManager;
            _hubEventEmitter = hubEventEmitter;
        }

        // Server functions
        //public async Task GetUsers()
        //{
        //    await Clients.All.SendAsync("Users", _pokerUserProvider.GetAllUsers());
        //}

        //public async Task GetTables()
        //{
        //    await Clients.All.SendAsync("Users", _pokerUserProvider.GetAllUsers());
        //}



        public async Task InitConnectionWithClient()
        {
            await Clients.Caller.SendAsync("GetTables", _tableProvider.GetAllTableViews());
        }


        public async Task AddTable()
        {
            _tableProvider.IncrementTables();
            await Clients.All.SendAsync("GetTables", _tableProvider.GetAllTableViews());
        }

        public async Task GetTables()
        {
            await Clients.Caller.SendAsync("GetTables", _tableProvider.GetAllTableViews());
        }

        public void SendAnswer(string guid, PokerAction value)
        {
            _hubEventEmitter.SetAnswer(guid, value);
        }

        public async Task JoinToTable(int tableId, PokerUser rawPokerUser)
        {
            var pokerUser = _pokerUserProvider.GetUser(rawPokerUser);
            _tableProvider.JoinToTable(tableId, pokerUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, tableId.ToString());
            await Clients.All.SendAsync("GetTables", _tableProvider.GetAllTableViews());

            await Task.Delay(1000);

            var currentTable = _tableProvider.GetCurrentTable(tableId);
            if(currentTable.PokerUsers.Count >= 2)
            {
                
                _eventProxy.GameStarted(currentTable);
                //await Clients.Group(tableId.ToString()).SendAsync("Test", String.Join(',', currentTable.PokerUsers.Select(p => p.Username)));
            }
        }

        public async Task LeaveTable(int tableId, PokerUser rawPokerUser)
        {
            var pokerUser = _pokerUserProvider.GetUser(rawPokerUser);
            _tableProvider.LeaveTable(tableId, pokerUser);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, tableId.ToString());
            await Clients.All.SendAsync("GetTables", _tableProvider.GetAllTableViews());
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }




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
