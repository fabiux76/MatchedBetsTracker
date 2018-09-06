using MatchedBetsTracker.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MatchedBetsTracker.ViewModels
{
    public class SimpleMatchedBetFormViewModel
    {
        public int Id { get; set; }
        [Required]
        public string EventDescription { get; set; }
        [Required]
        public string BetDescription { get; set; }
        [Required]
        public DateTime BetDate { get; set; }
        [Required]
        public DateTime EventDate { get; set; }     
        public double BackQuote { get; set; }
        public double BackAmount { get; set; }
        public BrokerAccount BackBrokerAccount { get; set; }
        public int BackBrokerAccountId { get; set; }
        public double LayQuote { get; set; }
        public double LayAmount { get; set; }
        public BrokerAccount LayBrokerAccount { get; set; }
        public int LayBrokerAccountId { get; set; }

        public bool ValidateTransactions { get; set; }

        public bool IsBackBack { get; set; }

        public IEnumerable<BrokerAccount> BrokerAccounts { get; set; }
    }
}