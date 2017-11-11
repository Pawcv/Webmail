using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Core.Models;
using MimeKit;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Core.Controllers
{
    public class MailController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
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
            return View("ImapTest", imapClientModel);
        }

        [HttpGet]
        public IActionResult CreateMail()
        {
            MailMessageModel model = new MailMessageModel();
            return View("CreateMail");
        }

        [HttpPost]
        public IActionResult CreateMail(MailMessageModel model)
        {
            var message = new MimeMessage();
            // Trzeba by pobrać nadawcę z bazy?
            message.From.Add(new MailboxAddress("Testowy nadawca", "test@test.com"));
            message.To.Add(new MailboxAddress(model.Recipent, model.Recipent));
            message.Subject = model.Title;

            message.Body = new TextPart("html")
            {
                Text = model.Content
            };


            return RedirectToAction("Index");
        }
    }
}
