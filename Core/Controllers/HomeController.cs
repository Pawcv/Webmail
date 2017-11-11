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

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewData["Message"] = "Register";

            return View();
        }

        [HttpPost]
        public IActionResult Register([Bind("Username,Password,RepeatedPassword")] RegisterUserModel model)
        {
            if (ModelState.IsValid)
            {
                // Create user logic
                return RedirectToAction("Index");
            }

            return RedirectToAction("Register");
        }

        [HttpPost]
        public IActionResult Login([Bind("Username,Password")] LoginUserModel model)
        {
            if (ModelState.IsValid)
            {
                // authorizing user logic
                // TODO


                // if authorized redirect to mail view
                return RedirectToAction("Index", "Mail");
            }
            return RedirectToAction("Index");
        }
    }
}
