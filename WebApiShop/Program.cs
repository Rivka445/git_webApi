using Microsoft.EntityFrameworkCore;
using Repositories;
using Services;
using WebApiShop;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IUserRipository, UserRipository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserPasswordRipository, UserPasswordRipository>();
builder.Services.AddScoped<IUserPasswordService, UserPasswordService>();
builder.Services.AddDbContext<WebApiShopContext>(options => options.UseSqlServer
("Data Source=DESKTOP-1VUANBN; Initial Catalog=WebApiShop;Integrated Security=True; Pooling=False"));
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
