// .NET Core 7.0 uses Minimal API
// This is the new Startup.cs file

using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;

var builder = WebApplication.CreateBuilder(args);

//https://www.youtube.com/watch?v=ZXfuxisC0IA //Israel Quiroz - How to use Azure tutorial
// Managed Identities
//TODO: Figure out what to do with production and development code
// KeyVault ID and Secrets belong to an newly registered application in azure
var keyVaultURL = builder.Configuration.GetSection("KeyVault:KeyVaultURL");
var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(new AzureServiceTokenProvider().KeyVaultTokenCallback));

// The New Azure application is added to the keyvault permissions before it can read from the key vault.
builder.Configuration.AddAzureKeyVault(
    keyVaultURL.Value!.ToString(), 
    new DefaultKeyVaultSecretManager());
//Create a Client with the credentials that can retieve From the vault
var client = new SecretClient(new Uri(keyVaultURL.Value!.ToString()), new DefaultAzureCredential());

string id = client.GetSecret("GitHub-clientid").Value.Value.ToString();
string secret = client.GetSecret("GitHub-clientsecret").Value.Value.ToString();


builder.Services.AddRazorPages();
//Handles the stages of Transactions to Authenticate github OAuth Account
//https://www.youtube.com/watch?v=PUXpfr1LzPE&t=2142s Raw Coding - Authenticating with GitHub OAuth tutorial
builder.Services.AddAuthentication("cookie")
    .AddCookie("cookie")
    .AddOAuth("github", o =>
{
    o.SignInScheme = "cookie";
    o.ClientId = id;
    o.ClientSecret = secret;

    o.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
    o.TokenEndpoint = "https://github.com/login/oauth/access_token";
    o.CallbackPath = new PathString("/signin-github");
    o.SaveTokens = true;
    o.UserInformationEndpoint = "https://api.github.com/user";

    o.ClaimActions.MapJsonKey("sub", "id");
    o.ClaimActions.MapJsonKey(ClaimTypes.Name, "login");

    o.Events.OnCreatingTicket = async ctx =>
    {
        
        using var request = new HttpRequestMessage(HttpMethod.Get, ctx.Options.UserInformationEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ctx.AccessToken);
        using var result = await ctx.Backchannel.SendAsync(request);
        
        var user = await result.Content.ReadFromJsonAsync<JsonElement>();
        ctx.RunClaimActions(user);
    };
});

var app = builder.Build();

// Configure the http request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Pages/Error");
    //TODO:: HSTS value is 30 days, may want to change. https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

app.MapGet("/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync("cookie");
    ctx.Response.Redirect("/");
});

app.MapGet("/login", (HttpContext ctx) =>
{
    return Results.Challenge(new AuthenticationProperties()
    {
        
        RedirectUri = "/signin-github"
    },
    authenticationSchemes: new List<string>() { "github" }); ;
});

app.Run();
