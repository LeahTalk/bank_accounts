using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
namespace BankAccounts.Models
{
    public class User
    {
        // auto-implemented properties need to match the columns in your table
        // the [Key] attribute is used to mark the Model property being used for your table's Primary Key
        [Key]
        public int UserId { get; set; }
        // MySQL VARCHAR and TEXT types can be represeted by a string
        [Required(ErrorMessage = "First Name is required!")]
        [MinLength(2, ErrorMessage="First Name must be at least 2 characters!")]
        [Display(Name = "First Name:")] 
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required!")]
        [MinLength(2, ErrorMessage="Last Name must be at least 2 characters!")]
        [Display(Name = "Last Name:")] 
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email is required!")]
        [Display(Name = "Email:")] 
        [EmailAddress]
        public string Email { get; set; }
        //[DataType(DataType.Password)]
        [Display(Name = "Password:")] 
        [MinLength(8, ErrorMessage="Password must be 8 characters or longer!")]
        public string Password { get; set; }
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
        public int Balance {get;set;} = 0;

        [NotMapped]
        [Compare("Password", ErrorMessage = "Passwords do not match!")]
        [Display(Name = "Confirm Password:")] 
        //[DataType(DataType.Password)]
        public string Confirm {get;set;}

        public List<Transaction> CreatedTransactions {get;set;}
    }
}