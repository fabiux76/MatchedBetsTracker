using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MatchedBetsTracker.Startup))]
namespace MatchedBetsTracker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
