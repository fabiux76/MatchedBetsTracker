using MatchedBetsTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MatchedBetsTracker.ViewModels
{
    public class BetFormViewModel
    {
        public Bet Bet { get; set; }
        public IEnumerable<BetStatus> BetStatuses { get; set; }
        public IEnumerable<BrokerAccount> BrokerAccounts { get; set; }
    }
}