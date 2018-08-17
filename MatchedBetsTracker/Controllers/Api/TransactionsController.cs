using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using MatchedBetsTracker.Dtos;
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
        public IEnumerable<TransactionDto> GeTransactions()
        {
            return _context.Transactions.ToList().Select(Mapper.Map<Transaction, TransactionDto>);
        }

        //GET /api/customers/1
        public TransactionDto GetTransaction(int id)
        {
            var transaction = _context.Transactions.SingleOrDefault(t => t.Id == id);

            if (transaction == null) throw new HttpResponseException(HttpStatusCode.NotFound);
            return Mapper.Map<Transaction, TransactionDto>(transaction);
        }

        [HttpPost]
        public IHttpActionResult CreateTransaction(TransactionDto transactionDto)
        {
            //Questo darà eccezione perchè nel validation c'è casting a Transaction, mentre questo è un TransactionDto

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var transaction = Mapper.Map<TransactionDto, Transaction>(transactionDto);
            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            transactionDto.Id = transaction.Id;
            return Created(new Uri(Request.RequestUri + "/" + transactionDto.Id),  transactionDto);
        }

        [HttpPut]
        public void UpdateTransaction(int id, TransactionDto transactionDto)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var transactionInDb = _context.Transactions.SingleOrDefault(t => t.Id == id);

            if (transactionInDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            Mapper.Map(transactionDto, transactionInDb);

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
        public void UpdateTransactionValidationStatus(int id, bool isValid)
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
        }
    }
}
