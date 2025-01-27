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
    
    [HttpGet("getAllRecords")]
    public ActionResult<IEnumerable<RecordDto>> GetAllRecords() {
        var records = recordService.GetAllRecords();
        if (records == null || !records.Any()) {
            return NotFound("No records found");
        }
        return Ok(records);
    }
    
    [HttpGet("getById/{id}")]
    public async Task<ActionResult<RecordDto>> GetRecordByIdAsync(int id) {
        var record = await recordService.GetRecordByIdAsync(id);
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
    
    [HttpPost("createRecord")]
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
    
    // [HttpPut("EditRecordById/{id}")]
    // public async Task<ActionResult> EditRecordByIdAsync(int id, RecordDto recordDto) {
    //     await recordService.EditRecordByIdAsync(id, recordDto);
    //     return Ok();
    // }
    
    // jeste nefunguje i kdyz vraci kod 200
    [HttpPut("EditRecordById/{id}")]
    public async Task<IActionResult> EditRecordByIdAsync(int id, [FromBody] RecordDto editedRecord) {
        if (!ModelState.IsValid) {
            return BadRequest(error: ModelState);
        }
        var recordToEdit = await recordService.GetRecordByIdAsync(id);
        if (recordToEdit == null) {
            return NotFound("Record not found");
        }
        recordToEdit.Date = editedRecord.Date;
        recordToEdit.RecordTime = editedRecord.RecordTime;
        recordToEdit.RecordStudy = editedRecord.RecordStudy;
        recordToEdit.Description = editedRecord.Description;
        if (!ModelState.IsValid) {
            return BadRequest(error: ModelState);
        }
        await recordService.EditRecordByIdAsync(id, editedRecord);
        return Ok(recordToEdit);
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
    
}