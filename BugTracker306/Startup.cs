using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BugTracker306.Startup))]
namespace BugTracker306
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
