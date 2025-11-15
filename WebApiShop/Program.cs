using Repositories;
using Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IUserRipository, UserRipository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserPasswordRipository, UserPasswordRipository>();
builder.Services.AddScoped<IUserPasswordService, UserPasswordService>();

builder.Services.AddControllers();



var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
