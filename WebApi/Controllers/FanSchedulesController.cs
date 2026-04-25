using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FanSchedulesController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IFanScheduleEvaluator _evaluator;

    public FanSchedulesController(AppDbContext dbContext, IFanScheduleEvaluator evaluator)
    {
        _dbContext = dbContext;
        _evaluator = evaluator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FanSchedule>>> GetAll()
    {
        var items = await _dbContext.FanSchedules
            .OrderBy(x => x.DeviceName)
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("{deviceName}")]
    public async Task<ActionResult<FanSchedule>> GetByDeviceName(string deviceName)
    {
        var entity = await _dbContext.FanSchedules.FindAsync(deviceName);
        if (entity is null)
        {
            return NotFound();
        }

        return Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<FanSchedule>> Create([FromBody] FanSchedule input)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var exists = await _dbContext.FanSchedules.AnyAsync(x => x.DeviceName == input.DeviceName);
        if (exists)
        {
            ModelState.AddModelError(nameof(FanSchedule.DeviceName), "Device name already exists.");
            return ValidationProblem(ModelState);
        }

        _dbContext.FanSchedules.Add(input);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetByDeviceName), new { deviceName = input.DeviceName }, input);
    }

    [HttpPut("{deviceName}")]
    public async Task<ActionResult<FanSchedule>> Update(string deviceName, [FromBody] FanSchedule input)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (!string.Equals(deviceName, input.DeviceName, StringComparison.Ordinal))
        {
            ModelState.AddModelError(nameof(FanSchedule.DeviceName), "Route deviceName must match body deviceName.");
            return ValidationProblem(ModelState);
        }

        var entity = await _dbContext.FanSchedules.FindAsync(deviceName);
        if (entity is null)
        {
            return NotFound();
        }

        entity.StopStartTime = input.StopStartTime;
        entity.StopEndTime = input.StopEndTime;
        entity.RunSeconds = input.RunSeconds;
        entity.StopSeconds = input.StopSeconds;
        //entity.ColForceStop = input.ColForceStop;
        entity.IsEnabled = input.IsEnabled;

        await _dbContext.SaveChangesAsync();
        return Ok(entity);
    }

    [HttpDelete("{deviceName}")]
    public async Task<IActionResult> Delete(string deviceName)
    {
        var entity = await _dbContext.FanSchedules.FindAsync(deviceName);
        if (entity is null)
        {
            return NotFound();
        }

        _dbContext.FanSchedules.Remove(entity);
        await _dbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{deviceName}/status")]
    public async Task<ActionResult<FanScheduleEvaluationResult>> GetCurrentStatus(string deviceName)
    {
        var entity = await _dbContext.FanSchedules.FindAsync(deviceName);
        if (entity is null)
        {
            return NotFound();
        }

        var result = _evaluator.Evaluate(entity, DateTime.Now);
        return Ok(result);
    }
}
