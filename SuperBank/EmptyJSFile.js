




public async Task < IActionResult > OnPostRetrieveTransactionsAsync()
{

    var currentUser = await _userManager.GetUserAsync(User);

    // Find the bank account associated with the current user
    SameBankAccount = await _dbContext.BankAccounts.FirstOrDefaultAsync(a => a.UserId == currentUser.Id);


    // Retrieve the user's selected options from the form
    var periodOption = Request.Form["periodOption"];
    var startDate = Request.Form["startDate"];
    var endDate = Request.Form["endDate"];
    var minAmount = Request.Form["minAmount"];
    var maxAmount = Request.Form["maxAmount"];
    var transactionType = Request.Form["type"];

    // Check if the start date is after the end date
    if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate)) {
        if (DateTime.TryParse(startDate, out var start) && DateTime.TryParse(endDate, out var end) && start > end)
        {
            ModelState.AddModelError("", "Start date can't be after the End date.");
        }
    }

    // Check if either radio buttons or custom dates are selected
    if (string.IsNullOrEmpty(periodOption) && (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate))) {
        ModelState.AddModelError(string.Empty, "Please select a period or enter a custom date range.");
    }

    // Validate minimum and maximum amount
    if (!string.IsNullOrEmpty(minAmount) && !string.IsNullOrEmpty(maxAmount)) {
        if (!decimal.TryParse(minAmount, out var min) || !decimal.TryParse(maxAmount, out var max) || min < 0 || max < 0 || max < min)
        {
            ModelState.AddModelError(string.Empty, "Invalid minimum or maximum amount values.");
        }
    }

    // If there are any validation errors, return to the form with error messages
    if (!ModelState.IsValid) {
        // You can use ModelState.AddModelError for each field to specify which field the error is associated with.
        // For example, ModelState.AddModelError("startDate", "Start date can't be after the End date.");
        // This will display the error message next to the corresponding input field in the view.
        return Page();
    }

    // If no errors, proceed to retrieve the filtered transactions and display them
    var transactions = await _transactionService.GetUserTransactionsAsync(SameBankAccount.AccountNumber, -1);

    // Pass the retrieved transactions to your view model or ViewData
    DetailedTransactionsList = transactions;

    if (DetailedTransactionsList == null) {
        TempData["NewMessage"] = "No Transactions";
    }

    // Return the view to display the filtered transactions
    return Page();
} }