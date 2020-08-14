using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TesteAgape.Site.Startup))]
namespace TesteAgape.Site
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
