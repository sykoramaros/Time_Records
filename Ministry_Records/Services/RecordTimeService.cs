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
    
    public async Task<TimeFormatDto> SumActualMinistryYearTotalRecordTimeAsync() {
        var records = await dbContext.Records.ToListAsync();
        var actualYear = DateTime.Now.Year;
        var startMinistryYear = new DateOnly(actualYear - 1, 9, 1);
        var endMinistryYear = new DateOnly(actualYear, 8, 31);
        Console.WriteLine(startMinistryYear + " " + endMinistryYear);
        var ministryYearrecords = records
            .Where(record => record.Date >= startMinistryYear && record.Date <= endMinistryYear)
            .Aggregate(TimeSpan.Zero, (sum, record) => sum + record.RecordTime);

        return new TimeFormatDto() 
        { 
            Hours = (int)ministryYearrecords.TotalHours,
            Minutes = ministryYearrecords.Minutes
        };
    }
    
    public async Task<double> YearRecordProgressAsync() {
        var yearTotalRecord = await SumActualMinistryYearTotalRecordTimeAsync();
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

    public async Task<TimeFormatDto> SumActualMonthTotalRecordTimeAsync() {
        var records = await dbContext.Records.ToListAsync();
        var actualMonth = DateTime.Now.Month;
        var actualYear = DateTime.Now.Year;
        var actualMonthRecords = records
            .Where(record => record.Date.Year == actualYear && record.Date.Month == actualMonth)
            .Aggregate(TimeSpan.Zero, (sum, record) => sum + record.RecordTime);

        return new TimeFormatDto() 
        { 
            Hours = (int)actualMonthRecords.TotalHours,
            Minutes = actualMonthRecords.Minutes
        };
    }

    public async Task<double> MonthRecordProgressAsync() {
        var monthTotalRecord = await SumActualMonthTotalRecordTimeAsync();
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
    
    public async Task<TimeFormatDto> SumActualWeekTotalRecordTimeAsync() {
        var records = await dbContext.Records.ToListAsync();
        // Převedeme DayOfWeek na hodnotu, kde pondělí = 1, neděle = 7
        var actualDay = DateTime.Now;
        int dayOfWeek;
        if (actualDay.DayOfWeek == DayOfWeek.Sunday) {
            dayOfWeek = 7;
        } else {
            dayOfWeek = (int)actualDay.DayOfWeek;
        }
        // Vypočítáme pondělí daného týdne
        // var startOfWeek = actualDay.AddDays(1 - dayOfWeek);
        var startOfWeek = actualDay.AddDays(-(dayOfWeek - 1)).Date;
        var endOfWeek = startOfWeek.AddDays(7);

        var weekRecords = records
            .Where(record => record.Date >= DateOnly.FromDateTime(startOfWeek) && 
                        record.Date < DateOnly.FromDateTime(endOfWeek))
            .Aggregate(TimeSpan.Zero, (sum, record) => sum + record.RecordTime);
        return new TimeFormatDto() 
        { 
            Hours = (int)weekRecords.TotalHours,
            Minutes = weekRecords.Minutes
        };
    }

    public async Task<double> WeekRecordProgressAsync() {
        var weekTotalRecord = await SumActualWeekTotalRecordTimeAsync();
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