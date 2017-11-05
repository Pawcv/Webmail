using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Core.Models;
using Core.Models.Home;

namespace Core.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewData["Message"] = "Register";

            return View();
        }

        [HttpPost]
        public IActionResult Register([Bind("Username,Password,RepeatedPassword")] HomeUserModel model)
        {
            if (ModelState.IsValid)
            {
                // Create user logic
                return RedirectToAction("Index");
            }

            return RedirectToAction("Register");
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            //Login logic
            return RedirectToAction("Index");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
