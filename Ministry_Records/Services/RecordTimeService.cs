using Microsoft.EntityFrameworkCore;
using Ministry_Records.DTO;

namespace Ministry_Records.Services;

public class RecordTimeService {
    private ApplicationDbContext dbContext;

    public RecordTimeService(ApplicationDbContext dbContext) {
        this.dbContext = dbContext;
    }
    
    public async Task<TimeFormatDto> SumTotalRecordTimeAsync() {
        var records = await dbContext.Records.ToListAsync();
        var totalTime = records.Aggregate(TimeSpan.Zero, (sum, record) => sum + record.RecordTime);

        return new TimeFormatDto() 
        { 
            Hours = (int)totalTime.TotalHours,
            Minutes = totalTime.Minutes
        };
    }

    public async Task<TimeFormatDto> SumMonthTotalRecordTimeAsync(DateOnly chosenMonthYear) {
        var records = await dbContext.Records.ToListAsync();

        var chosenMonthRecords = records
            .Where(record => record.Date.Year == chosenMonthYear.Year && record.Date.Month == chosenMonthYear.Month)
            .Aggregate(TimeSpan.Zero, (sum, record) => sum + record.RecordTime);

        return new TimeFormatDto() 
        { 
            Hours = (int)chosenMonthRecords.TotalHours,
            Minutes = chosenMonthRecords.Minutes
        };
    }
    
    public async Task<TimeFormatDto> SumWeekTotalRecordTimeAsync(DateOnly chosenDay) {
        var records = await dbContext.Records.ToListAsync();
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

        var chosenWeekRecords = records
            .Where(record => record.Date >= startOfWeek && record.Date < endOfWeek)
            .Aggregate(TimeSpan.Zero, (sum, record) => sum + record.RecordTime);

        return new TimeFormatDto() 
        { 
            Hours = (int)chosenWeekRecords.TotalHours,
            Minutes = chosenWeekRecords.Minutes
        };
    }
}