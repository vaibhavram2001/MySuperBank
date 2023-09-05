using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SuperBank.Data;
using SuperBank.Models;
using SuperBank.Services;

namespace SuperBank.Pages
{
    [Authorize]
    public class ServicesModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly BankAccountService _bankAccountService;

        public ServicesModel(
        UserManager<IdentityUser> userManager,
        BankAccountService bankAccountService)
        {
            _userManager = userManager;
            _bankAccountService = bankAccountService;
        }

        public IdentityUser CurrentUser { get; private set; }
        public BankAccount BankAccount { get; private set; }
        public bool HasBankAccount => BankAccount != null;

        public async Task<IActionResult> OnGetAsync()
        {
            ModelState.Clear();
            CurrentUser = await _userManager.GetUserAsync(User);
            BankAccount = await _bankAccountService.GetUserBankAccountAsync(CurrentUser.Id);
            if (!HasBankAccount)
            {
                return RedirectToPage("/Profile");
            }
            return Page();
        }
    }
}
