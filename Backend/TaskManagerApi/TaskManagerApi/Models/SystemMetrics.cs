namespace TaskManagerApi.Models;

public class SystemMetrics
{
    public double CpuUsagePercent { get; set; }

    public double MemoryUsageMB { get; set; }

    public double AvailableMemoryMB { get; set; }

    public double MemoryUsagePercent { get; set; }

    public double ProcessCount { get; set; }

    public double ThreadCount { get; set; }    

    public double WorkingSetMB { get; set; }

    public DateTime LastUpdated { get; set; }

    public string MachineName { get; set; } = string.Empty;
}
