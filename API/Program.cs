using API.Data;
using API.Data.Repositories.User;
using API.Entities;
using API.Interfaces.Users;
using API.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = " Mingle Api",
        Version = "v1",
        Description = "MIngle Api is a dating software  application.",
        Contact = new OpenApiContact
        {
            Name = "Anene Grace",
            Email = "anenelucy0@gmail.com"
        }
    });
});
//identity
builder.Services.AddIdentityCore<AppUser>(options => options.User.RequireUniqueEmail =true)
.AddRoles<AppRole>()
.AddRoleManager<RoleManager<AppRole>>()
.AddSignInManager<SignInManager<AppUser>>()
.AddRoleValidator<RoleValidator<AppRole>>()
.AddEntityFrameworkStores<MingleDbContext>();


builder.Services.AddAuthentication();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MingleDbContext>(options =>options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Mingle Api Doc v1"));
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.MapControllers();

//Using keyword disposes this sevice scope after use 
using var scope =app.Services.CreateScope();
var services = scope.ServiceProvider;

var userManager =services.GetRequiredService<UserManager<AppUser>>();
var roleManager =services.GetRequiredService<RoleManager<AppRole>>();

await SeedData.SeedUsersAsync(userManager, roleManager);

await app.RunAsync();
