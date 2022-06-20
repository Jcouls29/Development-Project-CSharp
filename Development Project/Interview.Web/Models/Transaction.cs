using System;
using System.ComponentModel.DataAnnotations;

namespace Interview.Web.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        [Required]
        public TransactionType TransactionType { get; set; }
        public DateTime Date { get; set; }
        public int ProductCount { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }

    public enum TransactionType
    {
        Purchase,
        Used
    }
}
