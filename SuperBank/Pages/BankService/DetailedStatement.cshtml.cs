using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SuperBank.Models;
using SuperBank.Services;

namespace SuperBank.Pages.BankService
{

    [Authorize]
    public class DetailedStatementModel : PageModel
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly TransactionService _transactionService;
        private readonly BankAccountService _bankAccountService;

        public BankAccountService BankAccountService => _bankAccountService;




        public DetailedStatementModel(UserManager<IdentityUser> userManager, TransactionService transactionService, BankAccountService bankAccountService)
        {
            _userManager = userManager;
            _transactionService = transactionService;
            _bankAccountService = bankAccountService;

        }

        public BankAccount UserBankAccount { get; set; } // Property to hold the user's bank account
        private List<Transaction> TransactionsList { get; set; } = new List<Transaction>();
        public List<Transaction> FilteredTransactions { get; set; } = new List<Transaction>();


        public async Task<IActionResult> OnGetAsync()
        {
            TempData.Clear();


            var currentUser = await _userManager.GetUserAsync(User);

            UserBankAccount = await BankAccountService.GetUserBankAccountAsync(currentUser.Id);

            if (UserBankAccount != null)
            {
                TransactionsList = await _transactionService.GetUserTransactionsAsync(UserBankAccount.AccountNumber, -1);

                if (TransactionsList == null)
                {
                    TempData["Message"] = "No Transactions";
                }
                else
                {
                    var periodOption = Request.Query["periodOption"];
                    var startDate = Request.Query["startDate"];
                    var endDate = Request.Query["endDate"];
                    var minAmount = Request.Query["minAmount"];
                    var maxAmount = Request.Query["maxAmount"];
                    var type = Request.Query["type"];

                    DateTime? parsedStartDate = null;
                    DateTime? parsedEndDate = null;

                    // Parse and validate the custom start date
                    if (!string.IsNullOrEmpty(startDate) && DateTime.TryParse(startDate, out var start))
                    {
                        parsedStartDate = start;
                    }

                    // Parse and validate the custom end date
                    if (!string.IsNullOrEmpty(endDate) && DateTime.TryParse(endDate, out var end))
                    {
                        parsedEndDate = end;
                    }

                    // Determine the selected date range based on the radio button if not provided in custom inputs
                    if (!parsedStartDate.HasValue && !parsedEndDate.HasValue && !string.IsNullOrEmpty(periodOption))
                    {
                        DateTime currentDate = DateTime.Now;
                        if (periodOption == "LastMonth")
                        {
                            parsedStartDate = currentDate.AddMonths(-1);
                            parsedEndDate = currentDate;
                        }
                        else if (periodOption == "Last3Months")
                        {
                            parsedStartDate = currentDate.AddMonths(-3);
                            parsedEndDate = currentDate;
                        }
                        else if (periodOption == "Last6Months")
                        {
                            parsedStartDate = currentDate.AddMonths(-6);
                            parsedEndDate = currentDate;
                        }
                        else if (periodOption == "LastYear")
                        {
                            parsedStartDate = currentDate.AddYears(-1);
                            parsedEndDate = currentDate;
                        }
                    }

                    FilteredTransactions = TransactionsList;

                    if (parsedStartDate.HasValue && parsedEndDate.HasValue)
                    {
                        TempData["startDate"] = parsedStartDate.Value.ToString("MM/dd/yyyy");
                        TempData["endDate"] = parsedEndDate.Value.ToString("MM/dd/yyyy");
                        FilteredTransactions = FilteredTransactions.Where(t => t.TransactionDate >= parsedStartDate && t.TransactionDate <= parsedEndDate).ToList();
                    }

                    TempData["mini"] = null;
                    if (decimal.TryParse(minAmount, out decimal parsedMinAmount) && parsedMinAmount!=0)
                    {
                        TempData["mini"] = parsedMinAmount;
                        FilteredTransactions = FilteredTransactions.Where(t => t.TransactionAmount >= parsedMinAmount).ToList();
                    }

                    TempData["maxi"] = null;
                    if (decimal.TryParse(maxAmount, out decimal parsedMaxAmount) && parsedMaxAmount!=0)
                    {
                        TempData["maxi"] = parsedMaxAmount;
                        FilteredTransactions = FilteredTransactions.Where(t => t.TransactionAmount <= parsedMaxAmount).ToList();
                    }
                    


                    if (!string.IsNullOrEmpty(type))
                    {
                        string mappedType = MapType(type);
                        TempData["Type"] = mappedType;
                        FilteredTransactions = FilteredTransactions.Where(t => t.TransactionType.ToString() == mappedType).ToList();
                    }
                    else
                    {
                        TempData["Type"] = "Any";
                    }

                    
                    TempData["Count"] = FilteredTransactions.Count;
                    
                    return Page();
                }
            }
            else
            {
                return RedirectToPage("/Profile");
            }

            return Page();

        }

        private string MapType(string inputType)
        {
            switch (inputType.ToUpper())
            {
                case "DR":
                    return TransactionType.Withdrawal.ToString();
                case "CR":
                    return TransactionType.Deposit.ToString();
                default:
                    return inputType;
            }
        }


    }
}
