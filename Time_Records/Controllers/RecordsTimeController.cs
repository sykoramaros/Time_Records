using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Time_Records.DTO;
using Time_Records.Services;

namespace Time_Records.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RecordsTimeController : ControllerBase {
    private RecordTimeService recordTimeService;

    public RecordsTimeController(RecordTimeService recordTimeService) {
        this.recordTimeService = recordTimeService;
    }
    
    
    [HttpGet("SumActualMinistryYearTotalRecordTimeQuery")]
public async Task<ActionResult<TimeFormatDto>> SumActualMinistryYearTotalRecordTimeQueryAsync([FromQuery] Guid userId) {
    var sum = await recordTimeService.SumActualMinistryYearTotalRecordTimeQueryAsync(userId);
    if (sum == null) {
        return NotFound("No records found");
    }
    return Ok(sum);
}

[HttpGet("YearRecordProgressQuery")]
public async Task<ActionResult<double>> YearRecordProgressQueryAsync([FromQuery] Guid userId) {
    var yearPercentageProgress = await recordTimeService.YearRecordProgressQueryAsync(userId);
    if (double.IsInfinity(yearPercentageProgress)) {
        return BadRequest("The result is Infinity, which is not valid.");
    }
    return Ok(yearPercentageProgress);
}

[HttpGet("YearRemainingTimeQuery")]
public async Task<ActionResult<TimeFormatDto>> YearRemainingTimeQueryAsync([FromQuery] Guid userId) {
    var remainingTime = await recordTimeService.YearRemainingTimeQueryAsync(userId);
    return Ok(remainingTime);
}

[HttpGet("SumActualMonthTotalRecordTimeQuery")]
public async Task<ActionResult<TimeFormatDto>> SumActualMonthTotalRecordTimeQueryAsync([FromQuery] Guid userId) {
    var sum = await recordTimeService.SumActualMonthTotalRecordTimeQueryAsync(userId);
    if (sum == null) {
        return NotFound("No records found");
    }
    return Ok(sum);
}

[HttpGet("MonthRecordProgressQuery")]
public async Task<ActionResult<double>> MonthRecordProgressQueryAsync([FromQuery] Guid userId) {
    var monthPercentageProgress = await recordTimeService.MonthRecordProgressQueryAsync(userId);
    if (double.IsInfinity(monthPercentageProgress)) {
        return BadRequest("The result is Infinity, which is not valid.");
    }
    return Ok(monthPercentageProgress);
}

[HttpGet("MonthRemainingTimeQuery")]
public async Task<ActionResult<TimeFormatDto>> MonthRemainingTimeQueryAsync([FromQuery] Guid userId) {
    var remainingTime = await recordTimeService.MonthRemainingTimeQueryAsync(userId);
    return Ok(remainingTime);
}

[HttpGet("SumActualWeekTotalRecordTimeQuery")]
public async Task<ActionResult<TimeFormatDto>> SumActualWeekTotalRecordTimeQueryAsync([FromQuery] Guid userId) {
    var sum = await recordTimeService.SumActualWeekTotalRecordTimeQueryAsync(userId);
    if (sum == null) {
        return NotFound("No records found");
    }
    return Ok(sum);
}

[HttpGet("WeekRecordProgressQuery")]
public async Task<ActionResult<double>> WeekRecordProgressQueryAsync([FromQuery] Guid userId) {
    var weekPercentageProgress = await recordTimeService.WeekRecordProgressQueryAsync(userId);
    if (double.IsInfinity(weekPercentageProgress)) {
        return BadRequest("The result is Infinity, which is not valid.");
    }
    return Ok(weekPercentageProgress);
}

[HttpGet("WeekRemainingTimeQuery")]
public async Task<ActionResult<TimeFormatDto>> WeekRemainingTimeQueryAsync([FromQuery] Guid userId) {
    var remainingTime = await recordTimeService.WeekRemainingTimeQueryAsync(userId);
    return Ok(remainingTime);
}

[HttpGet("GetMonthTimeGoalQuery")]
public async Task<ActionResult<int>> GetMonthTimeGoalQueryAsync([FromQuery] Guid userId) {
    var monthTimeGoal = await recordTimeService.GetMonthTimeGoalAsyncQuery(userId);
    if (monthTimeGoal == null) {
        return BadRequest("Month time goal cannot be zero");
    }
    return Ok(monthTimeGoal);
}

[HttpGet("SumChosenMonthTotalRecordTimeQuery")]
public async Task<ActionResult<TimeFormatDto>> SumChosenMonthTotalRecordTimeQueryAsync([FromQuery] Guid userId, [FromQuery] int chosenMonth, [FromQuery] int chosenYear) {
    var sum = await recordTimeService.SumChosenMonthTotalRecordTimeQueryAsync(userId, chosenMonth, chosenYear);
    if (sum == null) {
        return NotFound("No records found");
    }

    return Ok(sum);
}

    
    // [HttpGet("SumActualMinistryYearTotalRecordTimeQuery")]
    // public async Task<ActionResult<TimeFormatDto>> SumActualMinistryYearTotalRecordTimeQueryAsync([FromQuery] string userId) {
    //     var sum = await recordTimeService.SumActualMinistryYearTotalRecordTimeQueryAsync(userId);
    //     if (sum == null) {
    //         return NotFound("No records found");
    //     }
    //     return Ok(sum);
    // }
    // [HttpGet("YearRecordProgressQuery")]
    // public async Task<ActionResult<double>> YearRecordProgressQueryAsync([FromQuery] string userId) {
    //     var yearPercentageProgress = await recordTimeService.YearRecordProgressQueryAsync(userId);
    //     if (double.IsInfinity(yearPercentageProgress)) {
    //         return BadRequest("The result is Infinity, which is not valid.");
    //     }
    //
    //     return Ok(yearPercentageProgress);
    // }
    // [HttpGet("YearRemainingTimeQuery")]
    // public async Task<ActionResult<TimeFormatDto>> YearRemainingTimeQueryAsync([FromQuery] string userId) {
    //     var remainingTime = await recordTimeService.YearRemainingTimeQueryAsync(userId);
    //     return Ok(remainingTime);
    // }
    // [HttpGet("SumActualMonthTotalRecordTimeQuery")]
    // public async Task<ActionResult<TimeFormatDto>> SumActualMonthTotalRecordTimeQueryAsync([FromQuery] string userId) {
    //     var sum = await recordTimeService.SumActualMonthTotalRecordTimeQueryAsync(userId);
    //     if (sum == null) {
    //         return NotFound("No records found");
    //     }
    //     return Ok(sum);
    // }
    // [HttpGet("MonthRecordProgressQuery")]
    // public async Task<ActionResult<double>> MonthRecordProgressQueryAsync([FromQuery] string userId) {
    //     var monthPercentageProgress = await recordTimeService.MonthRecordProgressQueryAsync(userId);
    //     if (double.IsInfinity(monthPercentageProgress)) {
    //         return BadRequest("The result is Infinity, which is not valid.");
    //     }
    //
    //     return Ok(monthPercentageProgress);
    // }
    // [HttpGet("MonthRemainingTimeQuery")]
    // public async Task<ActionResult<TimeFormatDto>> MonthRemainingTimeQueryAsync([FromQuery] string userId) {
    //     var remainingTime = await recordTimeService.MonthRemainingTimeQueryAsync(userId);
    //     return Ok(remainingTime);
    // }
    // [HttpGet("SumActualWeekTotalRecordTimeQuery")]
    // public async Task<ActionResult<TimeFormatDto>> SumActualWeekTotalRecordTimeQueryAsync([FromQuery] string userId) {
    //     var sum = await recordTimeService.SumActualWeekTotalRecordTimeQueryAsync(userId);
    //     if (sum == null) {
    //         return NotFound("No records found");
    //     }
    //     return Ok(sum);
    // }
    //
    // [HttpGet("WeekRecordProgressQuery")]
    // public async Task<ActionResult<double>> WeekRecordProgressQueryAsync([FromQuery] string userId) {
    //     var weekPercentageProgress = await recordTimeService.WeekRecordProgressQueryAsync(userId);
    //     if (double.IsInfinity(weekPercentageProgress)) {
    //         return BadRequest("The result is Infinity, which is not valid.");
    //     }
    //     return Ok(weekPercentageProgress);
    // }
    // [HttpGet("WeekRemainingTimeQuery")]
    // public async Task<ActionResult<TimeFormatDto>> WeekRemainingTimeQueryAsync([FromQuery] string userId) {
    //     var remainingTime = await recordTimeService.WeekRemainingTimeQueryAsync(userId);
    //     return Ok(remainingTime);
    // }
    //
    // [HttpGet("GetMonthTimeGoalQuery")]
    // public async Task<ActionResult<int>> GetMonthTimeGoalQueryAsync([FromQuery] string userId) {
    //     var monthTimeGoal = await recordTimeService.GetMonthTimeGoalAsyncQuery(userId);
    //     if (monthTimeGoal == null) {
    //         return BadRequest("Month time goal cannot be zero");
    //     }
    //     return Ok(monthTimeGoal);
    // }

    // [HttpGet("SumTotalRecordTime")]
    // public async Task<ActionResult<(int hours, int minutes)>> SumTotalRecordTimeAsync() {
    //     var sum = await recordTimeService.SumTotalRecordTimeAsync();
    //     return Ok(sum);
    // }
    //
    // [HttpGet("SumActualMinistryYearTotalRecordTime")]
    // public async Task<ActionResult<(int hours, int minutes)>> SumActualMinistryYearTotalRecordTimeAsync() {
    //     var sum = await recordTimeService.SumActualMinistryYearTotalRecordTimeAsync();
    //     return Ok(sum);
    // }
    // [HttpGet("YearRecordProgress")]
    // public async Task<ActionResult<double>> YearRecordProgressAsync() {
    //     var yearPercentageProgress = await recordTimeService.YearRecordProgressAsync();
    //     return Ok(yearPercentageProgress);
    // }
    
    
    // [HttpGet("YearRemainingTime")]
    // public async Task<ActionResult<(int hours, int minutes)>> YearRemainingTimeAsync() {
    //     var remainingTime = await recordTimeService.YearRemainingTimeAsync();
    //     return Ok(remainingTime);
    // }
    
    
    // [HttpGet("SumActualMonthTotalRecordTime")]
    // public async Task<ActionResult<(int hours, int minutes)>> SumActualMonthTotalRecordTimeAsync() {
    //     var sum = await recordTimeService.SumActualMonthTotalRecordTimeAsync();
    //     return Ok(sum);
    // }
    

    // [HttpGet("MonthRecordProgress")]
    // public async Task<ActionResult<double>> MonthRecordProgressAsync() {
    //     var monthPercentageProgress = await recordTimeService.MonthRecordProgressAsync();
    //     return Ok(monthPercentageProgress);
    // }
    
    
    // [HttpGet("MonthRemainingTime")]
    // public async Task<ActionResult<(int hours, int minutes)>> MonthRemainingTimeAsync() {
    //     var remainingTime = await recordTimeService.MonthRemainingTimeAsync();
    //     return Ok(remainingTime);
    // }
    
    
    // [HttpGet("SumMonthTotalRecordTime")]
    // public async Task<ActionResult<(int hours, int minutes)>> SumMonthTotalRecordTimeAsync(DateOnly chosenMonthYear) {
    //     var sum = await recordTimeService.SumMonthTotalRecordTimeAsync(chosenMonthYear);
    //     return Ok(sum);
    // }
    
    // [HttpGet("SumActualWeekTotalRecordTime")]
    // public async Task<ActionResult<(int hours, int minutes)>> SumActualWeekTotalRecordTimeAsync() {
    //     var sum = await recordTimeService.SumActualWeekTotalRecordTimeAsync();
    //     return Ok(sum);
    // }
    
    // [HttpGet("WeekRecordProgress")]
    // public async Task<ActionResult<double>> WeekRecordProgressAsync() {
    //     var weekPercentageProgress = await recordTimeService.WeekRecordProgressAsync();
    //     return Ok(weekPercentageProgress);
    // }
    
    
    // [HttpGet("WeekRemainingTime")]
    // public async Task<ActionResult<(int hours, int minutes)>> WeekRemainingTimeAsync() {
    //     var remainingTime = await recordTimeService.WeekRemainingTimeAsync();
    //     return Ok(remainingTime);
    // }
    
    
    // [HttpGet("SumWeekTotalRecordTime")]
    // public async Task<ActionResult<(int hours, int minutes)>> SumWeekTotalRecordTimeAsync(DateOnly chosenDay) {
    //     var sum = await recordTimeService.SumWeekTotalRecordTimeAsync(chosenDay);
    //     return Ok(sum);
    // }
}