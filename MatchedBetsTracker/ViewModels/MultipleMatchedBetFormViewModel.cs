using System;
using System.Collections.Generic;
using MatchedBetsTracker.Models;

namespace MatchedBetsTracker.ViewModels
{
    //In realtà qui ci sono informazioni di due sorgenti diverse che anrebbero separate: ! è l'evento sportivo, l'altra la scommessa
    public class SportEventDescriptionViewModel
    {
        public string EventDescription { get; set; }
        public DateTime SingleBetDate { get; set; }
        public DateTime EventDate { get; set; }
        public double QuoteInMultiple { get; set; } 
        public double QuoteInSingle { get; set; }
        public double SingleAmount { get; set; }
        public bool IsSingleLay { get; set; }
        public BrokerAccount SingleBrokerAccount { get; set; }
        public int SingleBrokerAccountId { get; set; }
    }    

    public class MultipleMatchedBetFormViewModel
    {
        public int Id { get; set; }
        public string BetDescription { get; set; }
        public DateTime MultipleBetDate { get; set; }
        public double MultipleQuoteTotal { get; set; } //La si può ottenere facendo le moltiplicazioni
        public double MultipleAmount { get; set; }
        public BrokerAccount MultipleBrokerAccount { get; set; }
        public int MultipleBrokerAccountId { get; set; }
        public List<SportEventDescriptionViewModel> Singles { get; set; }
        public bool ValidateTransactions { get; set; }
        public IEnumerable<BrokerAccount> BrokerAccounts { get; set; }
    }
    
}