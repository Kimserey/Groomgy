using Kimserey.Rating.Web.EmailModels;
using Mandrill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimserey.Rating.Web.Services
{
    public interface IEmailService
    {
        Task SendConfirmationEmail(string sentToEmail, string token);
        Task SendResetPasswordEmail(string sentToEmail, string token);
        Task SendVoteReceived(string sentToEmail, Guid userId, string votedByUserName);
        Task SendNewUserEmail(Guid userId, string newUserEmail);
        Task SendWeeklyEmail(IEnumerable<GroomgyWeeklyEmail> models);
    }
}
