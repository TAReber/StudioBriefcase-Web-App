// .NET Core 7.0 uses Minimal API
// This is the new Startup.cs file

using MySqlConnector;

using StudioBriefcase.Startup;

//https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/handle-errrors?view=aspnetcore-7.0
var builder = WebApplication.CreateBuilder(args);

// Create a list of names that exist in the key vault so I can iterate over and verify each secret is accessible.
// Console Window will display exception errors.
string keyVaultURL = builder.Configuration.GetSection("KeyVault:KeyVaultURL").Value!.ToString();
List<string> targetNames = new List<string>
{
    builder.Configuration.GetSection("keyVault:GitHub:id").Value!.ToString(),
    builder.Configuration.GetSection("keyVault:GitHub:secret").Value!.ToString(),
    builder.Configuration.GetSection("keyVault:Database:server").Value!.ToString(),
    builder.Configuration.GetSection("keyVault:Database:name").Value!.ToString(),
    builder.Configuration.GetSection("keyVault:Database:user").Value!.ToString(),
    builder.Configuration.GetSection("keyVault:Database:password").Value!.ToString(),
};
bool isVerified = builder.Configuration.VerifyKeyVaultSecrets(keyVaultURL, targetNames);

builder.Services.RegisterServices();
//if (isVerified)
    //DependencyInjectionSetup.RegisterGitHubService(builder);


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

//if(isVerified)
    //app.MapLoginEndPoints();

//Connect to Database Example
//var database = new MySqlConnectionStringBuilder()
//{
//    Server = builder.Configuration["database-library-server"],
//    Database = builder.Configuration["database-library-database"],
//    UserID = builder.Configuration["database-library-user"],
//    Password = builder.Configuration["database-library-password"],
//    SslMode = MySqlSslMode.Required,
//};

//using (var connection = new MySqlConnection(database.ConnectionString))
//{
//    await connection.OpenAsync();
//    Console.WriteLine("Opened Connection");
//}

    app.Run();

