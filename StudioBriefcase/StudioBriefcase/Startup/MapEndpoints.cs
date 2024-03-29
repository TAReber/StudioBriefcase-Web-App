﻿using Microsoft.AspNetCore.Authentication;

namespace StudioBriefcase.Startup
{
    public static class MapEndpoints
    {
        public static WebApplication MapUserEndpoints(this WebApplication app)
        {
            //Examples
            //app.MapGet(pattern: "/User", () => "Hello User");
            //app.MapGet(pattern: "/User/{name}", (string name) => $"Hello {name}");

            return app;
        }

        public static WebApplication MapMVCRoutes(this WebApplication app)
        {
            //This Route maps directly to the tables I created in my database.
            app.MapControllerRoute(
                name: "library",
                pattern: "Library/{Category}/{Libary}/{subject}/{topic}",
                defaults: new { controller = "LibraryLink", action = "Index" });

            //Route Deprecated and Worthless? is used for User Controller 
            //app.MapControllerRoute(
            //    name: "api",
            //    pattern: "api/{controller}/{action}/{id?}");

            return app;
        }

        // Dynamic Redirect returnUrl Episode 2 
        public static WebApplication MapLoginEndPoints(this WebApplication app)
        {
            //System.IO.Path.GetDirectoryName(Context.Request.Path)

            //TODO::??::Verify GitHub OAuth is a added Service
            app.MapGet("/logout", async (HttpContext ctx) =>
            {
                var returnUri = ctx.Request.Query["returnUrl"].FirstOrDefault() ?? "/";

                await ctx.SignOutAsync("cookie");
                ctx.Response.Redirect(returnUri);
            });

            
            app.MapGet("/login", (HttpContext ctx) =>
            {
                var returnUri = ctx.Request.Query["returnUrl"].FirstOrDefault() ?? "/";

                return Results.Challenge(new AuthenticationProperties()
                {
                    RedirectUri = returnUri
                },
                authenticationSchemes: new List<string>() { "github" });
            });

            return app;
        }
    }
}