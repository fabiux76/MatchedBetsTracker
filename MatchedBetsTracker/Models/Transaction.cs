using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MatchedBetsTracker.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public TransactionType TransactionType { get; set; }
        public double Amoount { get; set; }
        public bool Validated { get; set; }
    }
}