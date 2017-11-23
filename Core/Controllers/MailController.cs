using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using System.Security.Claims;
using Core.Data;
using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Webmail.Smtp;
using MimeKit;
using System.Linq;

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
        private readonly ApplicationDbContext _dbContext;

        public MailController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<AccountController> logger,
            ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _dbContext = dbContext;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                throw new ApplicationException($"User ID was not found in user claims!");
            }

            var user = await _dbContext.Users
                .Include(appUser => appUser.ImapConfigurations)
                .SingleOrDefaultAsync(appUser => appUser.Id == userId);

            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!user.ImapConfigurations.Any())
            {
                return RedirectToAction(nameof(ManageController.SelectImapProvider), "Manage");
            }

            // for now using only one configuration
            var firstImapConf = user.ImapConfigurations.First();

            if (!ImapClientModel.ImapClientModelsDictionary.TryGetValue(firstImapConf.Login + firstImapConf.Password, out var model))
            {
                model = new ImapClientModel(
                    firstImapConf.Login,
                    firstImapConf.Password,
                    firstImapConf.Host,
                    firstImapConf.Port,
                    firstImapConf.UseSsl);
            }

            if (!model.IsConnected)
            {
                model.Connect();
                model.ActiveFolder = "INBOX";
            }
            return View("ShowMailsView", model);
        }

        public async Task<IActionResult> SearchCurrentFolder(string search_phrase)
        {
            search_phrase = WebUtility.UrlDecode(search_phrase);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                throw new ApplicationException($"User ID was not found in user claims!");
            }

            var user = await _dbContext.Users.Include(appUser => appUser.ImapModel).SingleOrDefaultAsync(appUser => appUser.Id == userId);

            if (!ImapClientModel.ImapClientModelsDictionary.TryGetValue(user.ImapModel.login + user.ImapModel.password, out var model))
            {
                model = new ImapClientModel(user.ImapModel.login,
                    user.ImapModel.password,
                    user.ImapModel.ImapHost,
                    user.ImapModel.ImapPort,
                    user.ImapModel.useSsl);
            }

            model.FindPhraseInCurrFolder(search_phrase);
            return View("ShowMailsView", model);
        }

        public async Task<IActionResult> ChangeActiveFolder(string folderName)
        {
            folderName = WebUtility.UrlDecode(folderName);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                throw new ApplicationException($"User ID was not found in user claims!");
            }

            var user = await _dbContext.Users
                .Include(appUser => appUser.ImapConfigurations)
                .SingleOrDefaultAsync(appUser => appUser.Id == userId);

            // for now using only one configuration
            var firstImapConf = user.ImapConfigurations.First();

            if (!ImapClientModel.ImapClientModelsDictionary.TryGetValue(firstImapConf.Login + firstImapConf.Password, out var model))
            {
                model = new ImapClientModel(
                    firstImapConf.Login,
                    firstImapConf.Password,
                    firstImapConf.Host,
                    firstImapConf.Port,
                    firstImapConf.UseSsl);
            }

            model.ActiveFolder = folderName;

            return View("ShowMailsView", model);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult CreateMail()
        {
            MailMessageModel model = new MailMessageModel();
            return View("CreateMail");
        }

        [HttpPost]
        public async Task<IActionResult> CreateMail(MailMessageModel model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                throw new ApplicationException($"User ID was not found in user claims!");
            }

            var user = await _dbContext.Users
                .Include(appUser => appUser.SmtpConfigurations)
                .SingleOrDefaultAsync(appUser => appUser.Id == userId);

            // for now using only one configuration
            var firstSmtpConf = user.SmtpConfigurations.First();

            var sender = new MailSender(firstSmtpConf);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(user.UserName, firstSmtpConf.Username));
            message.To.Add(new MailboxAddress(model.Recipent, model.Recipent));
            message.Subject = model.Title;

            message.Body = new TextPart("html")
            {
                Text = model.Content
            };

            await sender.SendMailAsync(message);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<JsonResult> GetMessage(string folderName, int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                throw new ApplicationException($"User ID was not found in user claims!");
            }

            var user = await _dbContext.Users
                .Include(appUser => appUser.ImapConfigurations)
                .SingleOrDefaultAsync(appUser => appUser.Id == userId);

            // for now using only one configuration
            var firstImapConf = user.ImapConfigurations.First();

            if (!ImapClientModel.ImapClientModelsDictionary.TryGetValue(firstImapConf.Login + firstImapConf.Password, out var model))
            {
                model = new ImapClientModel(
                    firstImapConf.Login,
                    firstImapConf.Password,
                    firstImapConf.Host,
                    firstImapConf.Port,
                    firstImapConf.UseSsl);
            }

            string activeFolder = folderName == null ? "INBOX" : folderName;

            MimeKit.MimeMessage message = model.GetMessage(activeFolder, (uint) id);
            var messageBody = message.HtmlBody == null ? message.TextBody : message.HtmlBody;

            var data = new
            {
                Subject = message.Subject,
                From = message.From.ToString(),
                Body = messageBody
            };

            return new JsonResult(data);
        }
    }
}
