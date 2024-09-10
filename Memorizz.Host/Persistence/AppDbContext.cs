using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Memorizz.Host.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext(options);