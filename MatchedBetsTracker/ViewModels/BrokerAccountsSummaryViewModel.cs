using System.Collections.Generic;

namespace MatchedBetsTracker.ViewModels
{
    public class BrokerAccountsSummaryViewModel
    {
        public IEnumerable<BrokerAccountWithSummariesViewModel> BrokerAccounts { get; set; }

        public double TotalDepositValidated { get; set; }

        public double TotalDeposit{ get; set; }

        public double TotalWithdrawnValidated { get; set; }

        public double TotalWithdrawn { get; set; }

        public double TotalBonusCreditValidated { get; set; }

        public double TotalBonusCredit{ get; set; }

        public double TotalAvailabilityValidated { get; set; }

        public double TotalAvailability { get; set; }

        public double TotalOpenResponsabilitiesValidated { get; set; }

        public double TotalOpenResponsabilities { get; set; }

        public double NetProfitValidated { get; set; }

        public double NetProfit { get; set; }

        public bool ShowInactiveAccounts { get; set; }

        public IDictionary<string, UserAccountSummary> UserAccountSummaries;
    }

    public class UserAccountSummary
    {
        public double TotalDepositValidated { get; set; }

        public double TotalDeposit { get; set; }

        public double TotalWithdrawnValidated { get; set; }

        public double TotalWithdrawn { get; set; }

        public double TotalBonusValidated { get; set; }

        public double TotalBonus { get; set; }

        public double TotalBetGainValidated { get; set; }

        public double TotalBetGain { get; set; }

        public double TotalOpenResponsabilitiesValidated { get; set; }

        public double TotalOpenResponsabilities { get; set; }

        public double NetProfitValidated { get; set; }

        public double NetProfit { get; set; }

        public double ExposureValidated { get; set; }

        public double Exposure { get; set; }
    }
}