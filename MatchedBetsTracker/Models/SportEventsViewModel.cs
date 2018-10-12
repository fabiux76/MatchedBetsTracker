using System.Collections.Generic;

namespace MatchedBetsTracker.Models
{
    public class SportEventsViewModel
    {
        public IEnumerable<SportEvent> SportEvents { get; set; }
        public bool ShowClosed { get; set; }
    }
}