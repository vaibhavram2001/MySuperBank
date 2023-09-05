using System;
using Microsoft.EntityFrameworkCore;
using SuperBank.Data;
using SuperBank.Models;

namespace SuperBank.Services
{
    public class BankAccountService
    {
        private readonly AuthDbContext _dbContext;
        private readonly TransactionService _transactionService; // Add this

        public BankAccountService(AuthDbContext dbContext, TransactionService transactionService)
        {
            _dbContext = dbContext;
            _transactionService = transactionService; // Initialize the TransactionService
        }

        public string GenerateAccountNumber()
        {
            var lastAccount = _dbContext.BankAccounts.OrderByDescending(a => a.AccountNumber).FirstOrDefault();

            if (lastAccount == null)
            {
                // For the first user, use the account number seed
                return "AC000001";
            }
            else
            {
                // Increment the last account number by 1 for the next user
                var lastNumber = int.Parse(lastAccount.AccountNumber.Replace("AC", ""));
                var nextNumber = lastNumber + 1;
                return $"AC{nextNumber:D6}";
            }
        }


        public async Task<bool> UserHasBankAccountAsync(string userId)
        {
            // Check if there is a bank account associated with the given user ID
            return await _dbContext.BankAccounts.AnyAsync(account => account.UserId == userId);
        }


        public async Task<BankAccount> GetUserBankAccountAsync(string userId)
        {
            return await _dbContext.BankAccounts.FirstOrDefaultAsync(a => a.UserId == userId);
        }

        public void CreateBankAccount(string userId, string name, string phoneNumber, DateTime dateOfBirth, decimal initialDeposit)
        {

            var newAccount = new BankAccount
            {
                AccountNumber = GenerateAccountNumber(),
                Balance = 0, // Initial balance set to 0
                UserId = userId,
                Name = name,
                PhoneNumber = phoneNumber,
                DateOfBirth = dateOfBirth
            };

            _dbContext.BankAccounts.Add(newAccount);
            _dbContext.SaveChanges();

            if (initialDeposit > 0)
            {
                MakeDeposit(newAccount, initialDeposit);
            }
        }

        public void MakeDeposit(BankAccount account, decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be positive for deposits.");
            }

            account.Balance += amount;

            _transactionService.CreateTransaction(account.AccountNumber, amount, TransactionType.Deposit);

            _dbContext.SaveChanges();
        }

        public void MakeWithdrawal(BankAccount account, decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be positive for withdrawals.");
            }

            if (account.Balance < amount)
            {
                throw new InvalidOperationException("Insufficient balance.");
            }

            account.Balance -= amount;

            // Create a transaction record
            _transactionService.CreateTransaction(account.AccountNumber, amount, TransactionType.Withdrawal);

            _dbContext.SaveChanges();
        }


        public async Task<DateTime?> GetEarliestTransactionDateAsync(string userId)
        {
            var userBankAccount = await GetUserBankAccountAsync(userId);

            if (userBankAccount == null)
            {
                return null; // User doesn't have a bank account
            }

            return await _dbContext.Transactions
                .Where(t => t.AccountNumber == userBankAccount.AccountNumber)
                .OrderBy(t => t.TransactionDate)
                .Select(t => t.TransactionDate)
                .FirstOrDefaultAsync();
        }

    }
}

