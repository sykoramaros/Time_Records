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
}