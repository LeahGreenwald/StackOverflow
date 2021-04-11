using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StackOverflow.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StackOverflow.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly string _connectionString;
        public AccountController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConStr");
        }
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SignUp(User user, string password)
        {
            var connectionString = _connectionString;
            var repo = new QARepository(connectionString);
            repo.AddUser(user, password);
            return Redirect("/account/login");
        }
        public IActionResult Login()
        {
            if (TempData != null)
            {
                ViewBag.Message = (string)TempData["message"];
            }
            return View();
        }
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var connectionString = _connectionString;
            var repo = new QARepository(connectionString);
            var user = repo.Login(email, password);
            if (user == null)
            {
                TempData["message"] = "Invalid email/password combo";
                return Redirect("/account/login");
            }
            var claims = new List<Claim>
            {
                new Claim("user", email)
            };

            HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(claims, "Cookies", "user", "role"))).Wait();
            return Redirect("/home/index");
        }
    }
}
