using System;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using SuperBank.Data;
using SuperBank.Models;

namespace SuperBank.Services
{
    public class TransactionService
    {
        private readonly AuthDbContext _dbContext;

        public TransactionService(AuthDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateTransaction(string accountNumber, decimal amount, TransactionType type)
        {
            var transaction = new Transaction
            {
                AccountNumber = accountNumber,
                TransactionDate = DateTime.UtcNow,
                TransactionAmount = amount,
                TransactionType = type
            };

            _dbContext.Transactions.Add(transaction);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Transaction>> GetUserTransactionsAsync(string accountNumber , int maxCount = -1)
        {
            // Query your database to retrieve the user's transactions by joining Transaction and BankAccount tables


            var query = _dbContext.Transactions
                .Where(t => t.AccountNumber == accountNumber) // Assuming BankAccount has a UserId property
                .OrderByDescending(t => t.TransactionDate);
                
            if (maxCount != -1)
            {
                query = (IOrderedQueryable<Transaction>)query.Take(maxCount);
            }

            return await query.ToListAsync(); ;
        }
    }
}

