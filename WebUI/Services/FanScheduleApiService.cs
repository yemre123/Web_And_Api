using System.Net.Http.Json;
using WebUI.Models;

namespace WebUI.Services;

public class FanScheduleApiService
{
    private readonly HttpClient _httpClient;

    public FanScheduleApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<FanScheduleInputModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        List<FanScheduleInputModel>? result = null;
        try
        {
            result = await _httpClient.GetFromJsonAsync<List<FanScheduleInputModel>>(
                "api/FanSchedules",
                cancellationToken);
        }
        catch
        {
            return new List<FanScheduleInputModel>();
        }

        var list = result ?? new List<FanScheduleInputModel>();
        foreach (var item in list)
        {
            item.StopStartTime = FormatForTimeInput(item.StopStartTime);
            item.StopEndTime = FormatForTimeInput(item.StopEndTime);
        }

        return list;
    }

    public async Task<(bool IsSuccess, string Error)> CreateAsync(FanScheduleInputModel model, CancellationToken cancellationToken = default)
    {
        var payload = ToPayload(model);
        var response = await _httpClient.PostAsJsonAsync("api/FanSchedules", payload, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            return (true, string.Empty);
        }

        var message = await response.Content.ReadAsStringAsync(cancellationToken);
        return (false, $"Create failed: {message}");
    }

    public async Task<(bool IsSuccess, string Error)> UpdateAsync(FanScheduleInputModel model, CancellationToken cancellationToken = default)
    {
        var payload = ToPayload(model);
        var response = await _httpClient.PutAsJsonAsync($"api/FanSchedules/{model.DeviceName}", payload, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            return (true, string.Empty);
        }

        var message = await response.Content.ReadAsStringAsync(cancellationToken);
        return (false, $"Update failed: {message}");
    }

    private static object ToPayload(FanScheduleInputModel model)
    {
        return new
        {
            model.DeviceName,
            StopStartTime = NormalizeTime(model.StopStartTime),
            StopEndTime = NormalizeTime(model.StopEndTime),
            model.RunSeconds,
            model.StopSeconds,
            //model.ColForceStop,
            model.IsEnabled
        };
    }

    private static string NormalizeTime(string input)
    {
        if (!TimeOnly.TryParse(input, out var value))
        {
            return input;
        }

        return value.ToString("HH:mm:ss");
    }

    private static string FormatForTimeInput(string input)
    {
        if (!TimeOnly.TryParse(input, out var value))
        {
            return input;
        }

        return value.ToString("HH:mm");
    }
}
