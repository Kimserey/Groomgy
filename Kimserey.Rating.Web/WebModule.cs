using Kimserey.Rating.Web.Hubs;
using Kimserey.Rating.Web.Hubs.ClientInterfaces;
using Kimserey.Rating.Web.Migrations;
using Kimserey.Rating.Web.Persistence;
using Kimserey.Rating.Web.Providers;
using Kimserey.Rating.Web.Services;
using Mandrill;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimserey.Rating.Web
{
    public class WebModule: NinjectModule
    {
        public override void Load()
        {
            this.Kernel.Bind<IRepository>()
                .To<Repository>();

            this.BindServices();

            this.BindSignalRServices();

            this.BindEmailService();
        }

        private void BindEmailService()
        {
            this.Kernel.Bind<IMandrillApi>()
                .ToMethod(ctx => new MandrillApi(ConfigurationManager.AppSettings["MandrillApiKey"]));

            this.Kernel.Bind<IEmailService>()
                .To<MandrillEmailService>();
        }

        private void BindSignalRServices()
        {
            this.Kernel.Bind<IUserIdProvider>()
                .To<UserIdProvider>();

            this.Kernel.Bind<IHubContext<IConversationHubClient>>()
                .ToMethod(x => GlobalHost.DependencyResolver.Resolve<IConnectionManager>()
                   .GetHubContext<ConversationHub, IConversationHubClient>());

            this.Kernel.Bind<IHubContext<IProfileHubClient>>()
                .ToMethod(x => GlobalHost.DependencyResolver.Resolve<IConnectionManager>()
                   .GetHubContext<ProfileHub, IProfileHubClient>());

            this.Kernel.Bind<IHubContext<IVoteHubClient>>()
                .ToMethod(x => GlobalHost.DependencyResolver.Resolve<IConnectionManager>()
                   .GetHubContext<VoteHub, IVoteHubClient>());
        }

        private void BindServices()
        {
            this.Kernel.Bind<IFileStorageService>()
                .To<AzureFileStorageService>();

            this.Kernel.Bind<IUserService>()
                .To<UserService>();

            this.Kernel.Bind<IVoteService>()
                .To<VoteService>();

            this.Kernel.Bind<IConversationService>()
                .To<ConversationService>();

            this.Kernel.Bind<IOnlineUserService>()
                .To<OnlineUserService>();
        }
    }
}
