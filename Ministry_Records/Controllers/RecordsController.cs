using Microsoft.AspNetCore.Mvc;
using Ministry_Records.DTO;
using Ministry_Records.Services;

namespace Ministry_Records.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RecordsController : ControllerBase {
    private RecordService recordService;

    public RecordsController(RecordService recordService) {
        this.recordService = recordService;
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<RecordDto>> GetAllRecords() {
        var records = recordService.GetAllRecords();
        if (records == null || !records.Any()) {
            return NotFound("No records found");
        }
        return Ok(records);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<RecordDto>> GetRecordByIdAsync(int id) {
        var record = await recordService.GetRecordByIdAsync(id);
        if (record == null) {
            return NotFound("Record not found");
        }
        return Ok(record);
    }
}