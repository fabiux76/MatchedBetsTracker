using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using MatchedBetsTracker.Dtos;
using MatchedBetsTracker.Models;
using MatchedBetsTracker.BusinessLogic;
using System.Data.Entity;

namespace MatchedBetsTracker.Controllers.Api
{
    public class BetsController : ApiController
    {
        private ApplicationDbContext _context;

        public BetsController()
        {
            _context = new ApplicationDbContext();
        }

        //http://localhost:49804/api/transactions/UpdateTransactionValidationStatus?id=9&isValid=true
        [HttpPut]
        public void UpdateBetStatus(int id, byte status)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var betInDb = _context.Bets.Include(bet => bet.MatchedBet)
                                       .SingleOrDefault(b => b.Id == id);
            if (betInDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            if (betInDb.BetStatusId != status)
            {
                betInDb.BetStatusId = status;
                MatchedBetHandler.RecomputeBetResponsabilityAndProfit(betInDb);

                var creditBetTransactions = _context.Transactions.Where(t => t.BetId == betInDb.Id &&
                    t.TransactionTypeId == TransactionType.CreditBet).ToList();

                if (creditBetTransactions != null)
                {
                    _context.Transactions.RemoveRange(creditBetTransactions);
                }
                
                if (status == BetStatus.Won)
                {
                    var winningTransaction = MatchedBetHandler.CreateCloseBetTransaction(betInDb);
                    _context.Transactions.Add(winningTransaction);
                }

                //Update also MAtchedetStatus if all set
                if (status == BetStatus.Open)
                {
                    betInDb.MatchedBet.Status = MatchedBetStatus.Open;
                }
                else
                {
                    //Get all other bets of matchedBet
                    var allBetsOfMatchedBet = _context.Bets.Where(bet => bet.MatchedBetId == betInDb.MatchedBetId);
                    if (!allBetsOfMatchedBet.Any(bet => bet.BetStatusId == BetStatus.Open))
                    {
                        //Per convenzione anche se non avrebbe molto senso in questo caso...
                        betInDb.MatchedBet.Status = MatchedBetStatus.BackWon;
                    }
                }

                _context.SaveChanges();
            }
            

            /*
            var transactionInDb = _context.Transactions.SingleOrDefault(t => t.Id == id);

            if (transactionInDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            transactionInDb.Validated = isValid;

            //TODO: sistemare...
            try
            {
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            */
        }
    }
}
