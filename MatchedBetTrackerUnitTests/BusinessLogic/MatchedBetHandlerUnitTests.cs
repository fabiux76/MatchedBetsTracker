using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatchedBetsTracker.BusinessLogic;
using MatchedBetsTracker.Models;
using MatchedBetsTracker.ViewModels;
using NUnit.Framework;
using SharpTestsEx;

namespace MatchedBetTrackerUnitTests.BusinessLogic
{
    [TestFixture]
    public class MatchedBetHandlerUnitTests
    {
        private IMatchedBetModelController sut;

        [SetUp]
        public void SetUp()
        {
            sut = new MatchedBetModelController();
        }

        [TestCase(true, true)]
        [TestCase(false, true)]
        public void ShouldGenerateTheCorrectClassesForSimpleMatchedBet(bool isBackBack, bool validateTransactions)
        {
            const int userAccountId = 1;
            var betDate = new DateTime(2018, 5, 31);
            const double backAmount = 100;
            const int backBrokerAccountId = 11;
            const double backQuote = 1.8;
            const string betDescription = "Bet Description";
            var eventDate = new DateTime(2018, 6, 1);
            const string eventDescription = "Event Description";
            const double layQuote = 2.2;
            const int layBrokerAccountId = 12;
            const double layAmount = 98;            

            var simpleMatchedBetViewModel = new SimpleMatchedBetFormViewModel
            {
                Id = userAccountId,
                BetDate = betDate,
                BackAmount = backAmount,
                BackBrokerAccountId = backBrokerAccountId,
                BackQuote = backQuote,
                BetDescription = betDescription,
                EventDate = eventDate,
                EventDescription = eventDescription,
                LayQuote = layQuote,
                LayBrokerAccountId = layBrokerAccountId,
                ValidateTransactions = validateTransactions,
                LayAmount = layAmount,
                IsBackBack = isBackBack
            };

            var result = sut.CreateObjectsForSimpleMatchedBet(simpleMatchedBetViewModel, 1);
            result.SportEvents.Count().Should().Be(1);

            var sportEvent = result.SportEvents[0];
            sportEvent.EventDate.Should().Be.EqualTo(eventDate);
            sportEvent.EventDescription.Should().Be.EqualTo(eventDescription);
            sportEvent.Happened.Should().Be(null);

            var matchedBet = result.MatchedBet;
            matchedBet.UserAccountId.Should().Be.EqualTo(userAccountId);
            matchedBet.EventDescription.Should().Be.EqualTo(eventDescription);
            matchedBet.Status.Should().Be(MatchedBetStatus.Open);

            result.Bets.Count.Should().Be(2);            

            var firstBet = result.Bets.SingleOrDefault(b => b.BrokerAccountId == backBrokerAccountId);
            var secondBet = result.Bets.SingleOrDefault(b => b.BrokerAccountId == layBrokerAccountId);

            firstBet.BrokerAccountId.Should().Be(backBrokerAccountId);
            firstBet.MatchedBet.Should().Be(matchedBet);
            firstBet.UserAccountId.Should().Be(userAccountId);
            firstBet.BetAmount.Should().Be(backAmount);
            firstBet.BetDate.Should().Be(betDate);
            firstBet.BetDescription.Should().Be(betDescription);
            firstBet.BetStatusId.Should().Be(BetStatus.Open);
            firstBet.BetType.Should().Be(BetType.SingleBack);
            firstBet.ProfitLoss.Should().Be(0);
            firstBet.Quote.Should().Be(backQuote);
            firstBet.Responsability.Should().Be(backAmount);

            secondBet.BrokerAccountId.Should().Be(layBrokerAccountId);
            secondBet.MatchedBet.Should().Be(matchedBet);
            secondBet.UserAccountId.Should().Be(userAccountId);
            secondBet.BetAmount.Should().Be(layAmount);
            secondBet.BetDate.Should().Be(betDate);
            secondBet.BetDescription.Should().Be(betDescription);
            secondBet.BetStatusId.Should().Be(BetStatus.Open);
            secondBet.BetType.Should().Be(isBackBack ? BetType.SingleBack : BetType.SingleLay);
            secondBet.ProfitLoss.Should().Be(0);
            secondBet.Quote.Should().Be(layQuote);
            secondBet.Responsability.Should().Be(isBackBack ? layAmount : (layQuote - 1) * layAmount);

            var betEventsForFirstBet = result.BetEvents.Where(be => be.Bet == firstBet).ToList();
            betEventsForFirstBet.Count().Should().Be(1);
            var firstBetEvent = betEventsForFirstBet[0];

            firstBetEvent.SportEvent.Should().Be(sportEvent);
            firstBetEvent.Bet.Should().Be(firstBet);
            firstBetEvent.BetStatusId.Should().Be(BetStatus.Open);
            firstBetEvent.IsLay.Should().Be(false);
            firstBetEvent.Quote.Should().Be(backQuote);

            var betEventsForSecondtBet = result.BetEvents.Where(be => be.Bet == secondBet).ToList();
            betEventsForSecondtBet.Count().Should().Be(1);
            var secondBetEvent = betEventsForSecondtBet[0];

            secondBetEvent.SportEvent.Should().Be(sportEvent);
            secondBetEvent.Bet.Should().Be(secondBet);
            secondBetEvent.BetStatusId.Should().Be(BetStatus.Open);
            secondBetEvent.IsLay.Should().Be(!isBackBack);
            secondBetEvent.Quote.Should().Be(layQuote);

            var transactionsForFirstBet = result.Transactions.Where(t => t.Bet == firstBet).ToList();
            transactionsForFirstBet.Count.Should().Be(1);
            var firstBetTransaction = transactionsForFirstBet[0];

            firstBetTransaction.Bet.Should().Be(firstBet);
            firstBetTransaction.Amount.Should().Be(-backAmount);
            firstBetTransaction.BrokerAccountId.Should().Be(backBrokerAccountId);
            firstBetTransaction.Date.Should().Be(betDate);
            firstBetTransaction.TransactionTypeId.Should().Be(TransactionType.OpenBet);
            firstBetTransaction.UserAccountId.Should().Be(userAccountId);
            firstBetTransaction.Validated.Should().Be(validateTransactions);

            var transactionsForSecondBet = result.Transactions.Where(t => t.Bet == secondBet).ToList();
            transactionsForSecondBet.Count.Should().Be(1);
            var secondBetTransaction = transactionsForSecondBet[0];

            secondBetTransaction.Bet.Should().Be(secondBet);
            secondBetTransaction.Amount.Should().Be(isBackBack ? -layAmount : -secondBet.Responsability);
            secondBetTransaction.BrokerAccountId.Should().Be(layBrokerAccountId);
            secondBetTransaction.Date.Should().Be(betDate);
            secondBetTransaction.TransactionTypeId.Should().Be(TransactionType.OpenBet);
            secondBetTransaction.UserAccountId.Should().Be(userAccountId);
            secondBetTransaction.Validated.Should().Be(validateTransactions);
        }
    }
}
