using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.Shared.Models.PokerModels
{
    public struct ProgressPercent
    {
        public double Percent { get; set; }

        public string Color { 
            get {
                if (Percent > 67)
                {
                    return "bg-success";
                }
                else if (Percent > 33)
                {
                    return "bg-warning";
                }
                return "bg-danger";
            } 
        }

        public ProgressPercent(double percent)
        {
            Percent = Math.Abs(100 - percent);
        }


        public override string ToString()
        {
            return Convert.ToInt32(Percent).ToString() + "%";
        }
    }
}
