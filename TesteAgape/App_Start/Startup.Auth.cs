using System;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Owin;

namespace TesteAgape
{
    public partial class Startup
    {
        //public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        static Startup()
        {
            //OAuthOptions = new OAuthAuthorizationServerOptions
            //{
            //    TokenEndpointPath = new PathString("/token"),
            //    Provider = new OAuthAppProvider(),
            //    AccessTokenExpireTimeSpan = TimeSpan.FromDays(2),
            //    AllowInsecureHttp = true,
            //    AccessTokenProvider = new AccessTokenProvider()
            //};
        }
        public void ConfigureAuth(IAppBuilder app)
        {
            //app.UseOAuthBearerTokens(OAuthOptions);
        }
    }
}