using Microsoft.Owin;
using Owin;
using System;
using System.Threading.Tasks;

[assembly: OwinStartupAttribute(typeof(Kimserey.Rating.Web.Startup))]
namespace Kimserey.Rating.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            ConfigureSignalR(app);
            ConfigureLoaderio(app);
        }

        private void ConfigureLoaderio(IAppBuilder app)
        {
            app.Use((ctx, next) =>
            {
                if (ctx.Request.Path.HasValue && ctx.Request.Path.Value == "/loaderio-2178e65be66f8d3681723ef5f4b586f9/")
                {
                    ctx.Response.StatusCode = 200;
                    ctx.Response.Context.Response.Write("loaderio-2178e65be66f8d3681723ef5f4b586f9");
                    return Task.FromResult<object>(null);
                }
                return next();
            });
        }
    }
}
