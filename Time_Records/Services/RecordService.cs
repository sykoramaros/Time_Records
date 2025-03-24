using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Time_Records.DTO;
using Time_Records.Models;
using Microsoft.EntityFrameworkCore;

namespace Time_Records.Services;

public class RecordService {
    private ApplicationDbContext dbContext;
    private UserManager<AppUser> userManager;
    private IHttpContextAccessor httpContextAccessor;

    // public RecordService(ApplicationDbContext dbContext) {
    //     this.dbContext = dbContext;
    // }

    public RecordService(ApplicationDbContext dbContext, UserManager<AppUser> userManager) {
        this.dbContext = dbContext;
        this.userManager = userManager;
    }

    public RecordService(ApplicationDbContext dbContext, UserManager<AppUser> userManager,
        IHttpContextAccessor httpContextAccessor) {
        this.dbContext = dbContext;
        this.userManager = userManager;
        this.httpContextAccessor = httpContextAccessor;
    }

    private static RecordDto ModelToDto(Record record) {
        return new RecordDto {
            Id = record.Id,
            Date = record.Date,
            RecordTime = record.RecordTime,
            RecordStudy = record.RecordStudy,
            Description = record.Description,
            IdentityUserId = record.IdentityUserId
        };
    }

    private Record DtoToModel(RecordDto recordDto) {
        return new Record {
            Id = recordDto.Id,
            Date = recordDto.Date,
            RecordTime = recordDto.RecordTime,
            // pokud jsou zadany jen hodiny a minuty, automaticky se sekundy +00 pridaji 
            // RecordTime = recordDto.RecordTime.Seconds == 0 ?
            //     recordDto.RecordTime.Add(TimeSpan.FromSeconds(0)) :
            //     recordDto.RecordTime,
            RecordStudy = recordDto.RecordStudy,
            Description = recordDto.Description,
            IdentityUserId = recordDto.IdentityUserId
        };
    }

    public async Task CreateRecordQueryAsync([FromQuery] Guid userId, RecordDto recordDto) {
        if (userId == Guid.Empty) {
            throw new UnauthorizedAccessException("User not found");
        }

        var record = DtoToModel(recordDto);
        record.IdentityUserId = userId;
        await dbContext.Records.AddAsync(record);
        await dbContext.SaveChangesAsync();
    }


    internal async Task<IEnumerable<RecordDto>> GetAllRecords() {
        // var user = await userManager.GetUserAsync(httpContextAccessor.HttpContext.User);
        var userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Console.WriteLine($"Decoded userId: {userId}");
        if (userId == null) {
            throw new UnauthorizedAccessException("User not found");
        }

        return dbContext.Records
            .Where(r => r.IdentityUserId == Guid.Parse(userId))
            .Select(ModelToDto)
            .AsEnumerable();
    }

    internal async Task<IEnumerable<RecordDto>> GetAllRecordsQuery([FromQuery] Guid userId) {
        if (userId == Guid.Empty) {
            throw new UnauthorizedAccessException("User not found");
        }

        return dbContext.Records
            .Where(r => r.IdentityUserId == userId)
            .Select(ModelToDto)
            .AsEnumerable();
    }

    internal async Task<RecordDto> GetRecordByDateQueryAsync([FromQuery] Guid userId, [FromQuery] DateOnly date) {
        if (userId == Guid.Empty) {
            throw new UnauthorizedAccessException("User not found");
        }

        var recordToEdit = await dbContext.Records
            .FirstOrDefaultAsync(r => r.IdentityUserId == userId && r.Date == date);
        if (recordToEdit == null) {
            throw new UnauthorizedAccessException("Record not found");
        }

        return ModelToDto(recordToEdit);
    }

    internal async Task EditRecordByDateQueryAsync([FromQuery] Guid userId, [FromQuery] DateOnly date,
        RecordDto editedRecord) {
        if (userId == Guid.Empty) {
            throw new UnauthorizedAccessException("User not found");
        }

        var recordToEdit = await dbContext.Records
            .FirstOrDefaultAsync(r => r.IdentityUserId == userId && r.Date == date);
        if (recordToEdit == null) {
            return;
        }

        recordToEdit.Date = editedRecord.Date;
        recordToEdit.RecordTime = editedRecord.RecordTime;
        recordToEdit.RecordStudy = editedRecord.RecordStudy;
        recordToEdit.Description = editedRecord.Description;
        dbContext.Update(recordToEdit);
        await dbContext.SaveChangesAsync();
    }

    internal async Task DeleteRecordByDateQueryAsync([FromQuery] Guid userId, [FromQuery] DateOnly date) {
        if (userId == Guid.Empty) {
            throw new UnauthorizedAccessException("User not found");
        }

        var recordToDelete = await dbContext.Records
            .FirstOrDefaultAsync(r => r.IdentityUserId == userId && r.Date == date);
        if (recordToDelete != null) {
            dbContext.Records.Remove(recordToDelete);
            await dbContext.SaveChangesAsync();
        }
    }

    internal async Task DeleteAllRecordsQueryAsync([FromQuery] Guid userId) {
        if (userId == Guid.Empty) {
            throw new UnauthorizedAccessException("User not found");
        }

        var allRecordsToDelete = await dbContext.Records
            .Where(r => r.IdentityUserId == userId)
            .ToListAsync();
        if (allRecordsToDelete != null) {
            dbContext.Records.RemoveRange(allRecordsToDelete);
            await dbContext.SaveChangesAsync();
        }
    }
}


// public async Task CreateRecordAsync(RecordDto recordDto) {
    //     var user = await userManager.GetUserAsync(httpContextAccessor.HttpContext.User);
    //     if (user == null) {
    //         throw new UnauthorizedAccessException("User not found");
    //     }
    //     var record = DtoToModel(recordDto);
    //     record.IdentityUserId = user.Id;
    //     await dbContext.Records.AddAsync(record);
    //     await dbContext.SaveChangesAsync();
    // }
    // internal async Task<RecordDto> GetRecordByIdAsync(int id) {
    //     var user = await userManager.GetUserAsync(httpContextAccessor.HttpContext.User);
    //     if (user == null) {
    //         throw new UnauthorizedAccessException("User not found");
    //     }
    //     var recordToEdit = await dbContext.Records
    //         .FirstOrDefaultAsync(r => r.Id == id && r.IdentityUserId == user.Id);
    //     if (recordToEdit == null) {
    //         return null;
    //     }
    //     return ModelToDto(recordToEdit);
    // }
    // internal async Task<RecordDto> GetRecordByDateAsync(DateOnly date) {
    //     var user = await userManager.GetUserAsync(httpContextAccessor.HttpContext.User);
    //     if (user == null) {
    //         throw new UnauthorizedAccessException("User not found");
    //     }
    //     var recordToEdit = await dbContext.Records
    //         .FirstOrDefaultAsync(r => r.Date == date && r.IdentityUserId == user.Id);
    //     if (recordToEdit == null) {
    //         return null;
    //     }
    //     return ModelToDto(recordToEdit);
    // }
    // public async Task CreateRecordAsync(RecordDto recordDto) {
    //     await dbContext.Records.AddAsync(DtoToModel(recordDto));
    //     await dbContext.SaveChangesAsync();
    // }
    // internal IEnumerable<RecordDto> GetAllRecords() {
    //     return dbContext.Records
    //         .Select(ModelToDto);
    // }
    
    // internal IEnumerable<RecordDto> GetAllRecords() {
    //     var user = userManager.GetUserAsync(httpContextAccessor.HttpContext.User).Result;
    //     if (user == null) {
    //         throw new UnauthorizedAccessException("User not found");
    //     }
    //     return dbContext.Records
    //         .Where(r => r.IdentityUserId == user.Id)
    //         .Select(ModelToDto);
    // }
    // internal async Task<RecordDto> GetRecordByIdAsync(int id) {
    //     var recordToEdit = await dbContext.Records
    //         .FirstOrDefaultAsync(r => r.Id == id);
    //     if (recordToEdit == null) {
    //         return null;
    //     }
    //     return ModelToDto(recordToEdit);
    // }
    
    // internal async Task<RecordDto> GetRecordByDateAsync(DateOnly date) {
    //     var recordToEdit = await dbContext.Records
    //         .FirstOrDefaultAsync(r => r.Date == date);
    //     if (recordToEdit == null) {
    //         return null;
    //     }
    //     return ModelToDto(recordToEdit);
    // }
    
    
    // public IEnumerable<RecordDto> GetAllRecordsByDay(DateOnly chosenDate) {
    //     return dbContext.Records
    //         .Where(r => r.Date == chosenDate)
    //         .Select(ModelToDto);
    // }
    //
    // public IEnumerable<RecordDto> GetAllRecordsByWeek(DateOnly chosenDate) {
    //     var startOfWeek = chosenDate.AddDays(1 - (int)chosenDate.DayOfWeek);
    //     var endOfWeek = startOfWeek.AddDays(7);
    //     return dbContext.Records
    //         .Where(r => r.Date >= startOfWeek && r.Date < endOfWeek)
    //         .Select(ModelToDto);
    // }
    //
    // public IEnumerable<RecordDto> GetAllRecordsByMonthAndYear(int month, int year) {
    //     return dbContext.Records
    //         .Where(r => r.Date.Month == month && r.Date.Year == year)
    //         .Select(ModelToDto);
    // }
    //
    // public IEnumerable<RecordDto> GetAllRecordsByYear(int year) {
    //     return dbContext.Records
    //         .Where(r => r.Date.Year == year)
    //         .Select(ModelToDto);
    // }
    //
    // internal async Task EditRecordByDateAsync(DateOnly date, RecordDto editedRecord) {
    //     var recordToEdit = await dbContext.Records
    //         .FirstOrDefaultAsync(r => r.Date == date);
    //     if (recordToEdit == null) {
    //         return;
    //     }
    //     recordToEdit.Date = editedRecord.Date;
    //     recordToEdit.RecordTime = editedRecord.RecordTime;
    //     recordToEdit.RecordStudy = editedRecord.RecordStudy;
    //     recordToEdit.Description = editedRecord.Description;
    //     dbContext.Update(recordToEdit);
    //     await dbContext.SaveChangesAsync();
    // }
    
    // internal async Task EditRecordByIdAsync(int id, RecordDto editedRecord) {
    //     var recordToEdit = await dbContext.Records
    //         .FirstOrDefaultAsync(r => r.Id == id);
    //     if (recordToEdit == null) {
    //         return;
    //     }
    //     recordToEdit.Date = editedRecord.Date;
    //     recordToEdit.RecordTime = editedRecord.RecordTime;
    //     recordToEdit.RecordStudy = editedRecord.RecordStudy;
    //     recordToEdit.Description = editedRecord.Description;
    //     dbContext.Update(recordToEdit);
    //     await dbContext.SaveChangesAsync();
    // }
    //
    // internal async Task DeleteRecordByIdAsync(int id) {
    //     var recordToDelete = await dbContext.Records
    //         .FindAsync(id);
    //     dbContext.Records.Remove(recordToDelete);
    //     await dbContext.SaveChangesAsync();
    // }
    
    // // zatim nefunkcni
    // internal async Task DeleteRecordByDateAsync(DateOnly date) {
    //     var recordToDelete = await dbContext.Records
    //         .FirstOrDefaultAsync(r => r.Date == date);
    //     dbContext.Records.Remove(recordToDelete);
    //     await dbContext.SaveChangesAsync();
    // }
    
    // public async Task<TimeSpan> SumTotalRecordTimeAsync() {
    //     // Načítáme celkový čas v "Ticks"
    //     var totalTicks = await dbContext.Records
    //         .SumAsync(r => (long?)r.RecordTime.Ticks ?? 0); // Převod na nullable long
    //
    //     // Převod Ticks zpět na TimeSpan
    //     return TimeSpan.FromTicks(totalTicks);
    // }
    
//     public async Task<TimeSpan> SumDayTotalRecordTimeAsync() {
//         // Načteme data z databáze
//         var records = await dbContext.Records
//             .Where(r => r.RecordTime != null) // Filtrování null hodnot
//             .ToListAsync(); // Načteme všechny záznamy do paměti
//         // Vypočítáme součet minut na straně klienta
//         var totalMinutes = records
//             .Sum(r => r.RecordTime.TotalMinutes); // Používáme TotalMinutes pro součet minut
//         // Převod zpět na TimeSpan
//         return TimeSpan.FromMinutes(totalMinutes);
//     }
//
// // Řešení 1: Načtení dat do paměti a následný výpočet
//     public async Task<string> SumHoursTotalRecordTimeInHoursAsStringAsync() {
//         var records = await dbContext.Records.ToListAsync();
//         var totalTime = records.Aggregate(TimeSpan.Zero, (sum, record) => sum + record.RecordTime);
//     
//         int hours = (int)totalTime.TotalHours;
//         int minutes = totalTime.Minutes;
//     
//         return $"{hours} hodin {minutes} minut";
//     }
//     
//     public async Task<(int hours, int minutes)> SumHoursTotalRecordTimeInHoursAsIntAsync() {
//         var records = await dbContext.Records.ToListAsync();
//         var totalTime = records.Aggregate(TimeSpan.Zero, (sum, record) => sum + record.RecordTime);
//     
//         int hours = (int)totalTime.TotalHours;
//         int minutes = totalTime.Minutes;
//     
//         return (hours, minutes);
//     }
//     
//     public async Task<double> SumHoursTotalRecordTimeInHoursAsDoubleAsync() {
//         var records = await dbContext.Records.ToListAsync();
//         var totalTime = records.Aggregate(TimeSpan.Zero, (sum, record) => sum + record.RecordTime);
//     
//         return totalTime.TotalHours;
//     }
