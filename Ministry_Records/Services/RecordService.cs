using Ministry_Records.DTO;
using Ministry_Records.Models;
using Microsoft.EntityFrameworkCore;
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

    internal async Task EditRecordByDateAsync(DateOnly date, RecordDto editedRecord) {
        dbContext.Update(DtoToModel(editedRecord));
        await dbContext.SaveChangesAsync();
    }
    
    internal async Task<RecordDto> GetRecordByIdAsync(int id) {
        var recordToEdit = await dbContext.Records
            .FirstOrDefaultAsync(r => r.Id == id);
        if (recordToEdit == null) {
            return null;
        }
        return ModelToDto(recordToEdit);
    }

    internal async Task EditRecordByIdAsync(int id, RecordDto editedRecord) {
        dbContext.Update(DtoToModel(editedRecord));
        await dbContext.SaveChangesAsync();
    }

    internal async Task DeleteRecordByDateAsync(DateOnly date) {
        var recordToDelete = await dbContext.Records
            .FirstOrDefaultAsync(r => r.Date == date);
        dbContext.Records.Remove(recordToDelete);
        await dbContext.SaveChangesAsync();
    }
    
    internal async Task DeleteRecordByIdAsync(int id) {
        var recordToDelete = await dbContext.Records
            .FindAsync(id);
        dbContext.Records.Remove(recordToDelete);
        await dbContext.SaveChangesAsync();
    }
}