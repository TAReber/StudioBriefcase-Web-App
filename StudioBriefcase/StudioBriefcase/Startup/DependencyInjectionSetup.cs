﻿using Microsoft.AspNetCore.Authentication;
//using Microsoft.EntityFrameworkCore.Internal;
using System.Runtime.CompilerServices;

using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Razor;
using StudioBriefcase.Services;
using MySqlConnector;
using StudioBriefcase.Models;


namespace StudioBriefcase.Startup
{
    public static class DependencyInjectionSetup
    {
        //TODO:: Most of these services were copied from the Microsoft Template.
        //Need to go through and remove the ones that are not needed.

        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddRazorPages();
            //Opposite of Singleton, Transient creates a new instance of the service for each request.
            services.AddTransient<MySqlConnection>(_ => 
            new MySqlConnection(configuration["database-credential"]));

            //services.AddMvc();
            services.AddTransient<IBaseService, BaseService>();

            services.AddTransient<IPostTypeService, PostTypeService>();
            services.AddTransient<ICustodianService, CustodianService>();

            //Creates a reusable instance per HTTP request
            
            services.AddScoped<LibraryService>(); // If the same viewComponent is invoked multiple times, it automatically uses a scoped instance.
            services.AddScoped<PostTypeService>();
            services.AddScoped<UserService>(); //Mainly Used with Settings Menu pages
            services.AddScoped<PageService>();

            //I Forgot I added this, now I don't know if I need it or not.
            services.AddControllersWithViews();

            services.AddHttpClient("youtube", client =>
            {
                client.BaseAddress = new Uri("https://www.googleapis.com/youtube/v3/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });
            services.AddHttpClient();
            
            
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationFormats.Add("/Pages/Shared/Components/{0}" + RazorViewEngine.ViewExtension);
                //    options.PageViewLocationFormats.Add("/Views/{0}" + RazorViewEngine.ViewExtension);
                //    //options.PageViewLocationFormats.Add("/Pages/Shared/{0}" + RazorViewEngine.ViewExtension);
            });

            


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
                //Get Section handles Development and Production Environments
                o.ClientId = builder.Configuration[builder.Configuration.GetSection("keyVault:GitHub:id").Value!.ToString()]!;
                o.ClientSecret = builder.Configuration[builder.Configuration.GetSection("keyVault:GitHub:secret").Value!.ToString()]!;

                o.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
                o.TokenEndpoint = "https://github.com/login/oauth/access_token";

                //o.CallbackPath = new PathString(callbackUri);
                o.CallbackPath = new PathString("/signin-github");
                o.SaveTokens = true;
                o.UserInformationEndpoint = "https://api.github.com/user";

                //Link for github response
                //https://docs.github.com/en/rest/users/users?apiVersion=2022-11-28#get-the-authenticated-user
                //Whats happening here is were creating a map that can hold the respective data after authentication.
                o.ClaimActions.MapJsonKey("sub", "id");
                o.ClaimActions.MapJsonKey("name", "login");
                o.ClaimActions.MapJsonKey("picture", "avatar_url");
                o.ClaimActions.MapJsonKey("webpage", "html_url");
                

                //This event is a function thats called once Github is Authenticated.
                o.Events.OnCreatingTicket = async ctx =>
                {
                    using var request = new HttpRequestMessage(HttpMethod.Get, ctx.Options.UserInformationEndpoint);

                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ctx.AccessToken);

                    using var result = await ctx.Backchannel.SendAsync(request);

                    var user = await result.Content.ReadFromJsonAsync<JsonElement>();
                    ctx.RunClaimActions(user);

                    UserModel model = new UserModel
                    {
                        Id = user.GetProperty("id").GetUInt32(),
                        Name = user.GetProperty("login").ToString(),
                        avatar_url = user.GetProperty("avatar_url").ToString(),
                        profile_url = user.GetProperty("html_url").ToString(),
                        userclass = 1, //1 = Private
                        userprivilege = 1 //2 = Admin
                    };

                    var userService = ctx.HttpContext.RequestServices.GetRequiredService<UserService>();
                    bool exists = await userService.UserExistsAsync(model.Id);
                    if (exists)
                    {
                        var userrole = await userService.GetUserClass(model.Id);
                        var userprivilege = await userService.GetUserPrivilege(model.Id);
                        ctx.Identity?.AddClaim(new Claim("class", userrole));
                        ctx.Identity?.AddClaim(new Claim("privilege", userprivilege));

                    }
                    else
                    {
                        await userService.AddUserAsync(model);
                        ctx.Identity?.AddClaim(new Claim("role", "private"));
                        ctx.Identity?.AddClaim(new Claim("privilege", "Regular"));
                    }

                };
                

            });
        }


    }
}