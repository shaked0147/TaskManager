namespace TaskManagerApi.Models;

public class MetricStat
{
    public int TotalTasks { get; set; }

    public int CompletedTasks { get; set; }

    public int PendingTasks { get; set; }

    public double CompletionRate { get; set; }

    public int TodaysTasks { get; set; }

    public SystemMetrics SystemMetrics { get; set; } = new();
}
