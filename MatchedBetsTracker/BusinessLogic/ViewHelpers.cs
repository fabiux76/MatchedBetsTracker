using MatchedBetsTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MatchedBetsTracker.BusinessLogic
{
    public static class ViewHelpers
    {
        public static string SearchQuery(this SportEvent sportEvent)
        {
            return sportEvent.EventDescription.Substring(0, sportEvent.EventDescription.LastIndexOfOrLenght(':'))
                .Replace(" v ", " ")
                .Replace(" ", "+");
        }

        public static int LastIndexOfOrLenght(this string s, char c)
        {
            return s.LastIndexOf(c) == -1 ? s.Length : s.LastIndexOf(c);
        }

        public static MatchedBet MatchedBet(this SportEvent sportEvent)
        {
            return sportEvent.BetEvents.First().Bet.MatchedBet;
        }
    }
}