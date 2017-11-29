using Core.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Webmail.Smtp;
using NLog;

namespace Core.Data
{
    public class DatabaseInitializer
    {
        private const string TestAdminEmail = "admin@admin.com";
        private const string TestAdminPass = "Admin1!";

        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public DatabaseInitializer(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public void Initialize()
        {
            _dbContext.Database.Migrate();
        }

        public void Initialize(bool addAdmin)
        {
            Initialize();
            if (!addAdmin) return;
            AddTestAdminAsync().Wait();
            SeedAsync().Wait();
        }

        private async Task AddTestAdminAsync()
        {
            try
            {
                var testAdmin = await this._userManager.FindByNameAsync(TestAdminEmail);

                if (testAdmin == null)
                {
                    var user = new ApplicationUser { UserName = TestAdminEmail, Email = TestAdminEmail, EmailConfirmed = true };

                    var userResult = await this._userManager.CreateAsync(user, TestAdminPass);

                    if (!userResult.Succeeded)
                    {
                        throw new InvalidOperationException("Failed to add first admin!");
                    }

                    Logger.Info($"TestAdmin succesffully added!");
                }
                else
                {
                    Logger.Info($"TestAdmin already exists in DB.");
                }
            }
            catch (Exception e)
            {
                Logger.Warn($"Adding TestAdmin failed! Reason: {e}");
            }
        }

        /// <summary>
        /// Seeding database with some test data. Feel free to add something more.
        /// </summary>
        private async Task SeedAsync()
        {
            try
            {
                var isSeeded = await this._dbContext.SmtpConfigurations.AnyAsync();
                if (isSeeded)
                {
                    return;
                }

                var testAdmin = await this._dbContext.Users.SingleOrDefaultAsync(user => user.Email == TestAdminEmail);
                if (testAdmin == null)
                {
                    Logger.Warn("TestAdmin not in database, won't seed SmtpConf for him.");
                    return;
                }

                #region SmptConf
                var smtpConfigurations = new List<SmtpConfiguration>
                {
                    new SmtpConfiguration {
                        Username ="asdf-67@wp.pl",
                        Password = "asdF123$",
                        Host = "smtp.wp.pl",
                        Port = 465,
                        SecureSocketOptions = MailKit.Security.SecureSocketOptions.Auto
                    }
                };

                foreach (var smtpConfiguration in smtpConfigurations)
                {
                    testAdmin.SmtpConfigurations.Add(smtpConfiguration);
                }

                await this._dbContext.SaveChangesAsync();
                Logger.Info($"Added SMTP configurations!");
                #endregion

                #region ImapConf
                var imapConfigurations = new List<ImapConfiguration>
                {
                    new ImapConfiguration {
                        Login ="asdf-67@wp.pl",
                        Password = "asdF123$",
                        Host = "imap.wp.pl",
                        Port = 993,
                        UseSsl = true
                    }
                };

                foreach (var imapConfiguration in imapConfigurations)
                {
                    testAdmin.ImapConfigurations.Add(imapConfiguration);
                }

                await this._dbContext.SaveChangesAsync();
                Logger.Info($"Added IMAP configurations!");
                #endregion
            }
            catch (Exception e)
            {
                Logger.Warn($"Seeding database failed! Reason: {e}");
            }
        }
    }
}
