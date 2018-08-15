using MatchedBetsTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MatchedBetsTracker.ViewModels
{
    public class SimpleMatchedBetFormViewModel
    {
        public int Id { get; set; }
        public string EventDescription { get; set; }
        public string BetDescription { get; set; }
        public DateTime BetDate { get; set; }
        public DateTime EventDate { get; set; }
        //public bool Validated { get; set; }       
        public double BackQuote { get; set; }
        public double BackAmount { get; set; }
        public BrokerAccount BackBrokerAccount { get; set; }
        public int BackBrokerAccountId { get; set; }
        public double LayQuote { get; set; }
        public double LayAmount { get; set; }
        public BrokerAccount LayBrokerAccount { get; set; }
        public int LayBrokerAccountId { get; set; }
        //public double ProfitLoss { get; set; }

        public IEnumerable<BrokerAccount> BrokerAccounts { get; set; }
    }
}