using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MatchedBetsTracker.Models
{
    public enum MatchedBetStatus
    {
        Open,
        BackWon,
        LayWon
    }

    public class MatchedBet
    {
        public int Id { get; set; }
        public string EventDescription { get; set; }
        public MatchedBetStatus Status { get; set; }
        public ICollection<Bet> Bets { get; set; }
    }
}