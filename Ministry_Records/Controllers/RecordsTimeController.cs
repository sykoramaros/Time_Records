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
    
    [HttpGet("SumMinistryYearTotalRecordTime")]
    public async Task<ActionResult<(int hours, int minutes)>> SumMinistryYearTotalRecordTimeAsync() {
        var sum = await recordTimeService.SumActualMinistryYearTotalRecordTimeAsync();
        return Ok(sum);
    }

    [HttpGet("YearRecordProgress")]
    public async Task<ActionResult<double>> YearRecordProgressAsync() {
        var yearPercentageProgress = await recordTimeService.YearRecordProgressAsync();
        return Ok(yearPercentageProgress);
    }
    
    [HttpGet("YearRemainingTime")]
    public async Task<ActionResult<(int hours, int minutes)>> YearRemainingTimeAsync() {
        var remainingTime = await recordTimeService.YearRemainingTimeAsync();
        return Ok(remainingTime);
    }
    
    [HttpGet("SumActualMonthTotalRecordTime")]
    public async Task<ActionResult<(int hours, int minutes)>> SumActualMonthTotalRecordTimeAsync() {
        var sum = await recordTimeService.SumActualMonthTotalRecordTimeAsync();
        return Ok(sum);
    }

    [HttpGet("MonthRecordProgress")]
    public async Task<ActionResult<double>> MonthRecordProgressAsync() {
        var monthPercentageProgress = await recordTimeService.MonthRecordProgressAsync();
        return Ok(monthPercentageProgress);
    }
    
    [HttpGet("MonthRemainingTime")]
    public async Task<ActionResult<(int hours, int minutes)>> MonthRemainingTimeAsync() {
        var remainingTime = await recordTimeService.MonthRemainingTimeAsync();
        return Ok(remainingTime);
    }
    
    
    [HttpGet("SumMonthTotalRecordTime")]
    public async Task<ActionResult<(int hours, int minutes)>> SumMonthTotalRecordTimeAsync(DateOnly chosenMonthYear) {
        var sum = await recordTimeService.SumMonthTotalRecordTimeAsync(chosenMonthYear);
        return Ok(sum);
    }
    
    [HttpGet("SumActualWeekTotalRecordTime")]
    public async Task<ActionResult<(int hours, int minutes)>> SumActualWeekTotalRecordTimeAsync() {
        var sum = await recordTimeService.SumActualWeekTotalRecordTimeAsync();
        return Ok(sum);
    }

    [HttpGet("WeekRecordProgress")]
    public async Task<ActionResult<double>> WeekRecordProgressAsync() {
        var weekPercentageProgress = await recordTimeService.WeekRecordProgressAsync();
        return Ok(weekPercentageProgress);
    }
    
    [HttpGet("WeekRemainingTime")]
    public async Task<ActionResult<(int hours, int minutes)>> WeekRemainingTimeAsync() {
        var remainingTime = await recordTimeService.WeekRemainingTimeAsync();
        return Ok(remainingTime);
    }
    
    
    [HttpGet("SumWeekTotalRecordTime")]
    public async Task<ActionResult<(int hours, int minutes)>> SumWeekTotalRecordTimeAsync(DateOnly chosenDay) {
        var sum = await recordTimeService.SumWeekTotalRecordTimeAsync(chosenDay);
        return Ok(sum);
    }
    
    
}