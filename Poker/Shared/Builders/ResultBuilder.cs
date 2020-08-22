using Poker.Shared.Enums;
using Poker.Shared.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Shared.Builders
{
    public class ResultBuilder
    {
        private Result _result;

        public ResultBuilder()
        {
            _result = new Result();
        }


        public ResultBuilder Hand(Hand hand)
        {
            _result.Hand = hand;
            return this;
        }

        public ResultBuilder AddValues(CardType cardType)
        {
            _result.Values.Add(cardType);
            return this;
        }

        public ResultBuilder AddValues(List<CardType> cardTypes)
        {
            _result.Values.AddRange(cardTypes);
            return this;
        }

        public Result Build()
        {
            return _result;
        }

    }
}
