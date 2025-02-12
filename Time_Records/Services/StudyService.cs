using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Time_Records.Services;

public class StudyService {
    readonly ApplicationDbContext dbContext;
    
    public StudyService(ApplicationDbContext dbContext) {
        this.dbContext = dbContext;
    }

    public async Task<int> SumActualMinistryYearRecordStudyQuery([FromQuery] string userId) {
        if (string.IsNullOrEmpty(userId)) {
            throw new UnauthorizedAccessException("User not found");
        }
        int actualYear = (int)DateTime.Now.Year;
        DateOnly startMinistryYear;
        DateOnly endMinistryYear;
        if (DateTime.Now.Month <= 8) {
            startMinistryYear = new DateOnly(actualYear - 1, 9, 1);
            endMinistryYear = new DateOnly(actualYear, 8, 31);
        } else {
            startMinistryYear = new DateOnly(actualYear, 9, 1);
            endMinistryYear = new DateOnly(actualYear + 1, 8, 31);
        }

        var actualMinstryYearStudies = await dbContext.Records
            .Where(record => record.IdentityUserId == userId &&
                             record.Date >= startMinistryYear &&
                             record.Date <= endMinistryYear)
            .SumAsync(record => record.RecordStudy ?? 0);
        return actualMinstryYearStudies;
    }

    public async Task<int> SumActualMonthRecordStudyQuery([FromQuery] string userId) {
        if (string.IsNullOrEmpty(userId)) {
            throw new UnauthorizedAccessException("User not found");
        }
        var actualMonth = DateTime.Now.Month;
        var actualYear = DateTime.Now.Year;

        var actualMonthStudies = await dbContext.Records
            .Where(record =>
                record.IdentityUserId == userId &&
                record.Date.Month == actualMonth &&
                record.Date.Year == actualYear
                )
            .SumAsync(record => record.RecordStudy ?? 0);
        return actualMonthStudies;
    }

    public async Task<int> SumActualWeekRecordStudyQuery([FromQuery] string userId) {
        if (string.IsNullOrEmpty(userId)) {
            throw new UnauthorizedAccessException("User not found");
        }

        int exactDay = (int)DateTime.Now.DayOfWeek;
        var actualDate = DateTime.Now;
        var startWeek = actualDate.AddDays(-(exactDay - 1));
        if (actualDate.DayOfWeek == DayOfWeek.Sunday) {
            startWeek = actualDate.AddDays(-6);
        }
        var endWeek = startWeek.AddDays(7);

        int actualWeekStudies = await dbContext.Records
            .Where(record => record.Date >= DateOnly.FromDateTime(startWeek) &&
                             record.Date <= DateOnly.FromDateTime(endWeek))
            .SumAsync(record => record.RecordStudy ?? 0);
        return actualWeekStudies;
    }
    
    
}