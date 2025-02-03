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
    
    [HttpGet("GetAllRecords")]
    public ActionResult<IEnumerable<RecordDto>> GetAllRecords() {
        var records = recordService.GetAllRecords();
        if (records == null || !records.Any()) {
            return NotFound("No records found");
        }
        return Ok(records);
    }
    
    [HttpGet("GetRecordById/{id}")]
    public async Task<ActionResult<RecordDto>> GetRecordByIdAsync(int id) {
        var record = await recordService.GetRecordByIdAsync(id);
        if (record == null) {
            return NotFound("Record not found");
        }
        return Ok(record);
    }
    
    [HttpGet("GetRecordByDate/{date}")]
    public async Task<ActionResult<RecordDto>> GetRecordByDateAsync(DateOnly date) {
        var record = await recordService.GetRecordByDateAsync(date);
        if (record == null) {
            return NotFound("Record not found");
        }
        return Ok(record);
    }
    
    // spravny json format
    // {
    //     "id": 0,
    //     "date": "2023-04-15",
    //     "recordTime": "05:40:00",
    //     "recordStudy": 1,
    //     "description": "zkouska swagger s pocatecni nulou"
    // }
    
    [HttpPost("CreateRecord")]
    public async Task<ActionResult> CreateRecordAsync([FromBody] RecordDto recordDto) {
        if (!ModelState.IsValid) {
            return BadRequest(ModelState);
        }
        try {
            await recordService.CreateRecordAsync(recordDto);
            return Ok();
        }
        catch (Exception exception) {
            return StatusCode(500, "An error occurred while creating the record: " + exception.Message);
        }
    }
    
    // zatim haze error 400
    [HttpPut("EditRecordByDate/{date}")]
    public async Task<ActionResult> EditRecordByDateAsync(DateOnly date, RecordDto recordDto) {
        await recordService.EditRecordByDateAsync(date, recordDto);
        return Ok();
    }
    
    [HttpPut("EditRecordById/{id}")]
    public async Task<ActionResult> EditRecordByIdAsync(int id, RecordDto recordDto) {
        await recordService.EditRecordByIdAsync(id, recordDto);
        return Ok();
    }
    
    [HttpDelete("DeleteRecordById/{id}")]
    public async Task<IActionResult> DeleteRecordByIdAsync(int id) {
        var recordToDelete = await recordService.GetRecordByIdAsync(id);
        if (recordToDelete == null) {
            return NotFound("Record not found");
        }
        await recordService.DeleteRecordByIdAsync(id);
        return Ok();
    }
    
    [HttpDelete("DeleteRecordByDate/{date}")]
    public async Task<IActionResult> DeleteRecordByDateAsync(DateOnly date) {
        var recordToDelete = await recordService.GetRecordByDateAsync(date);
        if (recordToDelete == null) {
            return NotFound("Record not found");
        }
        await recordService.DeleteRecordByDateAsync(date);
        return Ok();
    }
    
    [HttpGet("SumDayTotalRecordTime")]
    public async Task<ActionResult<TimeSpan>> SumDayTotalRecordTimeAsync() {
        var sum = await recordService.SumDayTotalRecordTimeAsync();
        return Ok(sum);
    }
    
    [HttpGet("SumHoursTotalRecordTimeInHoursAsStringAsync")]
    public async Task<ActionResult<double>> SumHoursTotalRecordTimeInHoursAsStringAsync() {
        var sum = await recordService.SumHoursTotalRecordTimeInHoursAsStringAsync();
        return Ok(sum);
    }
    
    [HttpGet("SumHoursTotalRecordTimeInHoursAsIntAsync")]
    public async Task<ActionResult<double>> SumHoursTotalRecordTimeInHoursAsIntAsync() {
        var sum = await recordService.SumHoursTotalRecordTimeInHoursAsIntAsync();
        return Ok(sum);
    }
    
    [HttpGet("SumHoursTotalRecordTimeInHoursAsDoubleAsync")]
    public async Task<ActionResult<double>> SumHoursTotalRecordTimeInHoursAsDoubleAsync() {
        var sum = await recordService.SumHoursTotalRecordTimeInHoursAsDoubleAsync();
        return Ok(sum);
    }
}