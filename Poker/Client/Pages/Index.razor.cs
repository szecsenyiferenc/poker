using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Poker.Shared.Managers;
using Poker.Shared.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poker.Client.Pages
{
    public partial class Index
    {
        private HubConnection hubConnection;
        private NavigationManager _navigationManager;
        private string PlayerName { get; set; }
        private List<string> messages = new List<string>();

        List<Card> flop = new List<Card>();
        List<Card> ownCards = new List<Card>();
        List<Card>[] hands = new List<Card>[6];

        GameManager gameManager;

        public Index()
        {
            gameManager = new GameManager();
            //_navigationManager = NavigationManager;
            //unknownCards = new List<UnknownCard>
            //{
            //    new UnknownCard(),
            //    new UnknownCard()
            //};

            for (int i = 0; i < hands.Length; i++)
            {
                hands[i] = new List<Card>();
            }

        }

        protected override async Task OnInitializedAsync()
        {
            //hubConnection = new HubConnectionBuilder()
            //    .WithUrl(_navigationManager.ToAbsoluteUri("/pokerhub"))
            //    .Build();

            //hubConnection.On<List<Card>, List<Card>, List<Card>>("ReceiveMessage", (top, middle, bottom) =>
            //{
            //    flop = middle;
            //    ownCards = bottom;
            //    StateHasChanged();
            //});

            //await hubConnection.StartAsync();
        }

        Task Send()
        {
            return hubConnection.SendAsync("SendMessage", "asd", "asd");
        }

        Task Join()
        {
            return hubConnection.SendAsync("Join", PlayerName);
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

        public async Task Start()
        {
            await Join();
            await Send();
        }

        //protected override async Task OnAfterRenderAsync(bool firstRender)
        //{
        //    if (firstRender)
        //    {
        //        await Join();
        //        await Send();
        //    }
        //}

        public List<Card> GetTopCards()
        {
            return ownCards;
        }

        public bool IsConnected => hubConnection.State == HubConnectionState.Connected;

        public void Dispose()
        {
            hubConnection.DisposeAsync();
        }
    }
}
