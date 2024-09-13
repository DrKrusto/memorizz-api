using Memorizz.Host.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Memorizz.Host.Domain.Configuration;

var builder = WebApplication.CreateBuilder(args);

#region Add services to the container

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

builder.Services.AddCustomSwaggerGen()
    .AddServices()
    .AddGlobalExceptionHandler()
    .AddFluentValidation()
    .AddRequestContextBehavior();

#endregion

var app = builder.Build();

#region Add middleware to the pipeline

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGroup("/identity")
    .MapIdentityApi<IdentityUser>()
    .WithTags("Identity");
app.UseExceptionHandler();

#endregion

app.Run();