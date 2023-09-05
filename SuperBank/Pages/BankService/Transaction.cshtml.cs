using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using SuperBank.Models;
using SuperBank.Services;

namespace SuperBank.Pages.BankService
{
    [Authorize]
    public class TransactionModel : PageModel
    {
        
        private readonly UserManager<IdentityUser> _userManager;
        private readonly TransactionService _transactionService;
        private readonly BankAccountService _bankAccountService;

        public BankAccountService BankAccountService => _bankAccountService;




        public TransactionModel(UserManager<IdentityUser> userManager , TransactionService transactionService, BankAccountService bankAccountService)
        {
            _userManager = userManager;
            _transactionService = transactionService;
            _bankAccountService = bankAccountService;

        }

        public BankAccount UserBankAccount { get; set; } // Property to hold the user's bank account
        public List<Transaction> TransactionsList { get; set; } = new List<Transaction>();
        public List<Transaction> PostTransactionsList { get; set; } = new List<Transaction>();


        [BindProperty]
        public string DateOfCreation { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            UserBankAccount = await BankAccountService.GetUserBankAccountAsync(currentUser.Id);

            if (UserBankAccount != null)
            {

                DateTime? dateofcreation = await BankAccountService.GetEarliestTransactionDateAsync(currentUser.Id);

                if (dateofcreation.HasValue)
                {
                    DateOfCreation = dateofcreation.Value.ToString("MMM d, yyyy");
                    TransactionsList = await _transactionService.GetUserTransactionsAsync(UserBankAccount.AccountNumber, 10);

                    if (TransactionsList == null)
                    {
                        TempData["Message"] = "No Transactions";
                    }
                }
                else
                {
                    TempData["Message"] = "No Transactions"; 
                }

            }
            else
            {
                return RedirectToPage("/Profile");
            }

            return Page();

        }

    }
}
