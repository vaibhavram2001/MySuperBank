using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SuperBank.Models;

namespace SuperBank.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ForgotPasswordModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                // User is signed in, redirect to the logout page
                return RedirectToPage("/Account/Logout"); // Replace with your actual logout page path
            }

            // Continue with the normal "ForgotPassword" page logic if the user is not signed in
            return Page();
        }


        public async Task<IActionResult> OnPostAsync(string email)
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    //var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    // Send the reset password email using your email service here
                    // Include a link to the ResetPassword page with the token
                    // Example: SendResetPasswordEmail(user.Email, token);

                    TempData["Message"] = "An email with password reset instructions has been sent to your email address.";
                    return Page();
                }
            }

            TempData["ErrorMessage"] = "User not found or email address is invalid.";
            return Page();
        }
    }
}
