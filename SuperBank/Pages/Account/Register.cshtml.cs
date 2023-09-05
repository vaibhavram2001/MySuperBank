using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SuperBank.Data;
using SuperBank.Models;
using SuperBank.ViewModels;


namespace SuperBank.Pages.Account
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public Register Input { get; set; }
        private UserManager<IdentityUser> UserManager { get; }
        private SignInManager<IdentityUser> SignInManager { get; }

        public RegisterModel(UserManager<IdentityUser> userManager , SignInManager<IdentityUser> signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }


        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            if (SignInManager.IsSignedIn(User))
            {
                return RedirectToPage("/Account/Logout"); // Redirect to Logout page if signed in
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser()
                {
                    UserName = Input.Email,
                    Email = Input.Email
                };

                var result = await UserManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, false);
                    return RedirectToPage("/Profile");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

            }
            return Page();
        }
        
    }
}
