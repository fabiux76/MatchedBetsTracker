using System;
using System.Collections.Generic;
using MatchedBetsTracker.Models;

namespace MatchedBetsTracker.ViewModels
{
    public class MultipleMatchedBetAddSingleFormViewModel
    {
        //Questi praticamente sono popolati dal server in base. Non dovrebbero nemmeno essere editabili sulla pagina
        public int MatchedBetId { get; set; }
        public int SportEventId { get; set; }
        public SportEvent SportEvent { get; set; }

        //Queste sono quelle eventualmente editabili
        public DateTime BetDate { get; set; }
        public double Quote { get; set; }
        public double Amount { get; set; }
        public BrokerAccount BrokerAccount { get; set; }
        public int BrokerAccountId { get; set; }
        public bool IsLay { get; set; }
        public bool ValidateTransactions { get; set; }

        //Lista dei broker per il popolamento della combobox
        public IEnumerable<BrokerAccount> BrokerAccounts { get; set; }
    }
}