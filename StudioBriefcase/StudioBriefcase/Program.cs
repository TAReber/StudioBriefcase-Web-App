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
var keyVaultClientID = builder.Configuration.GetSection("KeyVault:ClientId");
var keyVaultClientSecret = builder.Configuration.GetSection("KeyVault:ClientSecret");
var KeyvaultDirectoryID = builder.Configuration.GetSection("KeyVault:DirectoryID");
//Create the client's Credentials object needed to access the Azure Keyvault
var credential = new ClientSecretCredential(
    KeyvaultDirectoryID.Value!.ToString(), 
    keyVaultClientID.Value!.ToString(), 
    keyVaultClientSecret.Value!.ToString());

// The New Azure application is added to the keyvault permissions before it can read from the key vault.
builder.Configuration.AddAzureKeyVault(
    keyVaultURL.Value!.ToString(), 
    keyVaultClientID.Value!.ToString(), 
    keyVaultClientSecret.Value!.ToString(), 
    new DefaultKeyVaultSecretManager());
//Create a Client with the credentials that can retieve From the vault
var client = new SecretClient(new Uri(keyVaultURL.Value!.ToString()), credential);

string id = client.GetSecret("GitHub-clientid").Value.Value.ToString();
string secret = client.GetSecret("GitHub-clientsecret").Value.Value.ToString();
//builder.Services.AddDbContext<SampleDatabaseContext>(Options =>
//{
//    Options.UseSqlServer(client.GetSecret("ProdConnection").Value.Value.ToString());
//});


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

//builder.Configuration.AddJsonFile("appsettings.json");
//builder.Configuration.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
//optional:true, reloadOnChange: true).AddEnvironmentVariables();



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
        
        RedirectUri = "https://localhost:7166"
    },
    authenticationSchemes: new List<string>() { "github" }); ;
});

//Tutorial Example
// Output from this code Displays page with just a list which looks like []
//app.MapGet("/", (HttpContext ctx) =>
//{
//    //This line populates the[list] with the claims returns from signing in with github.
//    return ctx.User.Claims.Select(x => new { x.Type, x.Value }).ToList();
//});

//app.MapGet("/login", () =>
//{
//    return Results.Challenge(new AuthenticationProperties()
//    {
//        RedirectUri = "https://localhost:7166"
//    },
//    authenticationSchemes: new List<string>() { "github" });
//});

app.Run();
