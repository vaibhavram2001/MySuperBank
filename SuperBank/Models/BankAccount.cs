using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using SuperBank.Attributes;

namespace SuperBank.Models
{
    public class BankAccount
	{
        [Key]
        public required string AccountNumber { get; set; }
        public decimal Balance { get; set; }

        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        public string Name { get; set; }
        public string PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        [MinimumAge(18, ErrorMessage = "You must be at least 18 years old.")]
        public DateTime DateOfBirth { get; set; }

        public List<Transaction> Transactions { get; set; }
    }
}

