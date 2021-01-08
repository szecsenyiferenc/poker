using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Poker.Shared.Models.DomainModels;
using Poker.Shared.Models.PokerModels;
using Poker.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Poker.Client.Services
{
    public class AuthService
    {
        private readonly RestBackendService _restBackendService;
        private readonly NavigationManager _navigationManager;
        private HubConnection _hubConnection;
        private LoggedInUser _loggedInUser;
        public bool LoggedIn { get => _loggedInUser?.PokerUser != null; }
        public PokerUser PokerUser { get => _loggedInUser?.PokerUser; }
        public HubConnection HubConnection { get => IsConnected && _hubConnection != null ? _hubConnection : null; }

        public AuthService(RestBackendService restBackendService, NavigationManager navigationManager)
        {
            _restBackendService = restBackendService;
            _navigationManager = navigationManager;
        }

        public async Task Login(LoginData loginData)
        {
            var loggedInUser = await _restBackendService.CheckLogin(loginData);
            if(loggedInUser?.Id != null)
            {
                _loggedInUser = loggedInUser;
                await StartHubConnection();
                _navigationManager.NavigateTo("/");
            }
        }

        public async Task Logout()
        {
            await StopHubConnection();
            await _restBackendService.Logout(_loggedInUser);
            _loggedInUser = null;
            _navigationManager.NavigateTo("/login");
        }

        public async Task Refresh()
        {

        }

        private async Task StartHubConnection()
        {
            _hubConnection = new HubConnectionBuilder()
            .WithUrl(_navigationManager.ToAbsoluteUri($"/pokerhub?auth={_loggedInUser.Id}"))
            .Build();

            await _hubConnection.StartAsync();
        }

        private async Task StopHubConnection()
        {
            Console.WriteLine("Disconnection...");
            await _hubConnection.DisposeAsync();
        }

        public bool IsConnected => _hubConnection.State == HubConnectionState.Connected;

        public void Dispose()
        {
            Console.WriteLine("Disposing....Disconnection...");
            _hubConnection.DisposeAsync();
        }

    }
}
