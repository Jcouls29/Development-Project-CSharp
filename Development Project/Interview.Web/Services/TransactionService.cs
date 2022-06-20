using Interview.Web.Models;
using Interview.Web.Data;
using System.Linq;
using System.Collections.Generic;

namespace Interview.Web.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly InventoryContext _inventoryContext;
        public TransactionService(InventoryContext context)
        {
            _inventoryContext = context;
        }
        public void CreateTransaction(Transaction transaction)
        {
            _inventoryContext.Transaction.Update(transaction);
            _inventoryContext.SaveChangesAsync();
        }

        public Transaction GetTransaction(int transactionId)
        {
            return _inventoryContext.Transaction.FindAsync(transactionId).Result;
        }

        public IEnumerable<Transaction> GetTransactions(int customerId)
        {
            return _inventoryContext.Transaction.Where(x => x.CustomerId == customerId);
        }

        public void UndoTransaction(Transaction transaction)
        {
        }
    }
}
