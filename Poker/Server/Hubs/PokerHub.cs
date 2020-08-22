using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;
using Poker.Server.Providers;
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

        List<string> ids = new List<string>();

        public PokerHub( 
            DatabaseService databaseService,
            TableProvider tableProvider,
            PokerUserProvider pokerUserProvider)
        {
            //_lobby = lobby;
            _databaseService = databaseService;
            _tableProvider = tableProvider;
            _pokerUserProvider = pokerUserProvider;
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

        //public async Task InitConnectionWithAll()
        //{

        //}





        #region ConnectionFunctions

        // Connection functions
        public override async Task OnConnectedAsync()
        {
            var currentUser = GetCurrentUser();

            await InitConnectionWithClient();

            Debug.WriteLine("SignalR - connected " + currentUser?.Username);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var currentUser = GetCurrentUser();

            Debug.WriteLine("SignalR - disconnect " + currentUser?.Username);

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
                return user;
            }
            return null;
        }

        #endregion
    }
}
