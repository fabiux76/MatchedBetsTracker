using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MatchedBetsTracker.Models
{
    public class BrokerAccount
    {
        public int Id { get; set; }

        [Display(Name = "Broker Name")]
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        [Display(Name = "Initial Amount")]
        public double IntialAmount { get; set; }
    }
}