using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SuperBank.Models;

namespace SuperBank.Data
{
    public class AuthDbContext : IdentityDbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        public DbSet<BankAccount> BankAccounts { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

    }
}

