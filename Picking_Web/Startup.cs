using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Picking_Web.Startup))]
namespace Picking_Web
{
    public partial class Startup
    {
        public static string _logFile = "";
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
