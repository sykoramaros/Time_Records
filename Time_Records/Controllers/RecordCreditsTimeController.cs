using Microsoft.AspNetCore.Mvc;
using Time_Records.DTO;
using Time_Records.Services;

namespace Time_Records.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RecordCreditsTimeController : ControllerBase{
    private RecordCreditTimeService recordCreditTimeService;

    public RecordCreditsTimeController(RecordCreditTimeService recordCreditTimeService) {
        this.recordCreditTimeService = recordCreditTimeService;
    }
    
    [HttpGet("SumActualMinistryYearTotalRecordCreditTimeQuery")]
    public async Task<ActionResult<TimeFormatDto>> SumActualMinistryYearTotalRecordCreditTimeQueryAsync([FromQuery] Guid userId) {
        var sum = await recordCreditTimeService.SumActualMinistryYearTotalRecordCreditTimeQueryAsync(userId);
        if (sum == null) {
            return NotFound("No records found");
        }
        return Ok(sum);
    }
    
    [HttpGet("SumActualMonthTotalRecordCreditTimeQuery")]
    public async Task<ActionResult<TimeFormatDto>> SumActualMonthTotalRecordCreditTimeQueryAsync([FromQuery] Guid userId) {
        var sum = await recordCreditTimeService.SumActualMonthTotalRecordCreditTimeQueryAsync(userId);
        if (sum == null) {
            return NotFound("No records found");
        }
        return Ok(sum);
    }
    
    [HttpGet("SumActualWeekTotalRecordCreditTimeQuery")]
    public async Task<ActionResult<TimeFormatDto>> SumActualWeekTotalRecordCreditTimeQueryAsync([FromQuery] Guid userId) {
        var sum = await recordCreditTimeService.SumActualWeekTotalRecordCreditTimeQueryAsync(userId);
        if (sum == null) {
            return NotFound("No records found");
        }
        return Ok(sum);
    }
    
    [HttpGet("SumChosenMonthTotalRecordCreditTimeQuery")]
    public async Task<ActionResult<TimeFormatDto>> SumChosenMonthTotalRecordCreditTimeQueryAsync([FromQuery] Guid userId, [FromQuery] int chosenMonth, [FromQuery] int chosenYear) {
        var sum = await recordCreditTimeService.SumChosenMonthTotalRecordCreditTimeQueryAsync(userId, chosenMonth, chosenYear);
        if (sum == null) {
            return NotFound("No records found");
        }
        return Ok(sum);
    }
}