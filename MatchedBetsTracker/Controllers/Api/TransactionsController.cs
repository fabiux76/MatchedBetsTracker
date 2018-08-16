using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MatchedBetsTracker.Models;

namespace MatchedBetsTracker.Controllers.Api
{
    public class TransactionsController : ApiController
    {
        private ApplicationDbContext _context;

        public TransactionsController()
        {
            _context = new ApplicationDbContext();
        }

        //Ed il dispose?

        //GET /api/customers
        public IEnumerable<Transaction> GeTransactions()
        {
            return _context.Transactions.ToList();
        }

        //GET /api/customers/1
        public Transaction GetTransaction(int id)
        {
            var transaction = _context.Transactions.SingleOrDefault(t => t.Id == id);

            if (transaction == null) throw new HttpResponseException(HttpStatusCode.NotFound);
            return transaction;
        }

        [HttpPost]
        public Transaction CreateTransaction(Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            _context.Transactions.Add(transaction);
            return transaction;
        }

        [HttpPut]
        public void UpdateTransaction(int id, Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var transactionInDb = _context.Transactions.SingleOrDefault(t => t.Id == id);

            if (transactionInDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            //TODO: passare ad AutoMapper
            transactionInDb.Amount = transaction.Amount;
            transactionInDb.BrokerAccountId = transaction.BrokerAccountId;
            transactionInDb.Date = transaction.Date;
            transactionInDb.Id = transaction.Id;
            transactionInDb.TransactionTypeId = transaction.TransactionTypeId;
            transactionInDb.Validated = transaction.Validated;

            _context.SaveChanges();
        }

        [HttpDelete]
        public void DeleteTransaction(int id)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var transactionInDb = _context.Transactions.SingleOrDefault(t => t.Id == id);

            if (transactionInDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            _context.Transactions.Remove(transactionInDb);
            _context.SaveChanges();
        }

        //http://localhost:49804/api/transactions/UpdateTransactionValidationStatus?id=9&isValid=true
        [HttpPut]
        public string UpdateTransactionValidationStatus(int id, bool isValid)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

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


            return "ciao";
        }
    }
}
