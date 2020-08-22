using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Shared.Models.ViewModels
{
    public class TableViewModel
    {
        public int Id { get; set; }
        public int PlayerNumber { get; set; }
        public int MaxNumber { get; set; }
        public string Name { get; set; }
    }
}
