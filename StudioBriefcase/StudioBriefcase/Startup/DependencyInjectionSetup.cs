using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore.Internal;
using System.Runtime.CompilerServices;

using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace StudioBriefcase.Startup
{
    public static class DependencyInjectionSetup
    {
        public static IServiceCollection RegisterServices(this  IServiceCollection services)
        {

            services.AddRazorPages();

            return services;
        }
        /// <summary>
        /// AddAuthentication Services to Register GithubOauth2.0
        /// </summary>
        public static void RegisterGitHubService(WebApplicationBuilder builder)
        {
            //Documentation Links
            //https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/other-logins?view=aspnetcore-5.0
            //https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/additional-claims?view=aspnetcore-5.0
            //Handles the stages of Transactions to Authenticate github OAuth Account
            //https://www.youtube.com/watch?v=PUXpfr1LzPE&t=2142s Raw Coding - Authenticating with GitHub OAuth tutorial

            builder.Services.AddAuthentication("cookie")
            .AddCookie("cookie")
            .AddOAuth("github", o =>
            {
                   o.SignInScheme = "cookie";

                   o.ClientId = builder.Configuration["GitHub-clientid"]!;
                   o.ClientSecret = builder.Configuration["GitHub-clientsecret"]!;

                   o.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
                   o.TokenEndpoint = "https://github.com/login/oauth/access_token";
                   o.CallbackPath = new PathString("/signin-github");
                   o.SaveTokens = true;
                   o.UserInformationEndpoint = "https://api.github.com/user";

                //Link for github response
                //https://docs.github.com/en/rest/users/users?apiVersion=2022-11-28#get-the-authenticated-user
                //Whats happening here is were creating a map that can hold the respective data after authentication.
                   o.ClaimActions.MapJsonKey("sub", "id");
                   o.ClaimActions.MapJsonKey(ClaimTypes.Name, "login");
                   o.ClaimActions.MapJsonKey(ClaimTypes.Uri, "avatar_url");
                   o.ClaimActions.MapJsonKey(ClaimTypes.Webpage, "html_url");


                   //This event is a function thats called once Github is Authenticated.
                   o.Events.OnCreatingTicket = async ctx =>
                   {

                       using var request = new HttpRequestMessage(HttpMethod.Get, ctx.Options.UserInformationEndpoint);

                       request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ctx.AccessToken);

                       using var result = await ctx.Backchannel.SendAsync(request);

                       var user = await result.Content.ReadFromJsonAsync<JsonElement>();
                       ctx.RunClaimActions(user);
                   };
            });
        }


    }
}
