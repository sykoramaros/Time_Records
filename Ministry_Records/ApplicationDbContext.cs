using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ministry_Records.Models;

namespace Ministry_Records;

public class ApplicationDbContext : IdentityDbContext<AppUser> { // Dědění z IdentityDbContext pro práci s Identity
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
}