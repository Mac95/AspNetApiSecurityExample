using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AspNetApiSecurity.Startup))]
namespace AspNetApiSecurity
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
