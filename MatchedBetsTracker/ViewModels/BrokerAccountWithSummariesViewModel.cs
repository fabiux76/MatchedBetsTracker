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

        public double AmountValidated { get; set; }

        public double AmountTotal { get; set; }

        public double OpenBetsResponsabilityValidated { get; set; }

        public double OpenBetsResponsabilityTotal { get; set; }
    }
}