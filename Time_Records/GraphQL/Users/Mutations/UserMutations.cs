using HotChocolate.Language;
using Microsoft.AspNetCore.Identity;
using Time_Records.GraphQL.Types.Inputs;
using Time_Records.GraphQL.Users.Payloads;
using Time_Records.Models;

// using Time_Records.Models;

namespace Time_Records.GraphQL.Users.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class UserMutations {
    // Administrátor vytvoří uživatele s heslem
    public async Task<AppUserPayload> CreateUserAsync(
        CreateAppUserInput input,
        [Service] UserManager<AppUser> userManager) {
        var user = new AppUser {
            UserName = input.Username,
            Email = input.Email,
            PhoneNumber = input.PhoneNumber,
            MonthTimeGoal = (input.MonthTimeGoal == null || input.MonthTimeGoal == 0) ? 15 : input.MonthTimeGoal
        };
        
        var result = await userManager.CreateAsync(user, input.Password);
        return new AppUserPayload {
            User = result.Succeeded ? user : null,
            Errors = result.Errors.Select(e => e.Description).ToList()
        };
    }
    
    // Administrátor vytvoří uživatele přes Google OAuth
    public async Task<AppUserPayload> CreateUserFromGoogleAsync(
        CreateAppUserFromGoogleInput input,
        [Service] UserManager<AppUser> userManager) {
        var user = new AppUser {
            UserName = input.UserName,
            Email = input.Email,
            PhoneNumber = input.PhoneNumber,
            GoogleId = input.GoogleId,      // Neměnný sub z Google tokenu
            MonthTimeGoal = (input.MonthTimeGoal == null || input.MonthTimeGoal == 0) ? 15 : input.MonthTimeGoal
        };
        
        var result = await userManager.CreateAsync(user);
        
        return new AppUserPayload {
            User = result.Succeeded ? user : null,
            Errors = result.Errors.Select(e => e.Description).ToList()
        };
    }
    
    // Administrátor updatuje uživatele
    public async Task<AppUserPayload> UpdateUserAsync(
        UpdateAppUserInput input,
        [Service] UserManager<AppUser> userManager) {
        var user = await userManager.FindByIdAsync(input.Id.ToString());
        if (user == null) {
            return new AppUserPayload {
                Errors = new List<string> {"User not found"}
            };
        }
        user.UserName = input.UserName;
        user.Email = input.Email;
        user.PhoneNumber = input.PhoneNumber;
        user.MonthTimeGoal = (input.MonthTimeGoal == null || input.MonthTimeGoal == 0) ? 15 : input.MonthTimeGoal;
        if (!string.IsNullOrEmpty(input.UserName)) {
            user.UserName = input.UserName;
        }
        if (!string.IsNullOrEmpty(input.Email)) {
            user.Email = input.Email;
        }
        if (!string.IsNullOrEmpty(input.PhoneNumber)) {
            user.PhoneNumber = input.PhoneNumber;
        }
        if (input.MonthTimeGoal.HasValue) {
            user.MonthTimeGoal = input.MonthTimeGoal;
        }
        var result = await userManager.UpdateAsync(user);
        return new AppUserPayload {
            User = result.Succeeded ? user : null,
            Errors = result.Errors.Select(e => e.Description).ToList()
        };
    }
    
    // Administrátor zmeni heslo uživatele
    public async Task<AppUserPayload> ChangePasswordAsync(
        ChangePasswordInput input,
        [Service] UserManager<AppUser> userManager) {
        var user = await userManager.FindByIdAsync(input.UserId.ToString());
        if (user == null) {
            return new AppUserPayload {
                Errors = new List<string> {"User not found"}
            };
        }
        var result = await userManager.ChangePasswordAsync(user, input.CurrentPassword, input.NewPassword);
        return new AppUserPayload {
            User = result.Succeeded ? user : null,
            Errors = result.Errors.Select(e => e.Description).ToList()
        };
    }
    
    // Administrátor odstraní uživatele
    // public async Task<AppUserPayload> DeleteUserAsync(
    //     DeleteAppUserInput input,
    //     [Service] UserManager<AppUser> userManager) {
    //     var user = await userManager.FindByIdAsync(input.Id.ToString());
    //     if (user == null) {
    //         return new AppUserPayload {
    //             Errors = new List<string> {"User not found"}
    //         };
    //     }
    //     var result = await userManager.DeleteAsync(user);
    //     return new AppUserPayload {
    //         User = result.Succeeded ? user : null,
    //         Errors = result.Errors.Select(e => e.Description).ToList()
    //     };
    // }
}

