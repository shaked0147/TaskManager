using Microsoft.Win32.TaskScheduler;
using System.Diagnostics;
using System.Management;
using TaskManagerApi.Interfaces;
using TaskManagerApi.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskManagerApi.Services;

public class SystemMetricsService : ISystemMetricsService
{
    public async Task<SystemMetrics> GetSystemMetricsAsync()
    {
        return await Task.Run(() =>
        {
            // --- CPU Usage ---
            using var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuCounter.NextValue();
            System.Threading.Thread.Sleep(500);
            float cpuUsage = cpuCounter.NextValue();

            // --- Memory Usage ---
            int availableMemoryMB = GetAvailableMemoryMB();
            int totalMemoryMB = GetTotalPhysicalMemoryMB();
            int usedMemoryMB = totalMemoryMB - availableMemoryMB;
            float memoryUsagePercent = totalMemoryMB > 0
                ? MathF.Round((usedMemoryMB * 100f) / totalMemoryMB, 1)
                : 0f;

            // --- Processes & Threads ---
            var allProcesses = Process.GetProcesses();
            int processCount = allProcesses.Length;
            int threadCount = allProcesses.Sum(p => p.Threads.Count);
            int workingSetMB = (int)(Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024);

            return new SystemMetrics
            {
                CpuUsagePercent = cpuUsage,
                MemoryUsageMB = usedMemoryMB,
                AvailableMemoryMB = availableMemoryMB,
                MemoryUsagePercent = memoryUsagePercent,
                ProcessCount = processCount,
                ThreadCount = threadCount,
                WorkingSetMB = workingSetMB,
                LastUpdated = DateTime.UtcNow,
                MachineName = Environment.MachineName
            };
        });
    }

    private int GetTotalPhysicalMemoryMB()
    {
        using var searcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");
        var totalMemoryBytes = Convert.ToInt64(searcher.Get().Cast<ManagementObject>().First()["TotalPhysicalMemory"]);
        return (int)(totalMemoryBytes / 1024 / 1024);
    }

    private int GetAvailableMemoryMB()
    {
        using var ramCounter = new PerformanceCounter("Memory", "Available MBytes");
        return (int)ramCounter.NextValue();
    }

    public async Task<int> GetTotalTasksAsync()
    {
        return await Task.Run(() =>
        {
            using var ts = new TaskService();
            return ts.AllTasks.Count();
        });
    }

    public async Task<int> GetCompletedTasksAsync()
    {
        return await Task.Run(() =>
        {
            using var ts = new TaskService();
            return ts.AllTasks.Count(t => t.LastTaskResult == 0);
        });
    }

    public async Task<int> GetTodaysTasksCountAsync()
    {
        return await Task.Run(() =>
        {
            using var ts = new TaskService();
            var today = DateTime.Today;
            int count = 0;

            foreach (var task in ts.AllTasks)
            {
                try
                {
                    var regDate = task.Definition?.RegistrationInfo?.Date;
                    if (regDate != null && regDate?.Date == today)
                    {
                        count++;
                    }
                }
                catch
                {
                    // ignore invalid or system tasks
                    continue;
                }
            }

            return count;
        });
    }
}



