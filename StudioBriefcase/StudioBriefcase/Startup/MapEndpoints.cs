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

            app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

            return app;
        }

        public static WebApplication MapLoginEndPoints(this WebApplication app)
        {
            //TODO::??::Verify GitHub OAuth is a added Service
            app.MapGet("/logout", async (HttpContext ctx) =>
            {
                await ctx.SignOutAsync("cookie");
                ctx.Response.Redirect("/");
            });

            app.MapGet("/login", (HttpContext ctx) =>
            {
                return Results.Challenge(new AuthenticationProperties()
                {
                    RedirectUri = "/"
                },
                authenticationSchemes: new List<string>() { "github" });
            });

            return app;
        }
    }
}