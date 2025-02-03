using Microsoft.AspNetCore.Identity;
namespace Ministry_Records.Models;

public class RoleMembersModel {
    
    public IdentityRole Role { get; set; }
    public IEnumerable<AppUser> Members { get; set; }
    public IEnumerable<AppUser> NonMembers { get; set; }
}