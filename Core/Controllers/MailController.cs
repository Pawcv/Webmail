using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MimeKit;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Core.Controllers
{
    [Authorize]
    public class MailController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;

        public MailController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _userManager.FindByIdAsync(id).Result;
            return View();
        }

        [HttpPost]
        public IActionResult SendMail(string login, string password, string message)
        {
            var mailSender = new MailSenderModel(login, password);
            mailSender.Connect();
            mailSender.SendMessage(message);
            mailSender.Disconnect();
            return RedirectToAction("Index");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult ImapClientTest(string login, string password, string host, int port, bool useSsl)
        {
            var imapClientModel = new ImapClientModel(login, password, host, port, useSsl);
            imapClientModel.Connect();
            imapClientModel.ActiveFolder = "INBOX";
            return View("ShowMailsView", imapClientModel);
        }

        public IActionResult CreateMail()
        {
            MailMessageModel model = new MailMessageModel();
            return View("CreateMail");
        }

        [HttpPost]
        public IActionResult CreateMail(MailMessageModel model)
        {
            model.Connect();
            model.SendMessage();
            model.Disconnect();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult GetMessage(int? id)
        {
            // pobranie wiadomości o danym id z aktualnego folderu
            return new JsonResult("<p>Jakas wiadomosc</p>");
        }
    }
}
