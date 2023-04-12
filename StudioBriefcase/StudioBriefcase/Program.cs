var builder = WebApplication.CreateBuilder(args);
//Add services to the contianer

builder.Services.AddRazorPages();

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
app.UseAuthorization();
app.MapRazorPages();

app.Run();
