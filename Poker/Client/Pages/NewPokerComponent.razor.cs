using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Poker.Client.Services;
using Poker.Shared.Enums;
using Poker.Shared.Managers;
using Poker.Shared.Models.DomainModels;
using Poker.Shared.Models.PokerModels;
using Poker.Shared.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poker.Client.Pages
{
    public partial class NewPokerComponent
    {

        [Inject]
        public AuthService AuthService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public PokerUser PokerUser { get; set; }
        public GameViewModel GameViewModel { get; set; }


        public int MyRaiseValue { get; set; }
        public bool IsRaise { get => CurrentValue != GameViewModel.MinValue; }
        public ProgressPercent ProgressPercent { get; private set; }
        public int CurrentValue { get; set; }

        bool cancel;

        public TableViewModel SelectedTable { get; set; }
        public bool TablesEnabled { get; set; }
        List<TableViewModel> _tables = new List<TableViewModel>();
        public Dictionary<int, string> StyleMap { get; set; }

        public NewPokerComponent()
        {
            CurrentValue = 0;
            TablesEnabled = true;

            StyleMap = CreateStyleMap();
        }

        protected override async Task OnInitializedAsync()
        {
            if (AuthService == null)
            {
                throw new Exception("no authService");
            }

            var hubConnection = AuthService.HubConnection;
            PokerUser = AuthService.PokerUser;

            hubConnection.On<List<TableViewModel>>("GetTables", (tables) =>
            {
                Console.WriteLine("GetTables");
                _tables = tables;
                StateHasChanged();
            });

            hubConnection.On<GameViewModel>("SendPokerAction", async gameViewModel =>
            {
                Console.WriteLine("------------");
                Console.WriteLine("GAMEVIEWMODEL");
                Console.WriteLine(gameViewModel);

                cancel = true;

                GameViewModel = gameViewModel;

                if (GameViewModel != null && GameViewModel.Player != null && GameViewModel.Player.IsInAllIn)
                {
                    await Task.Delay(250);
                    await SendAction(Poker.Shared.Enums.UserActionType.Check);
                }

                if (GameViewModel != null)
                {
                    CurrentValue = GameViewModel.MinValue;
                    PokerUser.Balance = GameViewModel.Balance;

                    if (GameViewModel.LeaveTable)
                    {
                        Console.WriteLine("Leave table");
                        await LeaveTable();
                        StateHasChanged();
                        return;
                    }
                }

                StateHasChanged();

                if (GameViewModel != null && GameViewModel.IsMyTurn)
                {
                    cancel = false;
                    StartCount();
                }
                Console.WriteLine("------------");
            });

            await hubConnection.SendAsync("GetUsers");

            await hubConnection.SendAsync("GetTables");


        }

        async Task AddTable()
        {
            await AuthService.HubConnection.SendAsync("AddTable");
        }

        async Task JoinToTable(TableViewModel tableViewModel)
        {
            if(PokerUser.Balance > 0)
            {
                SelectedTable = tableViewModel;
                await AuthService.HubConnection.SendAsync("JoinToTable", tableViewModel.Id, AuthService.PokerUser);
            }
        }

        async Task LeaveTable()
        {
            if (GameViewModel != null && GameViewModel.IsMyTurn)
            {
                await SendAction(Poker.Shared.Enums.UserActionType.Fold);
            }
            await AuthService.HubConnection.SendAsync("LeaveTable", SelectedTable.Id, AuthService.PokerUser);
            GameViewModel = null;
            SelectedTable = null;
        }

        async Task Logout()
        {
            SelectedTable = null;
            await AuthService.Logout();
        }

        async Task SendAction(UserActionType userActionType)
        {
            UserAction userAction = null;

            switch (userActionType)
            {
                case UserActionType.Raise:
                    if (IsRaise)
                    {
                        MyRaiseValue = CurrentValue;
                        if (CurrentValue == GameViewModel.MinValue)
                        {
                            if (GameViewModel.IsRaiseInProgess)
                            {
                                userAction = new UserAction(PokerUser, UserActionType.Call);
                            }
                            else
                            {
                                userAction = new UserAction(PokerUser, UserActionType.Check);
                            }
                        }
                        else if (CurrentValue == GameViewModel.Balance)
                        {
                            userAction = new UserAction(PokerUser, UserActionType.AllIn);
                        }
                        else
                        {
                            userAction = new UserAction(PokerUser, userActionType, CurrentValue);
                        }
                    }
                    break;
                case UserActionType.Call:
                    if (!GameViewModel.IsRaiseInProgess)
                    {
                        return;
                    }
                    userAction = new UserAction(PokerUser, userActionType);
                    break;
                case UserActionType.Check:
                    if (GameViewModel.IsRaiseInProgess)
                    {
                        return;
                    }
                    userAction = new UserAction(PokerUser, userActionType);
                    break;
                default:
                    userAction = new UserAction(PokerUser, userActionType);
                    break;
            }

            if(GameViewModel != null)
            {
                userAction.GameId = GameViewModel.GameId;
                userAction.RoundId = GameViewModel.RoundId;
            }

            await AuthService.HubConnection.SendAsync("SendUserAction", userAction);
        }

        async void StartCount()
        {
            try
            {
                ProgressPercent = new ProgressPercent(0);
                StateHasChanged();

                var startDate = DateTime.Now;
                var gap = TimeSpan.FromSeconds(14.5).TotalSeconds;

                var currentValue = (DateTime.Now - startDate).TotalSeconds;

                while (currentValue <= gap)
                {
                    if (cancel)
                    {
                        break;
                    }
                    currentValue = (DateTime.Now - startDate).TotalSeconds;

                    ProgressPercent = new ProgressPercent(currentValue / gap * 100);

                    await Task.Delay(200);
                    StateHasChanged();
                }

                if (!cancel)
                {
                    await SendAction(Poker.Shared.Enums.UserActionType.Fold);
                }
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

        }

        private Dictionary<int, string> CreateStyleMap()
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();

            dict.Add(0, "poker-bottom poker-bottom-rows");
            dict.Add(1, "poker-bottom-right poker-bottom-rows");
            dict.Add(2, "poker-top-right poker-top-rows");
            dict.Add(3, "poker-top poker-top-rows");
            dict.Add(4, "poker-top-left poker-top-rows");
            dict.Add(5, "poker-bottom-left poker-bottom-rows");

            return dict;
        }

        void ChangeView()
        {
            TablesEnabled = !TablesEnabled;
            StateHasChanged();
        }

    }
}
