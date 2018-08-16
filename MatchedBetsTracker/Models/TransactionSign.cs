using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MatchedBetsTracker.BusinessLogic;

namespace MatchedBetsTracker.Models
{
    public class TransactionSign : ValidationAttribute
    {
        private readonly Dictionary<byte, bool> _signForTransaction = new Dictionary<byte, bool>
        {
            {TransactionType.CreditBet, true},
            {TransactionType.CreditBonus, true },
            {TransactionType.ExpireBonus, false },
            {TransactionType.MoneyCredit, true },
            {TransactionType.MoneyDebit, false },
            {TransactionType.OpenBet, false }
        };

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var transaction = (Transaction) validationContext.ObjectInstance;

            if (Math.Abs(transaction.Amount) < 0.01 ) return new ValidationResult("Transaction amount must not be 0");

            return _signForTransaction[transaction.TransactionTypeId] == transaction.Amount > 0 
                ? ValidationResult.Success 
                : new ValidationResult("Amount sign not compatible with transaction");
        }
    }
}