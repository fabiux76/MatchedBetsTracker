using System;
using System.Collections.Generic;
using System.Linq;
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
        private IMatchedBetModelController _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new MatchedBetModelController(new ApplicationDbContext(), new MatchedBetsRepository(new ApplicationDbContext()));
        }

        /*
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

            var _context = new ApplicationDbContext();

            var result = _sut.CreateObjectsForSimpleMatchedBet(simpleMatchedBetViewModel, 1);
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
            result.BetEvents.Count.Should().Be(2);
            result.SportEvents.Count.Should().Be(1);
            result.Transactions.Count.Should().Be(2);

            var firstBet = result.Bets.Single(b => b.BrokerAccountId == backBrokerAccountId);
            var secondBet = result.Bets.Single(b => b.BrokerAccountId == layBrokerAccountId);

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
            firstBetEvent.BetEventType.Should().Be(BetEventType.BackHappen);
            firstBetEvent.Quote.Should().Be(backQuote);

            var betEventsForSecondtBet = result.BetEvents.Where(be => be.Bet == secondBet).ToList();
            betEventsForSecondtBet.Count().Should().Be(1);
            var secondBetEvent = betEventsForSecondtBet[0];

            secondBetEvent.SportEvent.Should().Be(sportEvent);
            secondBetEvent.Bet.Should().Be(secondBet);
            secondBetEvent.BetStatusId.Should().Be(BetStatus.Open);
            secondBetEvent.BetEventType.Should().Be(isBackBack ? BetEventType.BackNotHappen : BetEventType.Lay);
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

        [TestCase(true)]
        [TestCase(false)]
        public void ShouldGenerateTheCorrectClassesForMultipleMatchedBet(bool validateTransactions)
        {
            const int userAccountId = 1;

            var betDate = new DateTime(2018, 5, 31);
            const double multipleAmount = 100;
            const int multipleBrokerAccountId = 11;
            const double multipleQuote = 1.8;
            const string betDescription = "Bet Description";

            var event1Date = new DateTime(2018, 6, 1);
            var event1SingleBetDate = new DateTime(2018, 6, 12);
            const string event1Description = "Event1 Description";
            const bool event1IsLay = true;
            const double event1QuoteInMultiple = 1.8;
            const double event1QuoteInSingle = 1.9;
            const double event1SingleAmount = 98;
            const int event1BrokerAccountId = 12;

            var event2Date = new DateTime(2018, 6, 11);
            var event2SingleBetDate = new DateTime(2018, 6, 22);
            const string event2Description = "Event2 Description";
            const bool event2IsLay = true;
            const double event2QuoteInMultiple = 2.8;
            const double event2QuoteInSingle = 2.9;
            const double event2SingleAmount = 198;
            const int event2BrokerAccountId = 112;

            var event3Date = new DateTime(2018, 6, 21);
            var event3SingleBetDate = new DateTime(2018, 6, 30);
            const string event3Description = "Event3 Description";
            const bool event3IsLay = true;
            const double event3QuoteInMultiple = 3.8;
            const double event3QuoteInSingle = 3.9;
            const double event3SingleAmount = 298;
            const int event3BrokerAccountId = 212;

            var multipleMatchedBetFormView = new MultipleMatchedBetFormViewModel()
            {
                BetDescription = betDescription,
                MultipleAmount = multipleAmount,
                MultipleBetDate = betDate,
                MultipleBrokerAccountId = multipleBrokerAccountId,
                MultipleQuoteTotal = multipleQuote,
                ValidateTransactions = validateTransactions,                
                Singles = new List<SportEventDescriptionViewModel>
                {
                    new SportEventDescriptionViewModel()
                    {
                        EventDate = event1Date,
                        EventDescription = event1Description,
                        IsSingleLay = event1IsLay,
                        QuoteInMultiple = event1QuoteInMultiple,
                        QuoteInSingle = event1QuoteInSingle,
                        SingleAmount = event1SingleAmount,
                        SingleBrokerAccountId = event1BrokerAccountId,
                        SingleBetDate = event1SingleBetDate,                       
                    },
                    new SportEventDescriptionViewModel()
                    {
                        EventDate = event2Date,
                        EventDescription = event2Description,
                        IsSingleLay = event2IsLay,
                        QuoteInMultiple = event2QuoteInMultiple,
                        QuoteInSingle = event2QuoteInSingle,
                        SingleAmount = event2SingleAmount,
                        SingleBrokerAccountId = event2BrokerAccountId,
                        SingleBetDate = event2SingleBetDate,
                    },
                    new SportEventDescriptionViewModel()
                    {
                        EventDate = event3Date,
                        EventDescription = event3Description,
                        IsSingleLay = event3IsLay,
                        QuoteInMultiple = event3QuoteInMultiple,
                        QuoteInSingle = event3QuoteInSingle,
                        SingleAmount = event3SingleAmount,
                        SingleBrokerAccountId = event3BrokerAccountId,
                        SingleBetDate = event3SingleBetDate,
                    },
                }
            };

            var result = _sut.CreateObjectsForMultipleMatchedBet(multipleMatchedBetFormView, userAccountId);
            result.SportEvents.Count().Should().Be(3);
            result.BetEvents.Count.Should().Be(3);
            result.SportEvents.Count.Should().Be(3);
            result.Transactions.Count.Should().Be(1);

            var sportEvent1 = result.SportEvents[0];
            sportEvent1.EventDate.Should().Be.EqualTo(event1Date);
            sportEvent1.EventDescription.Should().Be.EqualTo(event1Description);
            sportEvent1.Happened.Should().Be(null);

            var sportEvent2 = result.SportEvents[1];
            sportEvent2.EventDate.Should().Be.EqualTo(event2Date);
            sportEvent2.EventDescription.Should().Be.EqualTo(event2Description);
            sportEvent2.Happened.Should().Be(null);

            var sportEvent3 = result.SportEvents[2];
            sportEvent3.EventDate.Should().Be.EqualTo(event3Date);
            sportEvent3.EventDescription.Should().Be.EqualTo(event3Description);
            sportEvent3.Happened.Should().Be(null);

            var matchedBet = result.MatchedBet;
            matchedBet.UserAccountId.Should().Be.EqualTo(userAccountId);
            //matchedBet.EventDescription.Should().Be.EqualTo(eventDescription); //TODO: PENSARE
            matchedBet.Status.Should().Be(MatchedBetStatus.Open);

            result.Bets.Count.Should().Be(1);

            var multipleBet = result.Bets[0];

            multipleBet.BrokerAccountId.Should().Be(multipleBrokerAccountId);
            multipleBet.MatchedBet.Should().Be(matchedBet);
            multipleBet.UserAccountId.Should().Be(userAccountId);
            multipleBet.BetAmount.Should().Be(multipleAmount);
            multipleBet.BetDate.Should().Be(betDate);
            multipleBet.BetDescription.Should().Be(betDescription);
            multipleBet.BetStatusId.Should().Be(BetStatus.Open);
            multipleBet.BetType.Should().Be(BetType.MultipleBack);
            multipleBet.ProfitLoss.Should().Be(0);
            multipleBet.Quote.Should().Be(multipleQuote);
            multipleBet.Responsability.Should().Be(multipleAmount);

            var betEventsForFirstBet = result.BetEvents.Where(be => be.Bet == multipleBet).ToList();
            betEventsForFirstBet.Count().Should().Be(3);

            var firstBetEvent = betEventsForFirstBet[0];
            var secondBetEvent = betEventsForFirstBet[1];
            var thirdBetEvent = betEventsForFirstBet[2];

            firstBetEvent.SportEvent.Should().Be(sportEvent1);
            firstBetEvent.Bet.Should().Be(multipleBet);
            firstBetEvent.BetStatusId.Should().Be(BetStatus.Open);
            firstBetEvent.BetEventType.Should().Be(BetEventType.BackHappen);
            firstBetEvent.Quote.Should().Be(event1QuoteInMultiple);

            secondBetEvent.SportEvent.Should().Be(sportEvent2);
            secondBetEvent.Bet.Should().Be(multipleBet);
            secondBetEvent.BetStatusId.Should().Be(BetStatus.Open);
            secondBetEvent.BetEventType.Should().Be(BetEventType.BackHappen);
            secondBetEvent.Quote.Should().Be(event2QuoteInMultiple);

            thirdBetEvent.SportEvent.Should().Be(sportEvent3);
            thirdBetEvent.Bet.Should().Be(multipleBet);
            thirdBetEvent.BetStatusId.Should().Be(BetStatus.Open);
            thirdBetEvent.BetEventType.Should().Be(BetEventType.BackHappen);
            thirdBetEvent.Quote.Should().Be(event3QuoteInMultiple);            

            var transactionsForMultipleBet = result.Transactions.Where(t => t.Bet == multipleBet).ToList();
            transactionsForMultipleBet.Count.Should().Be(1);
            var firstBetTransaction = transactionsForMultipleBet[0];

            firstBetTransaction.Bet.Should().Be(multipleBet);
            firstBetTransaction.Amount.Should().Be(-multipleAmount);
            firstBetTransaction.BrokerAccountId.Should().Be(multipleBrokerAccountId);
            firstBetTransaction.Date.Should().Be(betDate);
            firstBetTransaction.TransactionTypeId.Should().Be(TransactionType.OpenBet);
            firstBetTransaction.UserAccountId.Should().Be(userAccountId);
            firstBetTransaction.Validated.Should().Be(validateTransactions);
        }

        [Test]
        public void ShoudlParseMultiple()
        {
            const string copiedText = @"Multiplicatore Multiple Salvate
                            NOME MULTIPLA  
                            Prova per Parsing
                            MODALITÀ  
                            PARTITE

                            3
                            IMPORTO PUNTATA

                            100.00
                            RATING

                            79.61
                            GUADAGNO

                            -20.39
                            DATA	PARTITE	SCOMMESSA		QUOTA PUNTA	QUOTA BANCA	EXCHANGE	COM	BANCA	RESP

                            2018-10-12 15:57:59

                            Giovani - Vecchi

                            Giovani

                            2.000

                            2.100
	
                            0.050

                            83.81
                            copia	
                            92.19

                            2018-10-13 15:58:29

                            Scapoli - Ammogliati

                            Ammogliati

                            3.000

                            3.100
	
                            0.050

                            180.86
                            copia	
                            379.8

                            2018-10-14 15:58:44

                            Grassi - Magri

                            Grassi

                            1.500

                            1.600
	
                            0.050

                            580.65
                            copia	
                            348.39

                            9

                            10.42

                            820.38";

            var res = MatchedBetHandler.ParseMultipleMatchedBet(copiedText);

            res.BetDescription.Should().Be.EqualTo("Prova per Parsing");
            //res.MultipleBetDate.S
            res.MultipleAmount.Should().Be.EqualTo(100);
            res.MultipleQuoteTotal.Should().Be.EqualTo(9);

            res.Singles.Count.Should().Be.EqualTo(3);
            for (int i = 0; i < res.Singles.Count; i++)
            {
                var single = res.Singles[i];
                if (i == 0)
                {
                    //2018 - 10 - 12 15:57:59
                    single.EventDate.Should().Be.EqualTo(new DateTime(2018, 10, 12, 15, 57, 59));
                    single.EventDescription.Should().Be.EqualTo("Giovani - Vecchi : Giovani");
                    single.QuoteInMultiple.Should().Be.EqualTo(2);
                    single.QuoteInSingle.Should().Be.EqualTo(2.1);
                    single.SingleAmount.Should().Be.EqualTo(83.81);                    
                }
                if (i == 1)
                {
                    //2018 - 10 - 12 15:57:59
                    single.EventDate.Should().Be.EqualTo(new DateTime(2018, 10, 13, 15, 58, 29));
                    single.EventDescription.Should().Be.EqualTo("Scapoli - Ammogliati : Ammogliati");
                    single.QuoteInMultiple.Should().Be.EqualTo(3);
                    single.QuoteInSingle.Should().Be.EqualTo(3.1);
                    single.SingleAmount.Should().Be.EqualTo(180.86);
                }
                if (i == 2)
                {
                    //2018 - 10 - 12 15:57:59
                    single.EventDate.Should().Be.EqualTo(new DateTime(2018, 10, 14, 15, 58, 44));
                    single.EventDescription.Should().Be.EqualTo("Grassi - Magri : Grassi");
                    single.QuoteInMultiple.Should().Be.EqualTo(1.5);
                    single.QuoteInSingle.Should().Be.EqualTo(1.6);
                    single.SingleAmount.Should().Be.EqualTo(580.65);
                }
            }
        }
        */
    }    
}
