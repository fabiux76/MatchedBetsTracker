using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MatchedBetsTracker.Models;

namespace MatchedBetsTracker.Dtos
{
    public class TransactionDto
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public byte TransactionTypeId { get; set; }

        [TransactionSign]
        public double Amount { get; set; }

        public bool Validated { get; set; }

        public int BrokerAccountId { get; set; }

        public Bet Bet { get; set; }

        public int? BetId { get; set; }
    }
}