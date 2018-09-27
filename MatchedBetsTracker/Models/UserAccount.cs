using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MatchedBetsTracker.Models
{
    public class UserAccount
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<BrokerAccount> BrokerAccounts { get; set; }
    }
}