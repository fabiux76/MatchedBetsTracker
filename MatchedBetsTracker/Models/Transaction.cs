using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MatchedBetsTracker.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }
        
        public TransactionType TransactionType { get; set; }

        [Display(Name = "Transaction Type")]        
        public byte TransactionTypeId { get; set; }

        [TransactionSign]
        public double Amount { get; set; }

        public bool Validated { get; set; }

        public BrokerAccount BrokerAccount { get; set; }

        [Display(Name = "Broker")]
        public int BrokerAccountId { get; set; }  
              
        public Bet Bet { get; set; }

        [Display(Name = "Bet")]
        public int? BetId { get; set; }

        public UserAccount UserAccount { get; set; }

        [OwnerTransactions]
        public int UserAccountId { get; set; }
    }
}