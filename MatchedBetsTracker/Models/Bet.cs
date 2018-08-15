using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MatchedBetsTracker.Models
{
    public class Bet
    {
        public int Id { get; set; }
        public string EventDescription { get; set; }
        public string BetDescription { get; set; }
        public DateTime BetDate { get; set; }
        public DateTime EventDate { get; set; }
        public bool Validated { get; set; }        
        public bool IsLay { get; set; } //Lay = bancare
        public double Quote { get; set; }
        public double BetAmount { get; set; }
        public double Responsability { get; set; }
        public BetStatus Status { get; set; }
        public byte BetStatusId { get; set; }
        public double ProfitLoss { get; set; }        
        public MatchedBet MatchedBet { get; set; }
        public int? MatchedBetId { get; set; }        
        public BrokerAccount BrokerAccount { get; set; }
        public int BrokerAccountId { get; set; }
        
    }
}