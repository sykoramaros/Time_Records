using Time_Records.DTO;
using Time_Records.Models;

namespace Time_Records.GraphQL.Users.Mapper;

public static class AppUserMapper {
    public static AppUserDto ToAppUserDto(AppUser appUser) => new AppUserDto {
        GoogleId = appUser.GoogleId,
        UserName = appUser.UserName,
        Email = appUser.Email,
        PhoneNumber = appUser.PhoneNumber,
        MonthTimeGoal = appUser.MonthTimeGoal
    };
}