using ELMS.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var ConnectionString = builder.Configuration.GetConnectionString("ELMSDB");

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<ELMSDbContext>(options =>
    options.UseSqlServer(ConnectionString));

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddAuthentication()
     .AddCookie(m =>
     {
         m.LoginPath = "/account/login";
     });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
