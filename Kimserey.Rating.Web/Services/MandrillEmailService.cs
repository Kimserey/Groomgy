using Mandrill;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Kimserey.Rating.Web.EmailModels;

namespace Kimserey.Rating.Web.Services
{
    public class MandrillEmailService : IEmailService
    {
        private IMandrillApi _mandrillApi;

        public MandrillEmailService(IMandrillApi mandrillApi)
        {
            _mandrillApi = mandrillApi;
        }

        public Task SendConfirmationEmail(string sentToEmail, string token)
        {
            var email = this.InitEmail(new EmailAddress
            {
                email = sentToEmail,
                name = sentToEmail
            });

            var uriBuilder = new UriBuilder(Consts.SiteUrl);
            uriBuilder.Path = "/account/verify";

            var values = HttpUtility.ParseQueryString(string.Empty);
            values["email"] = sentToEmail;
            values["token"] = token;
            uriBuilder.Query = values.ToString();

            var callbackUrl = uriBuilder.ToString();
            email.subject = "Verify your email";
            email.AddGlobalVariable("VERIFY_EMAIL_LINK", callbackUrl);
            this.AddUnsubLinkToEmail(sentToEmail, email);
            return this.SendEmail("verify_email", email);
        }

        public Task SendResetPasswordEmail(string sentToEmail, string token)
        {
            var email = this.InitEmail(new EmailAddress
            {
                email = sentToEmail,
                name = sentToEmail
            });

            var uriBuilder = new UriBuilder(Consts.SiteUrl);
            uriBuilder.Path = "/account/resetpassword";

            var values = HttpUtility.ParseQueryString(string.Empty);
            values["email"] = sentToEmail;
            values["token"] = token;
            uriBuilder.Query = values.ToString();

            var callbackUrl = uriBuilder.ToString();
            email.subject = "Reset your password";
            email.AddGlobalVariable("RESET_PASSWORD_LINK", callbackUrl);
            this.AddUnsubLinkToEmail(sentToEmail, email);
            return this.SendEmail("reset_password", email);
        }

        public Task SendVoteReceived(string sentToEmail, Guid userId, string votedByUserName)
        {
            var email = this.InitEmail(new EmailAddress
            {
                email = sentToEmail,
                name = sentToEmail
            });

            var profileUriBuilder = new UriBuilder(Consts.SiteUrl);
            profileUriBuilder.Path = "/profile/" + userId;


            email.subject = "You received a vote!";
            email.AddGlobalVariable("VOTED_BY_USER", votedByUserName);
            email.AddGlobalVariable("USER_PROFILE", profileUriBuilder.Uri.ToString());
            this.AddUnsubLinkToEmail(sentToEmail, email);
            return this.SendEmail("vote_received", email);
        }

        public Task SendNewUserEmail(Guid userId, string newUserEmail)
        {
            var email = this.InitEmail(new EmailAddress
            {
                email = "kimserey.lam@groomgy.com",
                name = "kimserey.lam@groomgy.com"
            });

            var profileUriBuilder = new UriBuilder(Consts.SiteUrl);
            profileUriBuilder.Path = "/profile/" + userId;

            email.subject = "New user on Groomgy!";
            email.AddGlobalVariable("USER_EMAIL", newUserEmail);
            email.AddGlobalVariable("USER_PROFILE", profileUriBuilder.Uri.ToString());

            return this.SendEmail("new_user", email);
        }

        public Task SendWeeklyEmail(IEnumerable<GroomgyWeeklyEmail> models)
        {
            var modelList = models.ToList();

            return Task.WhenAll(modelList.Select(model =>
            {
                var email = this.InitEmail(new EmailAddress
                {
                    email = model.Email,
                    name = model.User
                });
                email.subject = "You have pending notifications";
                email.merge_language = "handlebars";
                email.AddRecipientVariable(model.Email, "user", model.User);
                email.AddRecipientVariable(model.Email, "conversations", model.Conversations);
                model.Members
                    .Take(4)
                    .Select((member, index) => new { member = member, index = index })
                    .ToList().ForEach(_ =>
                    {
                        email.AddRecipientVariable(model.Email, "memberimage" + _.index, _.member.Image);
                        email.AddRecipientVariable(model.Email, "memberurl" + _.index, _.member.Url);
                    });
                return this.SendEmail("groomgy_weekly", email);
            }).ToArray());
        }

        private void AddUnsubLinkToEmail(string unsubEmail, EmailMessage email)
        {
            var unsubUriBuilder = new UriBuilder(Consts.SiteUrl);
            unsubUriBuilder.Path = "/settings";

            var values = HttpUtility.ParseQueryString(string.Empty);
            values["email"] = unsubEmail;
            unsubUriBuilder.Query = values.ToString();

            email.AddGlobalVariable("UNSUB_LINK", unsubUriBuilder.Uri.ToString());
        }

        private EmailMessage InitEmail(params EmailAddress[] recipients)
        {
            var companyName = ConfigurationManager.AppSettings["CompanyName"];
            EmailMessage message = new EmailMessage
            {
                to = recipients,
                from_email = "donotreply@groomgy.com",
                from_name = companyName
            };

            return message;
        }

        private Task SendEmail(string templateName, EmailMessage message)
        {
            return _mandrillApi.SendMessageAsync(message, templateName, null, null);
        }
    }
}
