using Microsoft.AspNetCore.Mvc;
using TaskManagerApi.Interfaces;

namespace TaskManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatsController : ControllerBase
{
    private readonly ILogger<StatsController> _logger;
    private readonly ISystemMetricsService _metricsService;

    public StatsController(ILogger<StatsController> logger, ISystemMetricsService metricsService)
    {
        _logger = logger;
        _metricsService = metricsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetStatsAsync()
    {
        try
        {
            var totalTasks = await _metricsService.GetTotalTasksAsync();
            var completedTasks = await _metricsService.GetCompletedTasksAsync();
            var todaysTasks = await _metricsService.GetTodaysTasksCountAsync();
            var systemMetrics = await _metricsService.GetSystemMetricsAsync();

            var completionRate = totalTasks > 0 ? Math.Round((completedTasks * 100f) / totalTasks, 1) : 0f;

            return Ok(new
            {
                totalTasks,
                completedTasks,
                pendingTasks = totalTasks - completedTasks,
                completionRate,
                todaysTasks,
                systemMetrics
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch metrics");
            return StatusCode(500, new { message = "Failed to fetch metrics" });
        }
    }
}
