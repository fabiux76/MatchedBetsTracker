using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MatchedBetsTracker.BusinessLogic
{
    public static class Constants
    {
        public enum TransactionType {
            OpenBet = 2,
            CreditBet = 3,
            CreditBonus = 4,
            ExpireBonus = 5,
            MoneyCredit = 6,
            MoneyDebit = 7
        }

        public enum BetStatus
        {
            Open = 1,
            Won = 2,
            Loss = 3
        }
    }
}