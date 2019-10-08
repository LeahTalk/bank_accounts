using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace BankAccounts.Models
{
    public class LoginUser
    {
        // No other fields!
        public string Email {get; set;}
        public string Password { get; set; }
    }
}