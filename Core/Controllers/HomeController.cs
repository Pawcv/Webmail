using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Logging;
using Core.Services;
using Core.Data;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Core.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly UrlEncoder _urlEncoder;
        private readonly ApplicationDbContext _dbContext;

        public HomeController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<ManageController> logger,
            UrlEncoder urlEncoder,
            ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _urlEncoder = urlEncoder;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                throw new ApplicationException($"User ID was not found in user claims!");
            }

            var user = await _dbContext.Users.Include(appUser => appUser.ImapModel).SingleOrDefaultAsync(appUser => appUser.Id == userId);

            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (user.ImapModel == null)
            {
                return RedirectToAction("SelectImapProvider", "Manage");
            }

            ImapClientModel model = new ImapClientModel(user.ImapModel.login,
                                                        user.ImapModel.password,
                                                        user.ImapModel.host,
                                                        user.ImapModel.port,
                                                        user.ImapModel.useSsl);

            return RedirectToAction("Mail", "ShowMailsView", model);
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
    }
}
