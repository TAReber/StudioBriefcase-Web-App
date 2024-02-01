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
    builder.Configuration.GetSection("keyVault:database:credential").Value!.ToString(),
    builder.Configuration.GetSection("keyVault:api:youtube").Value!.ToString()
};

bool isVerified = builder.Configuration.VerifyKeyVaultSecrets(keyVaultURL, targetNames);

builder.Services.RegisterServices(builder.Configuration);
if (isVerified)
    DependencyInjectionSetup.RegisterGitHubService(builder);

var app = builder.Build();

// Configure the http request pipeline
if (!app.Environment.IsDevelopment())
{
    //app.UseExceptionHandler("/Pages/Error");
    //TODO:: HSTS value is 30 days, may want to change. https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapMVCRoutes(); //defined in MapEndpoints.cs

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

if (isVerified)
    app.MapLoginEndPoints();

app.Run();