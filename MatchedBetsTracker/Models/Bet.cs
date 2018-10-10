using System;
using System.Collections.Generic;

namespace MatchedBetsTracker.Models
{
    public class Bet
    {
        public int Id { get; set; }
        public string BetDescription { get; set; }
        public DateTime BetDate { get; set; }
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
        public ICollection<Transaction> Transactions { get; set; }
        public UserAccount UserAccount { get; set; }
        public int UserAccountId { get; set; }
        public ICollection<BetEvent> BetEvents { get; set; }
        public BetType BetType { get; set; } //Questo deve diventare singola o multipla... Ma in realtà si capisce dalla lista...
    }

    public enum BetType
    {
        SingleBack = 0,
        SingleLay = 1,
        MultipleBack = 2
    }

    public class BetEvent
    {
        public int Id { get; set; }
        public Bet Bet { get; set; }
        public int BetId { get; set; }
        public BetStatus Status { get; set; }
        public byte BetStatusId { get; set; }
        public double Quote { get; set; }
        public SportEvent SportEvent { get; set; }
        public int SportEventId { get; set; }
        public BetEventType BetEventType { get; set; }
    }

    public enum BetEventType
    {
        BackHappen = 1,
        BackNotHappen = 2,
        Lay = 3
    }

    public class SportEvent
    {
        public int Id { get; set; }
        public string EventDescription { get; set; }
        public DateTime EventDate { get; set; }
        public bool? Happened { get; set; }
        public ICollection<BetEvent> BetEvents { get; set; }
    }
}