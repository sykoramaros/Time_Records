using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Time_Records.DTO;
using Time_Records.Models;

namespace Time_Records.Services;

public class RecordCreditTimeService {
    private readonly ApplicationDbContext dbContext;
    private UserManager<AppUser> userManager;
    
    public RecordCreditTimeService(ApplicationDbContext dbContext, UserManager<AppUser> userManager) {
        this.dbContext = dbContext;
        this.userManager = userManager;
    }
    
    public async Task<TimeFormatDto> SumActualMinistryYearTotalRecordCreditTimeQueryAsync([FromQuery] Guid userId) {
        if (userId == Guid.Empty) {
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
        var totalTime = actualYearRecords
            .Where(record => record.RecordCreditTime.HasValue)
            .Aggregate(TimeSpan.Zero, (sum, record) => sum + record.RecordCreditTime.Value);
        
        return new TimeFormatDto() {
            Hours = (int)totalTime.TotalHours,
            Minutes = totalTime.Minutes
        };
    }
    
    public async Task<TimeFormatDto> SumActualMonthTotalRecordCreditTimeQueryAsync([FromQuery] Guid userId) {
        if (userId == Guid.Empty) {
            throw new UnauthorizedAccessException("User not found");
        }
        var monthRecords = await dbContext.Records
            .Where(record =>
                record.IdentityUserId == userId &&
                record.Date.Month == DateTime.Now.Month &&
                record.Date.Year == DateTime.Now.Year)
            .ToListAsync();
        var totalTime = monthRecords
            .Where(record => record.RecordCreditTime.HasValue)
            .Aggregate(TimeSpan.Zero, (sum, record) => sum + record.RecordCreditTime.Value);
        
        return new TimeFormatDto() {
            Hours = (int)totalTime.TotalHours,
            Minutes = totalTime.Minutes
        };
    }
    
    public async Task<TimeFormatDto> SumActualWeekTotalRecordCreditTimeQueryAsync([FromQuery] Guid userId) {
        if (userId == Guid.Empty) {
            throw new UnauthorizedAccessException("User not found");
        }
        var weekRecords = await dbContext.Records
            .Where(record =>
                record.IdentityUserId == userId &&
                record.Date >= DateOnly.FromDateTime(DateTime.Now) &&
                record.Date < DateOnly.FromDateTime(DateTime.Now.AddDays(7)))
            .ToListAsync();
        var totalTime = weekRecords
            .Where(record => record.RecordCreditTime.HasValue)
            .Aggregate(TimeSpan.Zero, (sum, record) => sum + record.RecordCreditTime.Value);
        
        return new TimeFormatDto() {
            Hours = (int)totalTime.TotalHours,
            Minutes = totalTime.Minutes
        };
    }
    
    public async Task<TimeFormatDto> SumChosenMonthTotalRecordCreditTimeQueryAsync([FromQuery] Guid userId,
        [FromQuery] int chosenMonth, [FromQuery] int chosenYear) {
        if (userId == Guid.Empty) {
            throw new UnauthorizedAccessException("User not found");
        }
        var chosenMonthRecords = await dbContext.Records
            .Where(record =>
                record.IdentityUserId == userId &&
                record.Date.Year == chosenYear &&
                record.Date.Month == chosenMonth)
            .ToListAsync();
        var totalTime = chosenMonthRecords
            .Where(record => record.RecordCreditTime.HasValue)
            .Aggregate(TimeSpan.Zero, (sum, record) => sum + record.RecordCreditTime.Value);
        
        return new TimeFormatDto() {
            Hours = (int)totalTime.TotalHours,
            Minutes = totalTime.Minutes
        };
    }
}