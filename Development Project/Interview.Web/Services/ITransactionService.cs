using Interview.Web.Models;
using System.Collections.Generic;

namespace Interview.Web.Services
{
    public interface ITransactionService
    {
        IEnumerable<Transaction> GetTransactions(int customerId);
        Transaction GetTransaction(int transactionId);
        void CreateTransaction(Transaction transaction);
        void UndoTransaction(Transaction transaction);
    }
}
