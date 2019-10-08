using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BankAccounts.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BankAccounts.Controllers
{
    public class HomeController : Controller
    {
        private BankContext dbContext;
        public HomeController(BankContext context)
        {
            dbContext = context;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            if(HttpContext.Session.GetInt32("curUser") != null) {
                return RedirectToAction("Success");
            }
            return View();
        }

        [HttpPost]
        [Route("/register")]
        public IActionResult Register(User user)
        {
            if(ModelState.IsValid) {
                if(dbContext.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                dbContext.Add(user);
                dbContext.SaveChanges();
                HttpContext.Session.SetInt32("curUser", user.UserId);
                return Redirect($"/account/{user.UserId}");
            }
            return View("Index");
        }

        [HttpPost]
        [Route("/process_login")]
        public IActionResult ProcessLogin(LoginUser userSubmission) {
            if(ModelState.IsValid)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.Email);
                if(userInDb == null)
                {
                    ModelState.AddModelError("Email", "There is no user with this email address!");
                    return View("Login");
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);
                // result can be compared to 0 for failure
                if(result == 0)
                {
                    ModelState.AddModelError("Password", "Incorrect Password!");
                    return View("Login");
                }
                HttpContext.Session.SetInt32("curUser", userInDb.UserId);
                return Redirect($"/account/{userInDb.UserId}");
            }
            return View("Login");
        }

        [HttpGet]
        [Route("/login")]
        public IActionResult Login()
        {
            if(HttpContext.Session.GetInt32("curUser") != null) {
                int curUser = (int)HttpContext.Session.GetInt32("curUser");
                return Redirect($"/account/{curUser}");
            }
            return View();
        }

        [HttpGet]
        [Route("/account/{userId}")]
        public IActionResult Account(int userId) {
            if(HttpContext.Session.GetInt32("curUser") == null) {
                return RedirectToAction("Index");
            }
            int curUser = (int)HttpContext.Session.GetInt32("curUser");
            if(userId != curUser) {
                return Redirect($"/account/{curUser}");
            }
            User user = dbContext.Users.Include(u => u.CreatedTransactions).
            FirstOrDefault(u => u.UserId == userId);
            user.CreatedTransactions.Reverse();
            ViewBag.curUser = user;
            return View();
        }

        [HttpPost]
        [Route("/transaction/new")] 
        public IActionResult CreateTransaction(Transaction newTransaction) {
            if(HttpContext.Session.GetInt32("curUser") == null) {
                return RedirectToAction("Index");
            }
            int curUser = (int)HttpContext.Session.GetInt32("curUser");
            User user = dbContext.Users.Include(u => u.CreatedTransactions)
                .FirstOrDefault(u => u.UserId == curUser);
            user.CreatedTransactions.Reverse();
            ViewBag.curUser = user;
            if(ModelState.IsValid){
                if(newTransaction.Amount < 0 && (Math.Abs(newTransaction.Amount) > user.Balance)) {
                    ModelState.AddModelError("Amount", "Cannot withdraw more than the current balance!");
                    return View("Account");
                }
                newTransaction.UserId = curUser;
                dbContext.Add(newTransaction);
                user.Balance += newTransaction.Amount;
                dbContext.SaveChanges();
                
                return Redirect($"/account/{curUser}");
            }
            return View("Account");
        }

        [HttpGet]
        [Route("/logout")]
        public IActionResult Logout() {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
