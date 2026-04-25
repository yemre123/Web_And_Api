using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebUI.Configuration;
using WebUI.Models;
using WebUI.Services;

namespace WebUI.Controllers;

public class FanSchedulesController : Controller
{
    private readonly FanScheduleApiService _apiService;
    private readonly FanScheduleUiOptions _options;

    public FanSchedulesController(FanScheduleApiService apiService, IOptions<FanScheduleUiOptions> options)
    {
        _apiService = apiService;
        _options = options.Value;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var vm = new FanSchedulePageViewModel
        {
            Items = await _apiService.GetAllAsync(cancellationToken),
            NewItem = new FanScheduleInputModel()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind(Prefix = "NewItem")] FanScheduleInputModel input, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return await ReloadWithModelAsync(input, cancellationToken);
        }

        if (string.IsNullOrWhiteSpace(input.NewRowPassword) ||
            !string.Equals(input.NewRowPassword, _options.NewRowPassword, StringComparison.Ordinal))
        {
            ModelState.AddModelError(nameof(FanScheduleInputModel.NewRowPassword), "Password is incorrect.");
            return await ReloadWithModelAsync(input, cancellationToken);
        }

        var (isSuccess, error) = await _apiService.CreateAsync(input, cancellationToken);
        if (!isSuccess)
        {
            ModelState.AddModelError(string.Empty, error);
            return await ReloadWithModelAsync(input, cancellationToken);
        }

        TempData["SuccessMessage"] = "New schedule added.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(FanScheduleInputModel input, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Invalid form values.";
            return RedirectToAction(nameof(Index));
        }

        var (isSuccess, error) = await _apiService.UpdateAsync(input, cancellationToken);
        if (!isSuccess)
        {
            TempData["ErrorMessage"] = error;
            return RedirectToAction(nameof(Index));
        }

        TempData["SuccessMessage"] = $"{input.DeviceName} updated.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<IActionResult> ReloadWithModelAsync(FanScheduleInputModel input, CancellationToken cancellationToken)
    {
        var vm = new FanSchedulePageViewModel
        {
            Items = await _apiService.GetAllAsync(cancellationToken),
            NewItem = input
        };

        return View("Index", vm);
    }
}
