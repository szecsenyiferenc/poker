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
        public Winner Winner { get; set; }
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
            PokerUser = AuthService.PokerUser;

            hubConnection.On<List<TableViewModel>>("GetTables", (tables) =>
            {
                Console.WriteLine("GetTables");
                _tables = tables;
                StateHasChanged();
            });

            hubConnection.On<string>("Test", (test) =>
            {
                Console.WriteLine($"The Winner is... {test}");
                StateHasChanged();
            });

            hubConnection.On("Fold", () =>
            {
                Console.WriteLine("Fold");
                CurrentSessionGuid = null;
                StateHasChanged();
            });

            hubConnection.On<string, string>("SendMessageToUser", async (guid, text) =>
            {
                Console.WriteLine("SendMessageToUser");
                CurrentSessionGuid = guid;
                StateHasChanged();
                await StartCount();
            });

            hubConnection.On<List<PokerUser>>("PlayerStatus", pokerPlayers =>
            {
                Console.WriteLine("PlayerStatus");
                Players = pokerPlayers;
                StateHasChanged();
            });

            hubConnection.On<List<Card>>("SendCards", cards =>
            {
                Console.WriteLine("SendCards");
                ownCards = cards;
                StateHasChanged();
            });


            hubConnection.On<RoundStatus>("SendStatus", roundStatus =>
            {
                Console.WriteLine("SendStatus");

                var allCards = roundStatus.Flop;

                flop = allCards;
                StateHasChanged();
            });


            hubConnection.On<PokerAction>("SendPokerAction", async pokerAction =>
            {
                Console.WriteLine("------------");

                switch (pokerAction.RoundType)
                {
                    case RoundType.Start:
                        await StartRound(pokerAction);
                        break;
                    case RoundType.Flop:
                        await FlopRound(pokerAction);
                        break;
                    case RoundType.Turn:
                        await TurnRound(pokerAction);
                        break;
                    case RoundType.River:
                        await RiverRound(pokerAction);
                        break;
                }

                if(pokerAction.Winner != null)
                {
                    Winner = pokerAction.Winner;
                }

                StateHasChanged();
            });

            await hubConnection.SendAsync("GetUsers");

            await hubConnection.SendAsync("GetTables");


        }

        async Task StartRound(PokerAction pokerAction)
        {
            Console.WriteLine("StartRound");

            if (pokerAction.PokerActionType == PokerActionType.StartingCards)
            {
                Console.WriteLine("StartRound - 1");
                Console.WriteLine(pokerAction.Targets.First().Cards[0]);

                ownCards = pokerAction.Targets.First().Cards;
                flop = null;
                Winner = null;
            }
            if (pokerAction.PokerActionType == PokerActionType.NextPlayer)
            {
                Console.WriteLine("StartRound - 2");

                await UpdateUI(pokerAction);
            }
        }
        async Task FlopRound(PokerAction pokerAction)
        {
            if (pokerAction.PokerActionType == PokerActionType.RoundUpdate)
            {
                Console.WriteLine("FlopRound - 1");
                flop = pokerAction?.Cards;
            }
            if (pokerAction.PokerActionType == PokerActionType.NextPlayer)
            {
                Console.WriteLine("FlopRound - 2");
                await UpdateUI(pokerAction);
            }
        }
        async Task TurnRound(PokerAction pokerAction)
        {
            if (pokerAction.PokerActionType == PokerActionType.RoundUpdate)
            {
                Console.WriteLine("TurnRound - 1");
                flop.Add(pokerAction?.Cards.First());
            }
            if (pokerAction.PokerActionType == PokerActionType.NextPlayer)
            {
                Console.WriteLine("TurnRound - 2");
                await UpdateUI(pokerAction);
            }
        }
        async Task RiverRound(PokerAction pokerAction)
        {
            if (pokerAction.PokerActionType == PokerActionType.RoundUpdate)
            {
                Console.WriteLine("RiverRound - 1");
                flop.Add(pokerAction?.Cards.First());
            }
            if (pokerAction.PokerActionType == PokerActionType.NextPlayer)
            {
                Console.WriteLine("RiverRound - 2");
                await UpdateUI(pokerAction);
            }
        }

        private async Task UpdateUI(PokerAction pokerAction)
        {
            if (pokerAction?.NextPlayer?.Username == PokerUser.Username)
            {
                Console.WriteLine("ENABLE");
                CurrentSessionGuid = "1";
            }
            else
            {
                Console.WriteLine("DISABLE");
                CurrentSessionGuid = null;
            }
            //await StartCount();
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

        async Task SendAction(UserActionType userActionType)
        {
            await AuthService.HubConnection.SendAsync("SendUserAction", new UserAction(PokerUser, userActionType));
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

                await Task.Delay(200);
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
