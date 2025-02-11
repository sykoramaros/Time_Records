using Microsoft.AspNetCore.Mvc;
using Time_Records.Services;

namespace Time_Records.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudiesController : ControllerBase {
    private readonly StudyService studyService;


    public StudiesController(StudyService studyService) {
        this.studyService = studyService;
    }

    [HttpGet("GetSumActualMonthRecordStudyQuery")]
    public async Task<ActionResult<int>> GetSumActualMonthRecordStudyQuery(string userId) {
        var actualMonthStudies = await studyService.SumActualMonthRecordStudyQuery(userId);
        if (actualMonthStudies == null) {
            return NotFound("No records found");
        }
        return Ok(actualMonthStudies);
    }

    [HttpGet("GetSumActualWeekRecordStudyQuery")]
    public async Task<ActionResult<int>> GetSumActualWeekRecordStudyQuery(string userId) {
        var actualWeekStudies = await studyService.SumActualWeekRecordStudyQuery(userId);
        if (actualWeekStudies == null) {
            return NotFound("No records found");
        }

        return Ok(actualWeekStudies);
    }
    
    
    
}