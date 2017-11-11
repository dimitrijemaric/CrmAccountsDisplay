using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AccountInfo.Startup))]
namespace AccountInfo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
           

        }
    }
}
