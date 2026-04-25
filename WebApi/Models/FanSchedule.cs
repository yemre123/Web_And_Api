using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class FanSchedule : IValidatableObject
{
    [Key]
    [Required]
    [MaxLength(128)]
    public string DeviceName { get; set; } = string.Empty;

    public TimeOnly StopStartTime { get; set; }

    public TimeOnly StopEndTime { get; set; }

    [Range(1, int.MaxValue)]
    public int RunSeconds { get; set; } = 20;

    [Range(0, int.MaxValue)]
    public int StopSeconds { get; set; } = 60;


    public bool IsEnabled { get; set; } = true;

    public string WifiName { get; set; }

    public string WifiPwd { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StopStartTime == StopEndTime)
        {
            yield return new ValidationResult(
                "StopStartTime and StopEndTime cannot be equal.",
                new[] { nameof(StopStartTime), nameof(StopEndTime) });
        }
    }
}
