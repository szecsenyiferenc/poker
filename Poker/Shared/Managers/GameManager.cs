using Poker.Shared.Builders;
using Poker.Shared.Enums;
using Poker.Shared.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poker.Shared.Managers
{
    public class GameManager
    {
        private const int COLOR_NUMBER = 4;
        private const int CARD_TYPE_NUMBER = 15;

        public Result GetResult(List<Card> cards)
        {
            var sortedCards = cards.OrderByDescending(c => c.CardType);

            Result straightFlushResult = StraightFlushResult(cards);

            if (straightFlushResult != null)
            {
                return straightFlushResult;
            }

            Result pokerResult = PokerResult(cards);

            if (pokerResult != null)
            {
                return pokerResult;
            }

            Result fullResult = FullResult(cards);

            if (fullResult != null)
            {
                return fullResult;
            }

            Result flushResult = FlushResult(cards);

            if (flushResult != null)
            {
                return flushResult;
            }

            Result straightResult = StraightResult(cards);

            if (straightResult != null)
            {
                return straightResult;
            }

            Result drillResult = DrillResult(cards);

            if (drillResult != null)
            {
                return drillResult;
            }

            Result pairResult = PairResult(cards);

            if (pairResult != null)
            {
                return pairResult;
            }

            Result highResult = HighCardResult(cards);

            return highResult;
        }

        private Result StraightFlushResult(List<Card> cards)
        {
            List<Card>[] filteredCards = new List<Card>[COLOR_NUMBER];

            for (int i = 0; i < COLOR_NUMBER; i++)
            {
                filteredCards[i] = new List<Card>();
            }

            foreach (var card in cards)
            {
                filteredCards[(int)card.CardColor].Add(card);
            }

            for (int i = 0; i < COLOR_NUMBER; i++)
            {
                int? maxRowValue = MaxRowValue(filteredCards[i]);
                if (maxRowValue.HasValue)
                {
                    if (maxRowValue.Value == (int)CardType.Ace)
                    {
                        return new ResultBuilder().Hand(Hand.RoyalFlush).AddValues(CardType.Ace).Build();
                    }
                    else
                    {
                        return new ResultBuilder().Hand(Hand.StraightFlush).AddValues((CardType)maxRowValue.Value).Build();
                    }
                }
            }


            return null;
        }

        private Result PokerResult(List<Card> cards)
        {
            int[] cardTypes = new int[CARD_TYPE_NUMBER];

            Result result = null;

            foreach (var item in cards)
            {
                cardTypes[(int)item.CardType]++;
            }

            for (int i = 0; i < CARD_TYPE_NUMBER; i++)
            {
                if (cardTypes[i] == 4)
                {
                    return new ResultBuilder().Hand(Hand.Poker).AddValues((CardType)i).Build();
                }
            }

            return result;
        }

        private Result FullResult(List<Card> cards)
        {
            int[] cardTypes = new int[CARD_TYPE_NUMBER];

            int drill = -1;
            int pair = -1;

            foreach (var item in cards)
            {
                cardTypes[(int)item.CardType]++;
            }

            for (int i = 0; i < CARD_TYPE_NUMBER; i++)
            {
                if (cardTypes[i] == 3)
                {
                    if (i > drill)
                    {
                        drill = i;
                    }
                }

                if (cardTypes[i] == 2)
                {
                    if (i > pair)
                    {
                        pair = i;
                    }
                }
            }

            if (drill == -1 || pair == -1)
            {
                return null;
            }

            return new ResultBuilder().Hand(Hand.FullHouse).AddValues((CardType)drill).AddValues((CardType)pair).Build();
        }

        private Result FlushResult(List<Card> cards)
        {
            int[] cardColors = new int[COLOR_NUMBER];
            int maxValue = 0;

            foreach (var card in cards)
            {
                if (maxValue < (int)card.CardType)
                {
                    maxValue = (int)card.CardType;
                }

                cardColors[(int)card.CardColor]++;
            }

            for (int i = 0; i < COLOR_NUMBER; i++)
            {
                if (cardColors[i] >= 5)
                {
                    return new ResultBuilder().Hand(Hand.Flush).AddValues((CardType)maxValue).Build();
                }
            }

            return null;
        }

        private Result StraightResult(List<Card> cards)
        {
            var sortedCards = cards.ToList();

            int? maxRowValue = MaxRowValue(sortedCards);
            if (maxRowValue.HasValue)
            {
                return new ResultBuilder().Hand(Hand.Straight).AddValues((CardType)maxRowValue.Value).Build();
            }

            return null;
        }

        private Result DrillResult(List<Card> cards)
        {
            int[] cardTypes = new int[CARD_TYPE_NUMBER];

            int drill = -1;

            foreach (var item in cards)
            {
                cardTypes[(int)item.CardType]++;
            }

            for (int i = 0; i < CARD_TYPE_NUMBER; i++)
            {
                if (cardTypes[i] == 3)
                {
                    if (i > drill)
                    {
                        drill = i;
                        break;
                    }
                }
            }

            if (drill == -1)
            {
                return null;
            }

            var remainCards = cards.ToList().Where(c => c.CardType != (CardType)drill).OrderByDescending(c => c.CardType).Take(2).Select(c => c.CardType).ToList();

            return new ResultBuilder().Hand(Hand.Drill).AddValues((CardType)drill).AddValues(remainCards).Build();
        }

        private Result PairResult(List<Card> cards)
        {
            int[] cardTypes = new int[CARD_TYPE_NUMBER];

            int pair1 = -1;
            int pair2 = -1;

            foreach (var item in cards)
            {
                cardTypes[(int)item.CardType]++;
            }

            for (int i = 0; i < CARD_TYPE_NUMBER; i++)
            {
                if (cardTypes[i] == 2)
                {
                    if (pair1 == -1)
                    {
                        pair1 = i;
                        continue;
                    }

                    if (pair2 == -1)
                    {
                        pair2 = i;
                        continue;
                    }

                    if (pair1 >= pair2)
                    {
                        pair2 = i;
                    }
                    else
                    {
                        pair1 = i;
                    }
                }
            }

            if (pair1 == -1)
            {
                return null;
            }

            if (pair1 != -1 && pair2 != -1)
            {
                if (pair1 < pair2)
                {
                    var temp = pair1;
                    pair1 = pair2;
                    pair2 = temp;
                }

                var remainCard = cards.ToList().Where(c => c.CardType != (CardType)pair1 && c.CardType != (CardType)pair2).OrderByDescending(c => c.CardType).Take(1).Select(c => c.CardType).First();
                return new ResultBuilder().Hand(Hand.TwoPair).AddValues((CardType)pair1).AddValues((CardType)pair2).AddValues(remainCard).Build();
            }

            var remainCards = cards.ToList().Where(c => c.CardType != (CardType)pair1).OrderByDescending(c => c.CardType).Take(3).Select(c => c.CardType).ToList();
            return new ResultBuilder().Hand(Hand.Pair).AddValues((CardType)pair1).AddValues(remainCards).Build();
        }

        private Result HighCardResult(List<Card> cards)
        {
            var sortedCards = cards.ToList().OrderByDescending(c => c.CardType).Take(5).Select(c => c.CardType).ToList();
            return new ResultBuilder().Hand(Hand.HighCard).AddValues(sortedCards).Build();
        }

        private int? MaxRowValue(List<Card> cards)
        {
            if (cards.Count < 5)
            {
                return null;
            }

            cards.Sort();

            int count = 1;
            int maxValue = 0;

            if (cards.Exists(c => c.CardType == CardType.Ace) && (int)cards[0].CardType == 2)
            {
                count = 2;
            }

            for (int i = 0; i < cards.Count; i++)
            {
                if (i != cards.Count - 1 && cards[i].CardType + 1 == cards[i + 1].CardType)
                {
                    count++;
                    maxValue = (int)cards[i + 1].CardType;
                }
                else if (count < 5)
                {
                    count = 0;
                }
            }

            if (count >= 5)
            {
                return maxValue;
            }

            return null;
        }

    }
}
