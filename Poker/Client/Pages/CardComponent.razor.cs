using Microsoft.AspNetCore.Components;
using Poker.Shared.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poker.Client.Pages
{
    public partial class CardComponent
    {
        [Parameter]
        public Card Card { get; set; }

        public string ImageUrl { get => $"assets/cards/{Card.ToString()}.png"; }
    }
}
