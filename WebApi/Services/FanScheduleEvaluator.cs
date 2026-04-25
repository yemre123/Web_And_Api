using WebApi.Models;

namespace WebApi.Services;

public sealed class FanScheduleEvaluationResult
{
    public required string DeviceName { get; init; }
    public required string Status { get; init; }
    public required string Reason { get; init; }
}

public interface IFanScheduleEvaluator
{
    FanScheduleEvaluationResult Evaluate(FanSchedule schedule, DateTime now);
}

public class FanScheduleEvaluator : IFanScheduleEvaluator
{
    public FanScheduleEvaluationResult Evaluate(FanSchedule schedule, DateTime now)
    {
        if (!schedule.IsEnabled)
        {
            return CreateResult(schedule.DeviceName, "Stop", "Schedule disabled");
        }

        //if (schedule.ColForceStop)
        //{
        //    return CreateResult(schedule.DeviceName, "Stop", "Forced stop");
        //}

        var currentTime = TimeOnly.FromDateTime(now);
        if (IsInsideStopWindow(currentTime, schedule.StopStartTime, schedule.StopEndTime))
        {
            return CreateResult(schedule.DeviceName, "Stop", "Inside stop window");
        }

        var cycleLength = schedule.RunSeconds + schedule.StopSeconds;
        if (cycleLength <= 0)
        {
            return CreateResult(schedule.DeviceName, "Stop", "Invalid cycle length");
        }

        var elapsedSecondsFromMidnight = now.TimeOfDay.TotalSeconds;
        var cycleOffset = (int)elapsedSecondsFromMidnight % cycleLength;
        var isRunning = cycleOffset < schedule.RunSeconds;

        return isRunning
            ? CreateResult(schedule.DeviceName, "Run", "Inside run period")
            : CreateResult(schedule.DeviceName, "Stop", "Inside cycle stop period");
    }

    private static bool IsInsideStopWindow(TimeOnly now, TimeOnly start, TimeOnly end)
    {
        return start < end
            ? now >= start && now < end
            : now >= start || now < end;
    }

    private static FanScheduleEvaluationResult CreateResult(string deviceName, string status, string reason)
    {
        return new FanScheduleEvaluationResult
        {
            DeviceName = deviceName,
            Status = status,
            Reason = reason
        };
    }
}
