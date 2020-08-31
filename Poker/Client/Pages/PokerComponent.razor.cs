using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
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
    public partial class PokerComponent
    {
        [Inject]
        public AuthService AuthService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }




        public PokerUser PokerUser { get; set; }
        public TableViewModel SelectedTable { get; set; }
        public string CurrentSessionGuid { get; private set; }
        public ProgressPercent ProgressPercent { get; private set; }
        public int MinValue { get; private set; }
        public int MaxValue { get; private set; }
        public int Balance { get; private set; }
        public Dictionary<int, string> StyleMap { get; set; }

        public List<PokerUser> Players { get; set; }

        List<Card> flop = new List<Card>();
        List<Card> ownCards = new List<Card>();
        List<Card> unknown = new List<Card>();
        List<Card>[] hands = new List<Card>[6];


        List<TableViewModel> _tables = new List<TableViewModel>();

        public PokerComponent()
        {
            MinValue = 100;
            MaxValue = 2020;
            Balance = 200;

            Players = new List<PokerUser>();

            StyleMap = CreateStyleMap();

            unknown = new List<Card>(){
                new UnknownCard(),
                new UnknownCard()
            };

            for (int i = 0; i < hands.Length; i++)
            {
                hands[i] = new List<Card>();
            }

        }

        protected override async Task OnInitializedAsync()
        {
            if(AuthService == null)
            {
                throw new Exception("no authService");
            }

            var hubConnection = AuthService.HubConnection;

            hubConnection.On<List<TableViewModel>>("GetTables", (tables) =>
            {
                _tables = tables;
                StateHasChanged();
            });

            hubConnection.On<string>("Test", (test) =>
            {
                Console.WriteLine(test);
                StateHasChanged();
            });

            hubConnection.On("Fold", () =>
            {
                CurrentSessionGuid = null;
                StateHasChanged();
            });

            hubConnection.On<string, string>("SendMessageToUser", async (guid, text) =>
            {
                CurrentSessionGuid = guid;
                StateHasChanged();
                await StartCount();
            });

            hubConnection.On<List<PokerUser>>("PlayerStatus", pokerPlayers =>
            {
                Players = pokerPlayers;
                StateHasChanged();
            });

            hubConnection.On<List<Card>>("SendCards", cards =>
            {
                ownCards = cards;
                StateHasChanged();
            });


            hubConnection.On<RoundStatus>("SendStatus", roundStatus =>
            {
                var allCards = roundStatus.Flop;

                flop = allCards;
                StateHasChanged();
            });

            await hubConnection.SendAsync("GetUsers");

            await hubConnection.SendAsync("GetTables");

            PokerUser = AuthService.PokerUser;

        }

        async Task AddTable()
        {
            await AuthService.HubConnection.SendAsync("AddTable");
        }


        async Task JoinToTable(TableViewModel tableViewModel) {
            SelectedTable = tableViewModel;
            await AuthService.HubConnection.SendAsync("JoinToTable", tableViewModel.Id, AuthService.PokerUser);
        }

        async Task LeaveTable()
        {
            await AuthService.HubConnection.SendAsync("LeaveTable", SelectedTable.Id, AuthService.PokerUser);
            SelectedTable = null;
        }

        async Task Logout()
        {
            SelectedTable = null;
            await AuthService.Logout();
        }

        async Task SendAction(PokerActionType pokerActionType)
        {
            await AuthService.HubConnection.SendAsync("SendAnswer", CurrentSessionGuid, new PokerAction(pokerActionType));
            CurrentSessionGuid = null;
        }

        async Task StartCount()
        {
            ProgressPercent = new ProgressPercent(0);
            StateHasChanged();

            var startDate = DateTime.Now;
            var gap = TimeSpan.FromSeconds(14.5).TotalSeconds;

            var currentValue = (DateTime.Now - startDate).TotalSeconds;

            while (currentValue <= gap)
            {
                currentValue = (DateTime.Now - startDate).TotalSeconds;

                ProgressPercent = new ProgressPercent(currentValue / gap * 100);

                await Task.Delay(25);
                StateHasChanged();
            }
        }


        private Dictionary<int, string> CreateStyleMap()
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();

            dict.Add(0, "poker-bottom");
            dict.Add(1, "poker-bottom-right");
            dict.Add(2, "poker-top-right");
            dict.Add(3, "poker-top");
            dict.Add(4, "poker-top-left");
            dict.Add(5, "poker-bottom-left");

            return dict;
        }



        public string Result()
        {
            //if (topCard != null && topCard.Any())
            //{
            //    var firstCards = topCard.ToList();
            //    firstCards.AddRange(middleCard);

            //    var secondCards = bottomCard.ToList();
            //    secondCards.AddRange(middleCard);

            //    var res1 = gameManager.GetResult(firstCards);
            //    var res2 = gameManager.GetResult(secondCards);

            //    switch (res1.CompareTo(res2)){
            //        case 1:
            //            return $"Winner top player, with: {res1.Hand}";
            //        case -1:
            //            return $"Winner bottom player, with: {res2.Hand}";
            //        case 0:
            //        default:
            //            return $"Tie, with: {res1.Hand}";
            //    }
            //};

            return "";
        }

        //public async Task Start()
        //{
        //    await Join();
        //    await Send();
        //}


        public List<Card> GetTopCards()
        {
            return ownCards;
        }


    }
}
