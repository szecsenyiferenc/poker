using Microsoft.AspNetCore.Components;
using Poker.Shared.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poker.Client.Pages
{
    public partial class CardRowComponent
    {
        [Parameter]
        public List<Card> Cards { get; set; }
    }
}
