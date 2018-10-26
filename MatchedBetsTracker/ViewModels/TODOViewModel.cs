using System.Collections.Generic;
using MatchedBetsTracker.Models;

namespace MatchedBetsTracker.ViewModels
{
    public class TodoViewModel
    {
        public List<Transaction> TransactionsToVerify { get; set; }
        public List<SportEvent> SportEventsToCheck { get; set; }
        public List<MatchedBet> MultipleBetsToLay { get; set; }
    }
}