using Microsoft.AspNetCore.Authorization;
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

    [HttpGet("GetSumActualMinistryYearRecordStudyQuery")]
    public async Task<ActionResult<int>> GetSumActualMinistryYearRecordStudyQuery(Guid userId) {
        var actualMinistryYearStudies = await studyService.SumActualMinistryYearRecordStudyQuery(userId);
        if (actualMinistryYearStudies == null) {
            return NotFound("No records found");
        }
        return actualMinistryYearStudies;
    }
    

    [HttpGet("GetSumActualMonthRecordStudyQuery")]
    public async Task<ActionResult<int>> GetSumActualMonthRecordStudyQuery(Guid userId) {
        var actualMonthStudies = await studyService.SumActualMonthRecordStudyQuery(userId);
        if (actualMonthStudies == null) {
            return NotFound("No records found");
        }
        return Ok(actualMonthStudies);
    }
    
    [HttpGet("GetSumChosenMonthRecordStudyQuery")]
    public async Task<ActionResult<int>> GetSumChosenMonthRecordStudyQuery(Guid userId, int chosenMonth, int chosenYear) {
        var actualMonthStudies = await studyService.SumChosenMonthRecordStudyQuery( userId, chosenMonth, chosenYear);
        if (actualMonthStudies == null) {
            return NotFound("No records found");
        }
        return Ok(actualMonthStudies);
    }

        
    [HttpGet("GetSumActualWeekRecordStudyQuery")]
    public async Task<ActionResult<int>> GetSumActualWeekRecordStudyQuery(Guid userId) {
        var actualWeekStudies = await studyService.SumActualWeekRecordStudyQuery(userId);
        if (actualWeekStudies == null) {
            return NotFound("No records found");
        }

        return Ok(actualWeekStudies);
    }

    [HttpGet("GetActualMinistryYearAverageStudyQuery")]
    public async Task<ActionResult<double>> GetActualMinistryYearAverageStudyQuery(Guid userId) {
        var averageActualMinistryYearStudies = await studyService.ActualMinistryYearAverageRecordStudyQuery(userId);
        if (averageActualMinistryYearStudies == null) {
            return NotFound("No study found in records");
        }
        return averageActualMinistryYearStudies;
    }
    
    
}