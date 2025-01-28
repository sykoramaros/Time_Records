using Microsoft.AspNetCore.Mvc;
using Ministry_Records.DTO;
using Ministry_Records.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Pomelo.EntityFrameworkCore.MySql;

namespace Ministry_Records.Services;

public partial class RecordService {
    private ApplicationDbContext dbContext;

    public RecordService(ApplicationDbContext dbContext) {
        this.dbContext = dbContext;
    }

    private static RecordDto ModelToDto(Record record) {
        return new RecordDto {
            Id = record.Id,
            Date = record.Date,
            RecordTime = record.RecordTime,
            RecordStudy = record.RecordStudy,
            Description = record.Description
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
            Description = recordDto.Description
        };
    }

    public async Task CreateRecordAsync(RecordDto recordDto) {
        await dbContext.Records.AddAsync(DtoToModel(recordDto));
        await dbContext.SaveChangesAsync();
    }

// internal IEnumerable<RecordDto> GetAllRecords() {
    //     var allRecords = dbContext.Records;
    //
    //     var recordDtos = new List<RecordDto>();
    //     foreach (var record in allRecords) {
    //         recordDtos.Add(ModelToDto(record));
    //     }
    //     return recordDtos;
    // }

    // GetAllRecords LINQ zkracena rychlejsi varianta se stejnym vysledkem 
    
    internal IEnumerable<RecordDto> GetAllRecords() {
        return dbContext.Records
            .Select(ModelToDto);
    }

    public IEnumerable<RecordDto> GetAllRecordsByDay(DateOnly chosenDate) {
        return dbContext.Records
            .Where(r => r.Date == chosenDate)
            .Select(ModelToDto);
    }

    public IEnumerable<RecordDto> GetAllRecordsByWeek(DateOnly chosenDate) {
        var startOfWeek = chosenDate.AddDays(1 - (int)chosenDate.DayOfWeek);
        var endOfWeek = startOfWeek.AddDays(7);
        return dbContext.Records
            .Where(r => r.Date >= startOfWeek && r.Date < endOfWeek)
            .Select(ModelToDto);
    }

    public IEnumerable<RecordDto> GetAllRecordsByMonthAndYear(int month, int year) {
        return dbContext.Records
            .Where(r => r.Date.Month == month && r.Date.Year == year)
            .Select(ModelToDto);
    }

    public IEnumerable<RecordDto> GetAllRecordsByYear(int year) {
        return dbContext.Records
            .Where(r => r.Date.Year == year)
            .Select(ModelToDto);
    }
    
    internal async Task<RecordDto> GetRecordByIdAsync(int id) {
        var recordToEdit = await dbContext.Records
            .FirstOrDefaultAsync(r => r.Id == id);
        if (recordToEdit == null) {
            return null;
        }
        return ModelToDto(recordToEdit);
    }
    
    // haze error 400
    internal async Task EditRecordByDateAsync(DateOnly date, RecordDto editedRecord) {
        dbContext.Update(DtoToModel(editedRecord));
        await dbContext.SaveChangesAsync();
    }
    
    internal async Task EditRecordByIdAsync(int id, RecordDto editedRecord) {
        dbContext.Update(DtoToModel(editedRecord));
        await dbContext.SaveChangesAsync();
    }
    
    internal async Task DeleteRecordByIdAsync(int id) {
        var recordToDelete = await dbContext.Records
            .FindAsync(id);
        dbContext.Records.Remove(recordToDelete);
        await dbContext.SaveChangesAsync();
    }

    // zatim nefunkcni
    internal async Task DeleteRecordByDateAsync(DateOnly date) {
        var recordToDelete = await dbContext.Records
            .FirstOrDefaultAsync(r => r.Date == date);
        dbContext.Records.Remove(recordToDelete);
        await dbContext.SaveChangesAsync();
    }
    
    // public async Task<TimeSpan> SumTotalRecordTimeAsync() {
    //     // Načítáme celkový čas v "Ticks"
    //     var totalTicks = await dbContext.Records
    //         .SumAsync(r => (long?)r.RecordTime.Ticks ?? 0); // Převod na nullable long
    //
    //     // Převod Ticks zpět na TimeSpan
    //     return TimeSpan.FromTicks(totalTicks);
    // }
    
    public async Task<TimeSpan> SumDayTotalRecordTimeAsync() {
        // Načteme data z databáze
        var records = await dbContext.Records
            .Where(r => r.RecordTime != null) // Filtrování null hodnot
            .ToListAsync(); // Načteme všechny záznamy do paměti
        // Vypočítáme součet minut na straně klienta
        var totalMinutes = records
            .Sum(r => r.RecordTime.TotalMinutes); // Používáme TotalMinutes pro součet minut
        // Převod zpět na TimeSpan
        return TimeSpan.FromMinutes(totalMinutes);
    }

// Řešení 1: Načtení dat do paměti a následný výpočet
    public async Task<string> SumHoursTotalRecordTimeInHoursAsStringAsync()
    {
        var records = await dbContext.Records.ToListAsync();
        var totalTime = records.Aggregate(TimeSpan.Zero, (sum, record) => sum + record.RecordTime);
    
        int hours = (int)totalTime.TotalHours;
        int minutes = totalTime.Minutes;
    
        return $"{hours} hodin {minutes} minut";
    }
    
    public async Task<(int hours, int minutes)> SumHoursTotalRecordTimeInHoursAsIntAsync()
    {
        var records = await dbContext.Records.ToListAsync();
        var totalTime = records.Aggregate(TimeSpan.Zero, (sum, record) => sum + record.RecordTime);
    
        int hours = (int)totalTime.TotalHours;
        int minutes = totalTime.Minutes;
    
        return (hours, minutes);
    }
    
    public async Task<double> SumHoursTotalRecordTimeInHoursAsDoubleAsync()
    {
        var records = await dbContext.Records.ToListAsync();
        var totalTime = records.Aggregate(TimeSpan.Zero, (sum, record) => sum + record.RecordTime);
    
        return totalTime.TotalHours;
    }
}
