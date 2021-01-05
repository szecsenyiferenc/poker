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
        public bool TablesEnabled { get; set; }
        public bool IsRaiseInProgess { get; set; }
        public int MaxRaiseValue { get; set; }
        public int MyRaiseValue { get; set; }
        public bool IsRaise { get => CurrentValue != MinValue; }
        public PokerUser NextPlayer { get; set; }
        public Winner Winner { get; set; }
        public TableViewModel SelectedTable { get; set; }
        public string CurrentSessionGuid { get; private set; }
        public ProgressPercent ProgressPercent { get; private set; }
        public int CurrentValue { get; set; }
        public int MinValue { get; private set; }
        public int MaxValue { get; private set; }
        public int Balance { get => PokerUser.Balance;  }
        public int Pot { get; private set; }

        public Dictionary<int, string> StyleMap { get; set; }

        public List<Player> Players { get; set; }

        List<Card> flop = new List<Card>();
        List<Card> ownCards = new List<Card>();
        List<Card> unknown = new List<Card>();
        List<Card>[] hands = new List<Card>[6];


        List<TableViewModel> _tables = new List<TableViewModel>();

        public PokerComponent()
        {
            MinValue = 0;
            CurrentValue = 0;
            TablesEnabled = true;
            IsRaiseInProgess = false;

            Players = new List<Player>();

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

            hubConnection.On<List<Player>>("PlayerStatus", pokerPlayers =>
            {
                Console.WriteLine("PlayerStatus");
                Console.WriteLine(pokerPlayers != null);
                Console.WriteLine(pokerPlayers.FirstOrDefault());
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

                if(pokerAction.PokerActionType == PokerActionType.RaiseHappened)
                {
                    Console.WriteLine("CurrentRaise");
                    Console.WriteLine("Raised player - " + pokerAction.PlayerWithRaise.Username);
                    var playerWithRaise = Players.FirstOrDefault(p => p.Username == pokerAction.PlayerWithRaise.Username);
                    playerWithRaise.CurrentRaise = pokerAction.PlayerWithRaise.CurrentRaise;
                    playerWithRaise.Balance = pokerAction.PlayerWithRaise.Balance;
                    Pot = pokerAction.Pot;
                    IsRaiseInProgess = true;
                    MaxRaiseValue = playerWithRaise.CurrentRaise;
                    MinValue = playerWithRaise.CurrentRaise - MyRaiseValue;
                }

                if(pokerAction.Winner != null)
                {
                    Winner = pokerAction.Winner;
                    var winnerPlayer = Players.FirstOrDefault(p => p.Username == Winner.PokerUser.Username);
                    winnerPlayer.Balance = pokerAction.Winner.PokerUser.Balance;
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
                Pot = 0;
                ResetRaiseState();
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
                ResetRaiseState();
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
                ResetRaiseState();
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
                ResetRaiseState();
            }
            if (pokerAction.PokerActionType == PokerActionType.NextPlayer)
            {
                Console.WriteLine("RiverRound - 2");
                await UpdateUI(pokerAction);
            }
        }

        private async Task UpdateUI(PokerAction pokerAction)
        {
            NextPlayer = pokerAction.NextPlayer;
            if (pokerAction?.NextPlayer?.Username == PokerUser.Username)
            {
                CurrentValue = MinValue;
                PokerUser.Balance = NextPlayer.Balance;
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

        private void ResetRaiseState()
        {
            Players.ForEach(p => p.CurrentRaise = 0);
            IsRaiseInProgess = false;
            MaxRaiseValue = 0;
            MyRaiseValue = 0;
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
            UserAction userAction = null;

            switch (userActionType)
            {
                case UserActionType.Raise:
                    if (IsRaise)
                    {
                        MyRaiseValue = CurrentValue;
                        if (CurrentValue == MinValue)
                        {
                            if (IsRaiseInProgess)
                            {
                                userAction = new UserAction(PokerUser, UserActionType.Call);
                            }
                            else
                            {
                                userAction = new UserAction(PokerUser, UserActionType.Check);
                            }
                        }
                        else if(CurrentValue == Balance)
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
                    if (!IsRaiseInProgess)
                    {
                        return;
                    }
                    userAction = new UserAction(PokerUser, userActionType);
                    break;
                case UserActionType.Check:
                    if (IsRaiseInProgess)
                    {
                        return;
                    }
                    userAction = new UserAction(PokerUser, userActionType);
                    break;
                default:
                    userAction = new UserAction(PokerUser, userActionType);
                    break;
            }

            await AuthService.HubConnection.SendAsync("SendUserAction", userAction);
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
