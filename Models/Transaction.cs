using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAccount.Models
{
    [Table("transactions")]
    public class Transaction
    {
        [Key]
        public int TransactionId {get;set;}

        [Required]
        [Display(Name="Deposit / Withdraw:")]
        public decimal Amount {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;        
        public int UserId {get;set;}
        public User User;
    }
}