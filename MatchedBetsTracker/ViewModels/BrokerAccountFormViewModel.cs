using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MatchedBetsTracker.Models;

namespace MatchedBetsTracker.ViewModels
{
	public class BrokerAccountFormViewModel
	{
	    public BrokerAccount BrokerAccount { get; set; }
	    public IEnumerable<UserAccount> UserAccounts { get; set; }
    }
}