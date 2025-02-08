using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Time_Records.DTO;
using Time_Records.Models;

namespace Time_Records.Services;

public class RecordTimeService {
    private ApplicationDbContext dbContext;
    private UserManager<AppUser> userManager;
    private IHttpContextAccessor httpContextAccessor;

    public RecordTimeService(ApplicationDbContext dbContext) {
        this.dbContext = dbContext;
    }
    public RecordTimeService(ApplicationDbContext dbContext, UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor) {
        this.dbContext = dbContext;
        this.userManager = userManager;
        this.httpContextAccessor = httpContextAccessor;
    }
    
    // public async Task<TimeFormatDto> SumTotalRecordTimeAsync() {
    //     var records = await dbContext.Records.ToListAsync();
    //     var totalTime = records.Aggregate(TimeSpan.Zero, (sum, record) => sum + record.RecordTime);
    //
    //     return new TimeFormatDto() 
    //     { 
    //         Hours = (int)totalTime.TotalHours,
    //         Minutes = totalTime.Minutes
    //     };
    // }


    public async Task<TimeFormatDto> SumTotalRecordTimeAsync() {
        var user = await userManager.GetUserAsync(httpContextAccessor.HttpContext.User);
        if (user == null) {
            throw new UnauthorizedAccessException("User not found");
        }
        var records = await dbContext.Records
            .Where(record => record.IdentityUserId == user.Id)
            .ToListAsync();
        var totalTime = records.Aggregate(TimeSpan.Zero, (sum, record) => sum + record.RecordTime);
        return new TimeFormatDto() 
        { 
            Hours = (int)totalTime.TotalHours,
            Minutes = totalTime.Minutes
        };
    }
    
    public async Task<TimeFormatDto> SumActualMinistryYearTotalRecordTimeAsync() {
        var user = await userManager.GetUserAsync(httpContextAccessor.HttpContext.User);
        if (user == null) {
            throw new UnauthorizedAccessException("User not found");
        }
        var actualYear = DateTime.Now.Year;
        var startMinistryYear = new DateOnly(actualYear - 1, 9, 1);
        var endMinistryYear = new DateOnly(actualYear, 8, 31);
        var actualYearRecords = await dbContext.Records
            .Where(record =>
                record.IdentityUserId == user.Id &&
                record.Date >= startMinistryYear &&
                record.Date <= endMinistryYear)
            .ToListAsync();
        
        var totalTime = actualYearRecords.Aggregate(TimeSpan.Zero, (sum, record) => sum + record.RecordTime);
        return new TimeFormatDto() 
        { 
            Hours = (int)totalTime.TotalHours,
            Minutes = totalTime.Minutes
        };
    }
    
    public async Task<TimeFormatDto> SumActualMinistryYearTotalRecordTimeQueryAsync([FromQuery] string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("User not found");
        }

        var actualYear = DateTime.Now.Year;
        var startMinistryYear = new DateOnly(actualYear - 1, 9, 1);
        var endMinistryYear = new DateOnly(actualYear, 8, 31);
    
        var actualYearRecords = await dbContext.Records
            .Where(record =>
                record.IdentityUserId == userId &&
                record.Date >= startMinistryYear &&
                record.Date <= endMinistryYear)
            .ToListAsync();
    
        var totalTime = actualYearRecords.Aggregate(TimeSpan.Zero, (sum, record) => sum + record.RecordTime);
        return new TimeFormatDto() 
        { 
            Hours = (int)totalTime.TotalHours,
            Minutes = totalTime.Minutes
        };
    }
    
    public async Task<double> YearRecordProgressAsync() {
        var yearTotalRecord = await SumActualMinistryYearTotalRecordTimeAsync();
        var hoursAndMinutes = yearTotalRecord.Hours + (yearTotalRecord.Minutes / 60.0);
        var yearHoursQuote = 600.0;
        var percentageYearProgress = (hoursAndMinutes * 100) / yearHoursQuote;
        return Math.Round(percentageYearProgress);
    }
    
    public async Task<double> YearRecordProgressQueryAsync([FromQuery] string userId) {
        var yearTotalRecord = await SumActualMinistryYearTotalRecordTimeQueryAsync(userId);
        var hoursAndMinutes = yearTotalRecord.Hours + (yearTotalRecord.Minutes / 60.0);
        var yearHoursQuote = 600.0;
        var percentageYearProgress = (hoursAndMinutes * 100) / yearHoursQuote;
        return Math.Round(percentageYearProgress);
    }
    
    public async Task<TimeFormatDto> YearRemainingTimeAsync() {
        var yearHoursQuote = 600.0;
        var yearTotalRecord = await SumActualMinistryYearTotalRecordTimeAsync();
        var hoursAndMinutes = yearTotalRecord.Hours + (yearTotalRecord.Minutes / 60.0);
        var remainingTime = yearHoursQuote - hoursAndMinutes;
        return new TimeFormatDto()
        { 
            Hours = (int)remainingTime,
            Minutes = (int)((remainingTime - (int)remainingTime) * 60)
        };
    }
    
    public async Task<TimeFormatDto> YearRemainingTimeQueryAsync([FromQuery] string userId) {
        if (string.IsNullOrEmpty(userId)) {
            throw new UnauthorizedAccessException("User not found");
        }

        var yearHoursQuote = 600.0;
        var yearTotalRecord = await SumActualMinistryYearTotalRecordTimeQueryAsync(userId);
        var hoursAndMinutes = yearTotalRecord.Hours + (yearTotalRecord.Minutes / 60.0);
        var remainingTime = yearHoursQuote - hoursAndMinutes;

        return new TimeFormatDto()
        { 
            Hours = (int)remainingTime,
            Minutes = (int)((remainingTime - (int)remainingTime) * 60)
        };
    }
    
    
    public async Task<TimeFormatDto> SumActualMonthTotalRecordTimeAsync() {
        var user = await userManager.GetUserAsync(httpContextAccessor.HttpContext.User);
        if (user == null) {
            throw new UnauthorizedAccessException("User not found");
        }
        var actualMonth = DateTime.Now.Month;
        var actualYear = DateTime.Now.Year;
        var actualMonthRecords = await dbContext.Records
            .Where(record =>
                record.IdentityUserId == user.Id &&
                record.Date.Year == actualYear &&
                record.Date.Month == actualMonth)
            .ToListAsync();
        
        var totalTime = actualMonthRecords.Aggregate(TimeSpan.Zero, (sum, record) => sum + record.RecordTime);
        return new TimeFormatDto() 
        { 
            Hours = (int)totalTime.TotalHours,
            Minutes = totalTime.Minutes
        };
    }
    
    public async Task<TimeFormatDto> SumActualMonthTotalRecordTimeQueryAsync([FromQuery] string userId) {
        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("User not found");
        }
        var actualMonth = DateTime.Now.Month;
        var actualYear = DateTime.Now.Year;
        var actualMonthRecords = await dbContext.Records
            .Where(record =>
                record.IdentityUserId == userId &&
                record.Date.Year == actualYear &&
                record.Date.Month == actualMonth)
            .ToListAsync();
        
        var totalTime = actualMonthRecords.Aggregate(TimeSpan.Zero, (sum, record) => sum + record.RecordTime);
        return new TimeFormatDto() 
        { 
            Hours = (int)totalTime.TotalHours,
            Minutes = totalTime.Minutes
        };
    }

    public async Task<double> MonthRecordProgressAsync() {
        var monthTotalRecord = await SumActualMonthTotalRecordTimeAsync();
        var hoursAndMinutes = monthTotalRecord.Hours + (monthTotalRecord.Minutes / 60.0);
        var percentageMonthProgress = (hoursAndMinutes * 100.0) / 50;
        return Math.Round(percentageMonthProgress);
    }
    
    public async Task<double> MonthRecordProgressQueryAsync([FromQuery] string userId) {
        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("User not found");
        }
        var monthTotalRecord = await SumActualMonthTotalRecordTimeQueryAsync(userId);
        var hoursAndMinutes = monthTotalRecord.Hours + (monthTotalRecord.Minutes / 60.0);
        var percentageMonthProgress = (hoursAndMinutes * 100.0) / 50;
        return Math.Round(percentageMonthProgress);
    }
    
    public async Task<TimeFormatDto> MonthRemainingTimeAsync() {
        var monthHoursQuote = 50.0;
        var monthTotalRecord = await SumActualMonthTotalRecordTimeAsync();
        var hoursAndMinutes = monthTotalRecord.Hours + (monthTotalRecord.Minutes / 60.0);
        var remainingTime = monthHoursQuote - hoursAndMinutes;
        return new TimeFormatDto()
        { 
            Hours = (int)remainingTime,
            Minutes = (int)((remainingTime - (int)remainingTime) * 60)
        };
    }
    
    public async Task<TimeFormatDto> MonthRemainingTimeQueryAsync([FromQuery] string userId) {
        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("User not found");
        }
        var monthHoursQuote = 50.0;
        var monthTotalRecord = await SumActualMonthTotalRecordTimeQueryAsync(userId);
        var hoursAndMinutes = monthTotalRecord.Hours + (monthTotalRecord.Minutes / 60.0);
        var remainingTime = monthHoursQuote - hoursAndMinutes;
        return new TimeFormatDto()
        { 
            Hours = (int)remainingTime,
            Minutes = (int)((remainingTime - (int)remainingTime) * 60)
        };
    }
    
    public async Task<TimeFormatDto> SumMonthTotalRecordTimeAsync(DateOnly chosenMonthYear) {
        var user = await userManager.GetUserAsync(httpContextAccessor.HttpContext.User);
        if (user == null) {
            throw new UnauthorizedAccessException("User not found");
        }
        var chosenMonthRecords = await dbContext.Records
            .Where(record =>
                record.IdentityUserId == user.Id &&
                record.Date.Year == chosenMonthYear.Year &&
                record.Date.Month == chosenMonthYear.Month)
            .ToListAsync();
        
        var totalTime = chosenMonthRecords.Aggregate(TimeSpan.Zero, (sum, record) => sum + record.RecordTime);
        return new TimeFormatDto() 
        { 
            Hours = (int)totalTime.TotalHours,
            Minutes = totalTime.Minutes
        };
    }
    
    public async Task<TimeFormatDto> SumActualWeekTotalRecordTimeAsync() {
        var user = await userManager.GetUserAsync(httpContextAccessor.HttpContext.User);
        if (user == null) {
            throw new UnauthorizedAccessException("User not found");
        }
        // Převedeme DayOfWeek na hodnotu, kde pondělí = 1, neděle = 7
        var actualDay = DateTime.Now;
        int dayOfWeek;
        if (actualDay.DayOfWeek == DayOfWeek.Sunday) {
            dayOfWeek = 7;
        } else {
            dayOfWeek = (int)actualDay.DayOfWeek;
        }
        // Vypočítáme pondělí daného týdne
        var startOfWeek = actualDay.AddDays(-(dayOfWeek - 1)).Date;
        var endOfWeek = startOfWeek.AddDays(7);
        var weekRecords = await dbContext.Records
            .Where(record =>
                record.IdentityUserId == user.Id &&
                record.Date >= DateOnly.FromDateTime(startOfWeek) &&
                record.Date < DateOnly.FromDateTime(endOfWeek))
            .ToListAsync();
        var totalTime = weekRecords.Aggregate(TimeSpan.Zero, (sum, record) => sum + record.RecordTime);
        return new TimeFormatDto() 
        { 
            Hours = (int)totalTime.TotalHours,
            Minutes = totalTime.Minutes
        };
    }
    
    public async Task<TimeFormatDto> SumActualWeekTotalRecordTimeQueryAsync([FromQuery] string userId) {
        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("User not found");
        }
        // Převedeme DayOfWeek na hodnotu, kde pondělí = 1, neděle = 7
        var actualDay = DateTime.Now;
        int dayOfWeek;
        if (actualDay.DayOfWeek == DayOfWeek.Sunday) {
            dayOfWeek = 7;
        } else {
            dayOfWeek = (int)actualDay.DayOfWeek;
        }
        // Vypočítáme pondělí daného týdne
        var startOfWeek = actualDay.AddDays(-(dayOfWeek - 1)).Date;
        var endOfWeek = startOfWeek.AddDays(7);
        var weekRecords = await dbContext.Records
            .Where(record =>
                record.IdentityUserId == userId &&
                record.Date >= DateOnly.FromDateTime(startOfWeek) &&
                record.Date < DateOnly.FromDateTime(endOfWeek))
            .ToListAsync();
        var totalTime = weekRecords.Aggregate(TimeSpan.Zero, (sum, record) => sum + record.RecordTime);
        return new TimeFormatDto() 
        { 
            Hours = (int)totalTime.TotalHours,
            Minutes = totalTime.Minutes
        };
    }

    public async Task<double> WeekRecordProgressAsync() {
        var weekTotalRecord = await SumActualWeekTotalRecordTimeAsync();
        var hoursAndMinutes = weekTotalRecord.Hours + (weekTotalRecord.Minutes / 60.0);
        var percentageWeekProgress = (hoursAndMinutes * 100.0) / 12.5;
        return Math.Round(percentageWeekProgress);
    }
    
    public async Task<double> WeekRecordProgressQueryAsync([FromQuery] string userId) {
        if (string.IsNullOrEmpty(userId)) {
            throw new UnauthorizedAccessException("User not found");
        }
        var weekTotalRecord = await SumActualWeekTotalRecordTimeQueryAsync(userId);
        var hoursAndMinutes = weekTotalRecord.Hours + (weekTotalRecord.Minutes / 60.0);
        var percentageWeekProgress = (hoursAndMinutes * 100.0) / 12.5;
        return Math.Round(percentageWeekProgress);
    }
    
    public async Task<TimeFormatDto> WeekRemainingTimeAsync() {
        var weekHoursQuote = 12.5;
        var weekTotalRecord = await SumActualWeekTotalRecordTimeAsync();
        var hoursAndMinutes = weekTotalRecord.Hours + (weekTotalRecord.Minutes / 60.0);
        var remainingTime = weekHoursQuote - hoursAndMinutes;
        return new TimeFormatDto()
        { 
            Hours = (int)remainingTime,
            Minutes = (int)((remainingTime - (int)remainingTime) * 60)
        };
    }
    
    public async Task<TimeFormatDto> WeekRemainingTimeQueryAsync([FromQuery] string userId) {
        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("User not found");
        }
        var weekHoursQuote = 12.5;
        var weekTotalRecord = await SumActualWeekTotalRecordTimeQueryAsync(userId);
        var hoursAndMinutes = weekTotalRecord.Hours + (weekTotalRecord.Minutes / 60.0);
        var remainingTime = weekHoursQuote - hoursAndMinutes;
        return new TimeFormatDto()
        { 
            Hours = (int)remainingTime,
            Minutes = (int)((remainingTime - (int)remainingTime) * 60)
        };
    }
    
    // public async Task<TimeFormatDto> SumWeekTotalRecordTimeAsync(DateOnly chosenDay) {
    //     var records = await dbContext.Records.ToListAsync();
    //     // Převedeme DayOfWeek na hodnotu, kde pondělí = 1, neděle = 7
    //     int dayOfWeek;
    //     if (chosenDay.DayOfWeek == DayOfWeek.Sunday) {
    //         dayOfWeek = 7;
    //     } else {
    //         dayOfWeek = (int)chosenDay.DayOfWeek;
    //     }
    //     // Vypočítáme pondělí daného týdne
    //     var startOfWeek = chosenDay.AddDays(1 - dayOfWeek);
    //     var endOfWeek = startOfWeek.AddDays(7);
    //
    //     var chosenWeekRecords = records
    //         .Where(record => record.Date >= startOfWeek && record.Date < endOfWeek)
    //         .Aggregate(TimeSpan.Zero, (sum, record) => sum + record.RecordTime);
    //     return new TimeFormatDto() 
    //     { 
    //         Hours = (int)chosenWeekRecords.TotalHours,
    //         Minutes = chosenWeekRecords.Minutes
    //     };
    // }
    
    public async Task<TimeFormatDto> SumWeekTotalRecordTimeAsync(DateOnly chosenDay) {
        var user = await userManager.GetUserAsync(httpContextAccessor.HttpContext.User);
        if (user == null) {
            throw new UnauthorizedAccessException("User not found");
        }
        // Převedeme DayOfWeek na hodnotu, kde pondělí = 1, neděle = 7
        int dayOfWeek;
        if (chosenDay.DayOfWeek == DayOfWeek.Sunday) {
            dayOfWeek = 7;
        } else {
            dayOfWeek = (int)chosenDay.DayOfWeek;
        }
        // Vypočítáme pondělí daného týdne
        var startOfWeek = chosenDay.AddDays(1 - dayOfWeek);
        var endOfWeek = startOfWeek.AddDays(7);
        var chosenWeekRecords = await dbContext.Records
            .Where(record =>
                record.IdentityUserId == user.Id &&
                record.Date >= startOfWeek &&
                record.Date < endOfWeek)
            .ToListAsync();
        var totalTime = chosenWeekRecords.Aggregate(TimeSpan.Zero, (sum, record) => sum + record.RecordTime);
        return new TimeFormatDto() 
        { 
            Hours = (int)totalTime.TotalHours,
            Minutes = totalTime.Minutes
        };
    }
}