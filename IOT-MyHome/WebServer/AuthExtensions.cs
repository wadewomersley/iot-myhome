namespace IOT_MyHome.WebServer
{   
    using Microsoft.AspNetCore.Authentication;
    using System;

    internal static class CustomAuthExtensions
    {
        public static AuthenticationBuilder AddCustomAuth(this AuthenticationBuilder builder, Action<CustomAuthOptions> configureOptions)
        {
            return builder.AddScheme<CustomAuthOptions, AuthHandler>("Basic Auth", "Basic Auth", configureOptions);
        }
    }
}
