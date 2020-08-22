using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Poker.Client.Services;
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
        private string PlayerName { get; set; }
        private List<string> messages = new List<string>();
        public PokerUser PokerUser { get; set; }

        List<Card> flop = new List<Card>();
        List<Card> ownCards = new List<Card>();
        List<Card>[] hands = new List<Card>[6];
        List<PokerUser> players;

        List<TableViewModel> _tables = new List<TableViewModel>();

        GameManager gameManager;

        public PokerComponent()
        {
            gameManager = new GameManager();
            players = new List<PokerUser>();
           
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
            var hubConnection = AuthService.HubConnection;

            hubConnection.On<List<TableViewModel>>("GetTables", (tables) =>
            {
                _tables = tables;
                StateHasChanged();
            });

            await hubConnection.SendAsync("GetUsers");

            PokerUser = AuthService.PokerUser;

        }

        async Task AddTable()
        {

        }


        async Task Logout()
        {
            await AuthService.Logout();
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
