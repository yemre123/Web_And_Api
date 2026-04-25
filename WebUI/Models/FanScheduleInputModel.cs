using System.ComponentModel.DataAnnotations;

namespace WebUI.Models;

public class FanScheduleInputModel : IValidatableObject
{
    [Required]
    [MaxLength(128)]
    public string DeviceName { get; set; } = string.Empty;

    [Required]
    public string StopStartTime { get; set; } = "23:00";

    [Required]
    public string StopEndTime { get; set; } = "10:00";

    [Range(1, int.MaxValue)]
    public int RunSeconds { get; set; } = 10;

    [Range(0, int.MaxValue)]
    public int StopSeconds { get; set; } = 900;

    //public bool ColForceStop { get; set; }

    public bool IsEnabled { get; set; } = true;

    [DataType(DataType.Password)]
    public string? NewRowPassword { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!TimeOnly.TryParse(StopStartTime, out var start))
        {
            yield return new ValidationResult("Invalid stop start time.", new[] { nameof(StopStartTime) });
        }

        if (!TimeOnly.TryParse(StopEndTime, out var end))
        {
            yield return new ValidationResult("Invalid stop end time.", new[] { nameof(StopEndTime) });
        }

        if (TimeOnly.TryParse(StopStartTime, out start) &&
            TimeOnly.TryParse(StopEndTime, out end) &&
            start == end)
        {
            yield return new ValidationResult(
                "Stop start and stop end times cannot be equal.",
                new[] { nameof(StopStartTime), nameof(StopEndTime) });
        }
    }
}
