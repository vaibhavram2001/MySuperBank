using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SuperBank.Services;
using SuperBank.ViewModels;

namespace SuperBank.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Login  Input { get; set; }

        SignInManager<IdentityUser> SignInManager;

        private readonly UserManager<IdentityUser> UserManager;

        private readonly BankAccountService _bankAccountService;

        public BankAccountService BankAccountService => _bankAccountService;

        public LoginModel(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager , BankAccountService bankAccountService)
        {
            SignInManager = signInManager;
            UserManager = userManager;
            _bankAccountService = bankAccountService;
        }


        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            if (SignInManager.IsSignedIn(User))
            {
                return RedirectToPage("/Account/Logout"); // Redirect to logout page if signed in
            }
            else
            {
                return Page();
            }

            
        }



        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(Input.Email);

                if (user != null)
                {
                    var result = await SignInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, false);

                 

                    if (result.Succeeded)
                    {
                        var HasBankAccount = await BankAccountService.UserHasBankAccountAsync(user.Id);

                        if (HasBankAccount!=null)
                        {
                            if (returnUrl == null || returnUrl == "/")
                            {
                                return RedirectToPage("/Index");
                            }
                            else
                            {
                                return RedirectToPage(returnUrl);
                            }
                        }
                        else
                        {
                            return RedirectToPage("/Profile");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Username or Password incorrect");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "No account found , Please Register !!!");
                }

                
            }
            return Page();
        }

    }
}
