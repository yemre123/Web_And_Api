namespace WebUI.Models;

public class FanSchedulePageViewModel
{
    public List<FanScheduleInputModel> Items { get; set; } = new();
    public FanScheduleInputModel NewItem { get; set; } = new();
}
