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
    public class WallController : Controller
    {

        private MyContext dbContext;

        public WallController(MyContext context)
        {
            dbContext = context;
        }

        [HttpGet("Dashboard")]
        public IActionResult success()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return View("Index", "Home");
            }
            List<Message> allM = dbContext.Messages.ToList();
            ViewBag.allinfo = dbContext.Messages.Include(a => a.UserCreated).ThenInclude(f => f.UserComments).ToList().OrderByDescending(e => e.CreatedAt);
            ViewBag.userInfo = dbContext.Users.FirstOrDefault(a => a.UserId == (int)HttpContext.Session.GetInt32("UserId"));
            return View();
        }

        [HttpPost("AddMessage")]
        public IActionResult addM(Message message)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return View("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                User UC = dbContext.Users.FirstOrDefault(a => a.UserId == (int)HttpContext.Session.GetInt32("UserId"));
                message.UserCreated = UC;
                message.UserId = UC.UserId;
                dbContext.Messages.Add(message);
                dbContext.SaveChanges();
                ViewBag.userInfo = dbContext.Users.FirstOrDefault(a => a.UserId == (int)HttpContext.Session.GetInt32("UserId"));
                ViewBag.allinfo = dbContext.Messages.Include(a => a.UserCreated).ThenInclude(f => f.UserComments).ToList().OrderByDescending(e => e.CreatedAt);
                return RedirectToAction("success");
            }
            return View("success");
        }
        [HttpPost("AddC/{MID}")]
        public IActionResult addC(string CommentC, int MID)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return View("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                User UC = dbContext.Users.FirstOrDefault(a => a.UserId == (int)HttpContext.Session.GetInt32("UserId"));
                Comment comment = new Comment
                {
                    CommentContent = CommentC,
                    UserId = UC.UserId,
                    UserCreated = UC,
                    MessageId = MID,
                    MessageCommented = dbContext.Messages.FirstOrDefault(a => a.MessageId == MID)
                };
                dbContext.Comments.Add(comment);
                dbContext.SaveChanges();
                ViewBag.userInfo = dbContext.Users.FirstOrDefault(a => a.UserId == (int)HttpContext.Session.GetInt32("UserId"));
                ViewBag.allinfo = dbContext.Messages.Include(a => a.UserCreated).ThenInclude(f => f.UserComments).ToList().OrderByDescending(e => e.CreatedAt);
                return RedirectToAction("success");
            }
            return View("success");
        }
    }
}