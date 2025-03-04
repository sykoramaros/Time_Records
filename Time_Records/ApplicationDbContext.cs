using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Time_Records.Models;

namespace Time_Records;

public class ApplicationDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid> { // Dědění z IdentityDbContext pro práci s Identity
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    
    public DbSet<Record> Records { get; set; }
}