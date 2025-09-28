using TaskManagerApi.Models;

namespace TaskManagerApi.Interfaces;

public interface ISystemMetricsService
{
    Task<SystemMetrics> GetSystemMetricsAsync();
    Task<int> GetTodaysTasksCountAsync();
    Task<int> GetTotalTasksAsync();
    Task<int> GetCompletedTasksAsync();
}
