using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SuperBank.Models
{
	public class Transaction
	{
        [Key]
        public int TransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal TransactionAmount { get; set; }

        public TransactionType TransactionType { get; set; }

        [ForeignKey("AccountNumber")]
        public string AccountNumber { get; set; }
        public BankAccount BankAccount { get; set; }
    }
}

public enum TransactionType
{
    Deposit,
    Withdrawal
}
