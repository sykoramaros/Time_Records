using Microsoft.AspNetCore.Identity;

namespace Time_Records.Models;

public class AppUser : IdentityUser {  // dedi z IdentityUser spoustu vlastnosti
    public int? MonthTimeGoal { get; set; }
}