using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BankAccount.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BankAccount.Controllers
{
    public class HomeController : Controller
    {
        // Create session property to hold UserId
        private int? UserSession
        {
            get { return HttpContext.Session.GetInt32("UserId"); }
            set { HttpContext.Session.SetInt32("UserId", (int)value); }
        }
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }
        
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("create")]
        public IActionResult Create(User newUser)
        {
            if (ModelState.IsValid)
            {
                // Check if email already exists in db
                var existingUser = dbContext.Users.FirstOrDefault(u => u.Email == newUser.Email);
                if (existingUser != null)
                {
                    // Create an error if email already in db
                    ModelState.AddModelError("Email", "Email already in use");
                    return View("Index");
                }
                // Hash password if new user
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                dbContext.Users.Add(newUser);
                dbContext.SaveChanges();
                // Put user in session
                UserSession = newUser.UserId;
                return RedirectToAction("Account");
            }
            return View("Index");
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("loginUser")]
        public IActionResult LoginUser(LoginUser currUser)
        {
            if (ModelState.IsValid)
            {
                // Make sure email is in db
                var existingUser = dbContext.Users.FirstOrDefault(u => u.Email == currUser.Email);
                if (existingUser == null)
                {
                    // Create error if email not in db
                    ModelState.AddModelError("Email", "Password's Invalid!");
                    return View("Login");
                }
                // Compare given password to password in db
                PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(currUser, existingUser.Password, currUser.Password);
                if (result == 0)
                {
                    // Create error if passwords don't match
                    ModelState.AddModelError("Email", "Password's Invalid!");
                    return View("Login");
                }
                UserSession = existingUser.UserId;
                return RedirectToAction("Account");
            }
            return View("Login");
        }

        [HttpGet("account")]
        public IActionResult Account()
        {
            // Redirect to login page if user not in session
            if (UserSession == null)
                return RedirectToAction("Login");
            
            // Grab user with included transactions
            User thisUser = dbContext.Users.Include(u => u.UserTransactions).FirstOrDefault(u => u.UserId == UserSession);
            ViewBag.User = thisUser;
            ViewBag.Transactions = dbContext.Transactions
                .OrderByDescending(t => t.CreatedAt)
                .Where(t => t.UserId == thisUser.UserId);
            return View();
        }

        [HttpPost("transaction")]
        public IActionResult Transaction(Transaction transaction)
        {
            // Get user with included user transactions
            User thisUser = dbContext.Users.Include(u => u.UserTransactions).FirstOrDefault(u => u.UserId == UserSession);
            if (ModelState.IsValid)
            {
                // Create error if user tries to withdraw more money than they have
                if (transaction.Amount < (0 - thisUser.Balance))
                {
                    ModelState.AddModelError("Amount", "You Do Not Have Enough Money, Sorry!");
                    // Set viewbag again to display correct info while allowing errors to show
                    ViewBag.User = thisUser;
                    ViewBag.Transactions = dbContext.Transactions
                        .OrderByDescending(t => t.CreatedAt)
                        .Where(t => t.UserId == thisUser.UserId);
                    return View("Account");
                }
                // Create transaction if no errors
                dbContext.Transactions.Add(transaction);
                dbContext.SaveChanges();
                return RedirectToAction("Account");
            }
            ViewBag.User = thisUser;
            ViewBag.Transactions = dbContext.Transactions
                .OrderByDescending(t => t.CreatedAt)
                .Where(t => t.UserId == thisUser.UserId);
            return View("Account");
        }
    }
}