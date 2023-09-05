using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SuperBank.Attributes;
using SuperBank.Data;
using SuperBank.Models;
using SuperBank.Services;

namespace SuperBank.Pages.BankService
{
    [Authorize]
    public class DepositModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly BankAccountService _bankAccountService;

        public BankAccountService BankAccountService => _bankAccountService;

        public DepositModel(
        UserManager<IdentityUser> userManager,
        BankAccountService bankAccountService)
        {
            _userManager = userManager;
            _bankAccountService = bankAccountService;
        }

        [BindProperty]
        public decimal DepositAmount { get; set; }

        [TempData]
        public string DepositAlert { get; set; }

        public IdentityUser CurrentUser { get; private set; }
        public BankAccount BankAccount { get; private set; }
        public bool HasBankAccount => BankAccount != null;

        

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

                if (DepositAmount > 0)
                {

                    if (BankAccount != null)
                    {
                        try
                        {
                            // Attempt to make the withdrawal
                            _bankAccountService.MakeDeposit(BankAccount, DepositAmount);

                            DepositAlert = $"${DepositAmount} credited. Your remaining balance is ${BankAccount.Balance}";

                            return RedirectToPage();
                        }
                        catch (InvalidOperationException ex)
                        {
                            // Handle insufficient balance or other errors
                            ModelState.AddModelError(nameof(DepositAmount), ex.Message);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(DepositAmount), "Bank account not found.");
                    }
                }
                else
                {
                    ModelState.AddModelError(nameof(DepositAmount), "Amount must be positive for deposits.");
                }

            }

            return Page();
        }
    }
}
