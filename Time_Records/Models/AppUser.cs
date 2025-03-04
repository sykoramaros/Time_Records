using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Time_Records.Models;

public class AppUser : IdentityUser<Guid> { // dedi z IdentityUser spoustu vlastnosti
    
    public string? GoogleToken { get; set; }
    
    [Range(1, int.MaxValue, ErrorMessage = "MonthTimeGoal must be at least 1")]
    public int? MonthTimeGoal { get; set; }
}