using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SuperBank.Data;
using SuperBank.Models;
using SuperBank.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace SuperBank.Pages.BankService
{
    [Authorize]
    public class WithdrawModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly BankAccountService _bankAccountService;

        public BankAccountService BankAccountService => _bankAccountService;

        public WithdrawModel(UserManager<IdentityUser> userManager,
        BankAccountService bankAccountService)
        {
            _userManager = userManager;
            _bankAccountService = bankAccountService;
           
        }

        [BindProperty]
        public decimal WithdrawAmount { get; set; }

        [TempData]
        public string WithdrawAlert { get; set; } // Store the alert message in TempData

        public IdentityUser CurrentUser { get; private set; }
        public BankAccount BankAccount { get; private set; }



        public async Task<IActionResult> OnGetAsync()
        {
            ModelState.Clear();

            CurrentUser = await _userManager.GetUserAsync(User);

            BankAccount = await BankAccountService.GetUserBankAccountAsync(CurrentUser.Id);

            if (BankAccount == null)
            {
                return RedirectToPage("/Profile");
            }
            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                CurrentUser = await _userManager.GetUserAsync(User);

                BankAccount = await BankAccountService.GetUserBankAccountAsync(CurrentUser.Id);

                if (WithdrawAmount > 0)
                {

                    if (BankAccount != null)
                    {
                        try
                        {
                            // Attempt to make the withdrawal
                            _bankAccountService.MakeWithdrawal(BankAccount, WithdrawAmount);

                            WithdrawAlert = $"${WithdrawAmount} debited. Your remaining balance is ${BankAccount.Balance}";

                            return RedirectToPage();
                        }
                        catch (InvalidOperationException ex)
                        {
                            // Handle insufficient balance or other errors
                            ModelState.AddModelError(nameof(WithdrawAmount), ex.Message);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(WithdrawAmount), "Bank account not found.");
                    }
                }
                else
                {
                    ModelState.AddModelError(nameof(WithdrawAmount), "Amount must be positive for withdrawals.");
                }
            }

            return Page();
        }
    }
}
