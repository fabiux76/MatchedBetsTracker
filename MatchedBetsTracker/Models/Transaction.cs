using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MatchedBetsTracker.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public TransactionType TransactionType { get; set; }
        public byte TransactionTypeId { get; set; }
        public double Amount { get; set; }
        public bool Validated { get; set; }
        public BrokerAccount BrokerAccount { get; set; }
        public int BrokerAccountId { get; set; }
        public Bet Bet { get; set; }
        public int BetId { get; set; }
    }
}