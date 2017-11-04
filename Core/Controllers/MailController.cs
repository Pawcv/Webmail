using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Core.Models;

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
            return View("ImapTest", imapClientModel);
        }
    }
}
