using Poker.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Shared.Models.DomainModels
{
    public class Result : IComparable
    {
        public Result()
        {
            Values = new List<CardType>();
        }

        public Result(Hand hand, List<CardType> values)
        {
            Hand = hand;
            Values = values;
        }

        public Hand Hand { get; set; }
        public List<CardType> Values { get; set; }

        public override string ToString()
        {
            return $"Hand: {Hand}, {FormattedValues()}";
        }

        private string FormattedValues()
        {
            var sb = new StringBuilder();
            foreach (var item in Values)
            {
                sb.Append($"Value: {item}, ");
            }
            return sb.ToString();
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            Result otherResult = obj as Result;
            if (otherResult != null)
            {
                if (Hand > otherResult.Hand)
                {
                    return 1;
                }
                else if (Hand < otherResult.Hand)
                {
                    return -1;
                }
                for (int i = 0; i < Values.Count; i++)
                {
                    if (Values[i] > otherResult.Values[i])
                    {
                        return 1;
                    }
                    if (Values[i] < otherResult.Values[i])
                    {
                        return -1;
                    }
                }
                return 0;
            }

            else
            {
                throw new ArgumentException("Object is not a Result");
            }
        }

    }
}
