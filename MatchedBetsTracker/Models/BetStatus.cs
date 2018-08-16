using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MatchedBetsTracker.Models
{
    public class BetStatus
    {
        public byte Id { get; set; }
        public string Description { get; set; }

        public static readonly byte Open = 1;
        public static readonly byte Won = 2;
        public static readonly byte Loss = 3;
    }
}