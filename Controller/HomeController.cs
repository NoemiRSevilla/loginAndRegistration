using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using loginAndRegistration.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

// Other using statements
namespace HomeController.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;

        // here we can "inject" our context service into the constructor
        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            string UserInSession = HttpContext.Session.GetString("Email");
            if (UserInSession != null)
            {
                return RedirectToAction("Success");
            }
            else
            {
                return View();
            }
        }

        [HttpPost("submit")]
        public IActionResult Submit(User newUser)
        {
            if (ModelState.IsValid)
            {
                if (dbContext.user.Any(u => u.Email == newUser.Email))
                {
                    // Manually add a ModelState error to the Email field, with provided
                    // error message
                    ModelState.AddModelError("Email", "Email already in use!");
                    // You may consider returning to the View at this point
                    return RedirectToAction("Index");
                }
                else
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                    dbContext.Add(newUser);
                    dbContext.SaveChanges();
                    HttpContext.Session.SetString("Email", newUser.Email);
                    return RedirectToAction("Success");
                }
            }
            else
            {
                TempData["First Name"] = newUser.FirstName;
                TempData["Last Name"] = newUser.LastName;
                TempData["Email"] = newUser.Email;
                return View("Index");
            }
        }

        [HttpGet("success")]
        public IActionResult Success()
        {
            string UserInSession = HttpContext.Session.GetString("Email");
            if (UserInSession != null)
            {
                return View("Success");
            }
            else
            {
                return View("Index");
            }
        }

        [HttpGet("loginuser")]
        public IActionResult LoginUser()
        {
            string UserInSession = HttpContext.Session.GetString("Email");
            if (UserInSession != null)
            {
                return RedirectToAction("Success");
            }
            else
            {
                return View("LoginUser");
            }
        }
        
        [HttpPost("submitlogin")]
        public IActionResult submitlogin(LoginUser userSubmission)
        {
            if (ModelState.IsValid)
            {
                // If inital ModelState is valid, query for a user with provided email
                var userInDb = dbContext.user.FirstOrDefault(u => u.Email == userSubmission.Email);
                // If no user exists with provided email
                if (userInDb == null)
                {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("Email", "You could not be logged in");
                    return View("LoginUser");
                }

                // Initialize hasher object
                var hasher = new PasswordHasher<LoginUser>();

                // verify provided password against hash stored in dbcopy
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);

                // result can be compared to 0 for failure
                if (result == 0)
                {
                    ModelState.AddModelError("Email", "You could not be logged in");
                    return View("LoginUser");
                }
                HttpContext.Session.SetString("Email", userSubmission.Email);
                return RedirectToAction("Success");
            }
            else
            {
                return View("LoginUser");
            }
        }
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return View("Index");
        }
    }
}