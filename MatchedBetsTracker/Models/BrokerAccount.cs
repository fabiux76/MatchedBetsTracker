using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MatchedBetsTracker.Models
{
    public class BrokerAccount
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Broker Name")]
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        [Required]
        [Display(Name = "Initial Amount")]
        public double IntialAmount { get; set; }

        public string Notes { get; set; }

        public ICollection<Transaction> Transactions { get; set; }

        public ICollection<Bet> Bets { get; set; }

        [DefaultValue(true)]
        public bool Active { get; set; }

        public UserAccount Owner { get; set; }
        public int? OwnerId { get; set; }
    }
}