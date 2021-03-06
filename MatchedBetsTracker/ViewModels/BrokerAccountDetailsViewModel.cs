using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MatchedBetsTracker.Models;

namespace MatchedBetsTracker.ViewModels
{
    public class BrokerAccountDetailsViewModel
    {
        public BrokerAccount BrokerAccount { get; set; }
        public IEnumerable<Transaction> Transactions { get; set; }    
        public IEnumerable<Bet> Bets { get; set; }
    }
}