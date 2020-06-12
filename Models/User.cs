using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BankAccount.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        public int UserId {get;set;}
        [Required]
        [Display(Name="First Name:")]
        public string FirstName {get;set;}

        [Required]
        [Display(Name="Last Name:")]
        public string LastName {get;set;}

        [Required]
        [EmailAddress]
        [Display(Name="Email:")]
        public string Email {get;set;}

        [Required]
        [DataType(DataType.Password)]
        public string Password {get;set;}
        
        [NotMapped]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string Confirm {get;set;}
        public decimal Balance
        {
            get { return UserTransactions.Sum(t => t.Amount); }
        }
        public List<Transaction> UserTransactions {get;set;} = new List<Transaction>();
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }

    public class LoginUser
    {
        [Required]
        [EmailAddress]
        public string Email {get;set;}
        [Required]
        [DataType(DataType.Password)]
        public string Password {get;set;}
    }
}