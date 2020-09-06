using Poker.Shared.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Shared.Models.PokerModels
{
    public class Winner
    {
        public Winner()
        {

        }

        public Winner(PokerUser pokerUser, Result result)
        {
            PokerUser = pokerUser;
            Result = result;
        }

        public PokerUser PokerUser { get; set; }
        public Result Result { get; set; }
    }
}
