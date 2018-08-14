using MatchedBetsTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MatchedBetsTracker.ViewModels
{
    public class NewTransactionViewModel
    {
        public IEnumerable<TransactionType> TransactionTypes { get; set; }
        public IEnumerable<BrokerAccount> BrokerAccounts { get; set; }
        public Transaction Transaction { get; set; }
    }
}