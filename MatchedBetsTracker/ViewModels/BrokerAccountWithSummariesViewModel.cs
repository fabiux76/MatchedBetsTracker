using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using MatchedBetsTracker.Models;

namespace MatchedBetsTracker.ViewModels
{
    public class BrokerAccountWithSummariesViewModel
    {
        public BrokerAccount BrokerAccount { get; set; }

        public double ValidatedAmount { get; set; }

        public double TheoreticalAmount { get; set; }
    }
}