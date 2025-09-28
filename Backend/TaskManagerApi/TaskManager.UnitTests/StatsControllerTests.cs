using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagerApi.Controllers;
using TaskManagerApi.Interfaces;
using TaskManagerApi.Models;
using Xunit;

namespace TaskManager.UnitTests;

public class StatsControllerTests
{
    [Fact]
    public async Task GetStatsAsync_ReturnsOk()
    {
        var metricsMock = new Mock<ISystemMetricsService>();
        metricsMock.Setup(m => m.GetTotalTasksAsync()).ReturnsAsync(10);
        metricsMock.Setup(m => m.GetCompletedTasksAsync()).ReturnsAsync(5);
        metricsMock.Setup(m => m.GetTodaysTasksCountAsync()).ReturnsAsync(2);
        metricsMock.Setup(m => m.GetSystemMetricsAsync()).ReturnsAsync(new SystemMetrics
        {
            CpuUsagePercent = 50,
            MemoryUsageMB = 500,
            AvailableMemoryMB = 1024,
            MemoryUsagePercent = 33.3f,
            ProcessCount = 100,
            ThreadCount = 500,
            WorkingSetMB = 64,
            MachineName = "TEST-PC"
        });

        var loggerMock = new Mock<ILogger<StatsController>>();
        var controller = new StatsController(loggerMock.Object, metricsMock.Object);

        var result = await controller.GetStatsAsync();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }
}
