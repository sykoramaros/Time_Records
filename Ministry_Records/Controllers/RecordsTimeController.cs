using Microsoft.AspNetCore.Mvc;
using Ministry_Records.Services;

namespace Ministry_Records.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RecordsTimeController : ControllerBase {
    private RecordTimeService recordTimeService;

    public RecordsTimeController(RecordTimeService recordTimeService) {
        this.recordTimeService = recordTimeService;
    }
    
    [HttpGet("SumTotalRecordTime")]
    public async Task<ActionResult<(int hours, int minutes)>> SumTotalRecordTimeAsync() {
        var sum = await recordTimeService.SumTotalRecordTimeAsync();
        return Ok(sum);
    }
    
    [HttpGet("SumMonthTotalRecordTime")]
    public async Task<ActionResult<(int hours, int minutes)>> SumMonthTotalRecordTimeAsync(DateOnly chosenMonthYear) {
        var sum = await recordTimeService.SumMonthTotalRecordTimeAsync(chosenMonthYear);
        return Ok(sum);
    }
}