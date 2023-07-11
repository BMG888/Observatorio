using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Observatorio.Startup))]
namespace Observatorio
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
