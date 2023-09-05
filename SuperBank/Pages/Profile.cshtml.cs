using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SuperBank.Attributes;
using SuperBank.Data;
using SuperBank.Models;
using SuperBank.Services;

namespace SuperBank.Pages
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly BankAccountService _bankAccountService;

        public BankAccountService BankAccountService => _bankAccountService;

        public ProfileModel(
        UserManager<IdentityUser> userManager,
        BankAccountService bankAccountService)
        {
            _userManager = userManager;
            _bankAccountService = bankAccountService; 
        }

        [BindProperty]
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        [BindProperty]
        [MinimumAge(18, ErrorMessage = "You must be at least 18 years old.")]
        public DateTime DateOfBirth { get; set; }
        [BindProperty]
        public decimal InitialDeposit { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Phone Number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone Number must be 10 digits.")]
        public string PhoneNumber { get; set; }


        public IdentityUser CurrentUser { get; private set; }
        public BankAccount BankAccount { get; private set; }
        public bool HasBankAccount => BankAccount != null;

        public async Task OnGetAsync()
        {
            ModelState.Clear();
            CurrentUser = await _userManager.GetUserAsync(User);
            BankAccount = await BankAccountService.GetUserBankAccountAsync(CurrentUser.Id);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                CurrentUser = await _userManager.GetUserAsync(User);

                if (InitialDeposit <= 100)
                {
                    ModelState.AddModelError(nameof(InitialDeposit), "Initial Deposit must be greater than 100.");
                    return Page();
                }

                _bankAccountService.CreateBankAccount(CurrentUser.Id, Name, PhoneNumber, DateOfBirth, InitialDeposit);

                return RedirectToPage();
            }

            return Page();
        }

    }

}
