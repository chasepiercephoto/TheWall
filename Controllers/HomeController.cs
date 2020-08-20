using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheWall.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace TheWall.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }
        [HttpGet("")]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                HttpContext.Session.Clear();
                return View();
            }
            return View();
        }

        [HttpPost("register")]
        public IActionResult reg(User newUser)
        {
            if (ModelState.IsValid)
            {
                if (dbContext.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already used, please log in!");
                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                dbContext.Add(newUser);
                dbContext.SaveChanges();
                User User = dbContext.Users.FirstOrDefault(p => p.Email == newUser.Email);
                HttpContext.Session.SetInt32("UserId", User.UserId);
                ViewBag.allinfo = dbContext.Messages.Include(a => a.UserCreated).ThenInclude(f => f.UserComments).ToList().OrderByDescending(e => e.CreatedAt);
                ViewBag.userInfo = dbContext.Users.FirstOrDefault(a => a.UserId == (int)HttpContext.Session.GetInt32("UserId"));
                return RedirectToAction("success", "Wall");
            }
            return View("Index");
        }

        [HttpPost("login")]
        public IActionResult login(LogModel user)
        {
            if (ModelState.IsValid)
            {
                var FindUser = dbContext.Users.FirstOrDefault(p => p.Email == user.LEmail);
                if (FindUser == null)
                {
                    ModelState.AddModelError("LEmail", "Invalid Email/Password");
                    return View("Index");
                }
                var hasher = new PasswordHasher<LogModel>();
                var res = hasher.VerifyHashedPassword(user, FindUser.Password, user.LPassword);
                if (res == 0)
                {
                    ModelState.AddModelError("LEmail", "Cannot log in for some reason");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("UserId", FindUser.UserId);
                ViewBag.allinfo = dbContext.Messages.Include(a => a.UserCreated).ThenInclude(f => f.UserComments).ToList().OrderByDescending(e => e.CreatedAt);
                ViewBag.userInfo = dbContext.Users.FirstOrDefault(a => a.UserId == (int)HttpContext.Session.GetInt32("UserId"));
                return RedirectToAction("success", "Wall");
            }
            return View("Index");
        }

        [HttpGet("logout")]
        public IActionResult logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
