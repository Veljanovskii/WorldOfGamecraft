using AccountService.Data;
using AccountService.Extensions;
using AccountService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AccountDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("AccountServiceDatabase")));
builder.Services.AddScoped<IAccountService, AccountService.Services.AccountService>();

var app = builder.Build();

app.ApplyMigrations();
app.SeedData();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
