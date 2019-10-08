using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
namespace BankAccounts.Models
{
    public class Transaction
    {
        // auto-implemented properties need to match the columns in your table
        // the [Key] attribute is used to mark the Model property being used for your table's Primary Key
        [Key]
        public int TransactionId { get; set; }
        // MySQL VARCHAR and TEXT types can be represeted by a string
        [Required(ErrorMessage = "Amount is required!")]
        public int Amount { get; set; }

        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

        public int UserId { get; set; }

        public User Creator { get; set; }

    }
}