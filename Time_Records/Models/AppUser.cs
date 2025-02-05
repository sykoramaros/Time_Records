using Microsoft.AspNetCore.Identity;

namespace Time_Records.Models;

public class AppUser : IdentityUser {  // dedi z IdentityUser spoustu vlastnosti
    // moznost pridani propojeni s jinou tabulkou (department, ...)
}