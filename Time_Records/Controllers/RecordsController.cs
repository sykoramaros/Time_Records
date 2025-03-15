using Microsoft.AspNetCore.Mvc;
using Time_Records.DTO;
using Time_Records.Services;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Time_Records.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RecordsController : ControllerBase {
    private RecordService recordService;

    public RecordsController(RecordService recordService) {
        this.recordService = recordService;
    }
    
    
    [HttpGet("GetAllRecords")]
    public async Task<ActionResult<IEnumerable<RecordDto>>> GetAllRecords() {
        var records = await recordService.GetAllRecords();
        if (records == null || !records.Any()) {
            return NotFound("No records found");
        }
        return Ok(records);
    }
    
    [HttpGet("GetAllRecordsQuery")]
    public async Task<ActionResult<IEnumerable<RecordDto>>> GetAllRecordsQuery([FromQuery] Guid userId) {
        var records = await recordService.GetAllRecordsQuery(userId);
        if (records == null || !records.Any()) {
            return NotFound("No records found");
        }
        return Ok(records);
    }
    
    
    [HttpGet("GetRecordByDateQuery")]
    public async Task<ActionResult<RecordDto>> GetRecordByDateQueryAsync([FromQuery] Guid userId, [FromQuery] DateOnly date) {
        var record = await recordService.GetRecordByDateQueryAsync(userId, date);
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
    
    
    [HttpPost("CreateRecordQuery")]
    public async Task<ActionResult> CreateRecordQueryAsync([FromQuery] Guid userId, [FromBody] RecordDto recordDto) {
        if (!ModelState.IsValid) {
            return BadRequest(ModelState);
        }
        try {
            await recordService.CreateRecordQueryAsync(userId, recordDto);
            return Ok();
        }
        catch (Exception exception) {
            return StatusCode(500, "An error occurred while creating the record: " + exception.Message);
        }
    }
    
    
    [HttpPut("EditRecordByDateQuery")]
    public async Task<ActionResult> EditRecordByDateQueryAsync([FromQuery] Guid userId, [FromQuery] DateOnly date, [FromBody] RecordDto editedRecord) {
        try {
            await recordService.EditRecordByDateQueryAsync(userId, date, editedRecord);
            return Ok();
        }
        catch (UnauthorizedAccessException) {
            return Unauthorized();
        }
        catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpDelete("DeleteRecordByDateQuery")]
    public async Task<ActionResult> DeleteRecordQueryAsync([FromQuery] Guid userId, [FromQuery] DateOnly date) {
        try {
            await recordService.DeleteRecordByDateQueryAsync(userId, date);
            return Ok();
        }
        catch (UnauthorizedAccessException) {
            return Unauthorized();
        }
        catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }
    
    // [HttpPost("CreateRecord")]
    // public async Task<ActionResult> CreateRecordAsync([FromBody] RecordDto recordDto) {
    //     if (!ModelState.IsValid) {
    //         return BadRequest(ModelState);
    //     }
    //     try {
    //         await recordService.CreateRecordAsync(recordDto);
    //         return Ok();
    //     }
    //     catch (Exception exception) {
    //         return StatusCode(500, "An error occurred while creating the record: " + exception.Message);
    //     }
    // }
    // [HttpGet("GetRecordById/{id}")]
    // public async Task<ActionResult<RecordDto>> GetRecordByIdAsync(int id) {
    //     var record = await recordService.GetRecordByIdAsync(id);
    //     if (record == null) {
    //         return NotFound("Record not found");
    //     }
    //     return Ok(record);
    // }
    //
    // [HttpGet("GetRecordByDate/{date}")]
    // public async Task<ActionResult<RecordDto>> GetRecordByDateAsync(DateOnly date) {
    //     var record = await recordService.GetRecordByDateAsync(date);
    //     if (record == null) {
    //         return NotFound("Record not found");
    //     }
    //     return Ok(record);
    // }
    // [HttpGet("GetAllRecords")]
    // public ActionResult<IEnumerable<RecordDto>> GetAllRecords() {
    //     var records = recordService.GetAllRecords();
    //     if (records == null || !records.Any()) {
    //         return NotFound("No records found");
    //     }
    //     return Ok(records);
    // }
    // zatim haze error 400
    // [HttpPut("EditRecordByDate/{date}")]
    // public async Task<ActionResult> EditRecordByDateAsync(DateOnly date, RecordDto recordDto) {
    //     await recordService.EditRecordByDateAsync(date, recordDto);
    //     return Ok();
    // }
    // [HttpPut("EditRecordById/{id}")]
    // public async Task<ActionResult> EditRecordByIdAsync(int id, RecordDto recordDto) {
    //     await recordService.EditRecordByIdAsync(id, recordDto);
    //     return Ok();
    // }
    
    // [HttpDelete("DeleteRecordById/{id}")]
    // public async Task<IActionResult> DeleteRecordByIdAsync(int id) {
    //     var recordToDelete = await recordService.GetRecordByIdAsync(id);
    //     if (recordToDelete == null) {
    //         return NotFound("Record not found");
    //     }
    //     await recordService.DeleteRecordByIdAsync(id);
    //     return Ok();
    // }
    
    // [HttpDelete("DeleteRecordByDate/{date}")]
    // public async Task<IActionResult> DeleteRecordByDateAsync(DateOnly date) {
    //     var recordToDelete = await recordService.GetRecordByDateAsync(date);
    //     if (recordToDelete == null) {
    //         return NotFound("Record not found");
    //     }
    //     await recordService.DeleteRecordByDateAsync(date);
    //     return Ok();
    // }
    
    
    // [HttpGet("SumDayTotalRecordTime")]
    // public async Task<ActionResult<TimeSpan>> SumDayTotalRecordTimeAsync() {
    //     var sum = await recordService.SumDayTotalRecordTimeAsync();
    //     return Ok(sum);
    // }
    
    // [HttpGet("SumHoursTotalRecordTimeInHoursAsStringAsync")]
    // public async Task<ActionResult<double>> SumHoursTotalRecordTimeInHoursAsStringAsync() {
    //     var sum = await recordService.SumHoursTotalRecordTimeInHoursAsStringAsync();
    //     return Ok(sum);
    // }
    
    // [HttpGet("SumHoursTotalRecordTimeInHoursAsIntAsync")]
    // public async Task<ActionResult<double>> SumHoursTotalRecordTimeInHoursAsIntAsync() {
    //     var sum = await recordService.SumHoursTotalRecordTimeInHoursAsIntAsync();
    //     return Ok(sum);
    // }
    //
    // [HttpGet("SumHoursTotalRecordTimeInHoursAsDoubleAsync")]
    // public async Task<ActionResult<double>> SumHoursTotalRecordTimeInHoursAsDoubleAsync() {
    //     var sum = await recordService.SumHoursTotalRecordTimeInHoursAsDoubleAsync();
    //     return Ok(sum);
    // }
}