using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MatchedBetsTracker.Models
{
    public class OwnerTransactions : ValidationAttribute
    {


        private readonly List<byte> _transactionsAllowedToOwnerOnly = new List<byte>
        {
            TransactionType.Correction,
            TransactionType.CreditBonus,
            TransactionType.MoneyCredit,
            TransactionType.MoneyDebit,
            TransactionType.ExpireBonus
        };

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var context = new ApplicationDbContext();
           
            var transaction = (Transaction)validationContext.ObjectInstance;
            var brokerAccount = context.BrokerAccounts.Where(ba => ba.Id == transaction.BrokerAccountId).SingleOrDefault();
            return (!_transactionsAllowedToOwnerOnly.Contains(transaction.TransactionTypeId)
                    || transaction.UserAccountId == brokerAccount.OwnerId)
                ? ValidationResult.Success
                : new ValidationResult("Transaction type allowed to owner only");
        }
    }
}