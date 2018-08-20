using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MatchedBetsTracker.Models
{
    public class TransactionType
    {
        public byte Id { get; set; }
        public string TransactionDescription { get; set; }

        public static readonly byte OpenBet = 2;
        public static readonly byte CreditBet = 3;
        public static readonly byte CreditBonus = 4;
        public static readonly byte ExpireBonus = 5;
        public static readonly byte MoneyCredit = 6;
        public static readonly byte MoneyDebit = 7;
        public static readonly byte Correction = 8;
    }
}