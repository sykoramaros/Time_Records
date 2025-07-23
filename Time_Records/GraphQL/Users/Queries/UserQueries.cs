using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Time_Records.DTO;
using Time_Records.GraphQL.Users.Mapper;
using Time_Records.Models;

namespace Time_Records.GraphQL.Users.Queries;

[ExtendObjectType(typeof(Query))]
public class UserQueries {
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<AppUserDto> GetUsers(
        [Service] UserManager<AppUser> userManager) 
        => userManager.Users.Select(user => AppUserMapper.ToAppUserDto(user));
    
    public async Task<AppUserDto?> GetUserByIdAsync(
        Guid id,
        [Service] UserManager<AppUserDto> userManager) 
        => await userManager.FindByIdAsync(id.ToString());
    
    public async Task<AppUserDto?> GetCurrentUserAsync(
        ClaimsPrincipal claimsPrincipal,
        [Service] UserManager<AppUserDto> userManager) 
        => await userManager.GetUserAsync(claimsPrincipal);
}

// [ExtendObjectType(typeof(Query))]
// public class UserQueries {
//     [UseProjection]
//     [UseFiltering]
//     [UseSorting]
//     public IQueryable<AppUser> GetUsers([Service] UserManager<AppUser> userManager) 
//         => userManager.Users;
//     
//     public async Task<AppUser?> GetUserByIdAsync(
//         Guid id,
//         [Service] UserManager<AppUser> userManager) 
//         => await userManager.FindByIdAsync(id.ToString());
//     
//     public async Task<AppUser?> GetCurrentUserAsync(
//         ClaimsPrincipal claimsPrincipal,
//         [Service] UserManager<AppUser> userManager) 
//         => await userManager.GetUserAsync(claimsPrincipal);
// }