import { config } from './config.js';

const token = localStorage.getItem('token');
console.log(localStorage.getItem('token'));

if (!token) window.location.href = 'login.html';

document.getElementById('logoutBtn').addEventListener('click', () => {
    localStorage.removeItem('token');
    window.location.href = 'login.html';
});

const tasksChartCtx = document.getElementById('tasksChart').getContext('2d');
const cpuMemoryChartCtx = document.getElementById('cpuMemoryChart').getContext('2d');

const tasksChart = new Chart(tasksChartCtx, {
    type: 'doughnut',
    data: {
        labels: ['Completed', 'Pending'],
        datasets: [{
            label: 'Tasks',
            data: [0, 0],
            backgroundColor: ['#28a745', '#ffc107'],
            borderWidth: 1
        }]
    }
});

const cpuMemoryChart = new Chart(cpuMemoryChartCtx, {
    type: 'bar',
    data: {
        labels: ['CPU Usage', 'Memory Usage'],
        datasets: [{
            label: 'Percent',
            data: [0, 0],
            backgroundColor: ['#007bff', '#17a2b8'],
        }]
    },
    options: {
        scales: {
            y: { beginAtZero: true, max: 100 }
        }
    }
});

async function fetchStats() {
    try {
        const response = await fetch(`${config.apiBaseUrl}/stats`, {
            headers: { 'Authorization': 'Bearer ' + token }
        });
        if (!response.ok) throw new Error('Failed to fetch stats');

        const data = await response.json();

        document.getElementById('totalTasks').textContent = data.totalTasks;
        document.getElementById('completedTasks').textContent = data.completedTasks;
        document.getElementById('pendingTasks').textContent = data.pendingTasks;
        document.getElementById('todaysTasks').textContent = data.todaysTasks;
        document.getElementById('completionRate').textContent = data.completionRate + '%';

        tasksChart.data.datasets[0].data = [data.completedTasks, data.pendingTasks];
        tasksChart.update();

        const sys = data.systemMetrics;
        document.getElementById('cpuUsagePercent').textContent = sys.cpuUsagePercent.toFixed(3) + '%';
        document.getElementById('memoryUsageMB').textContent = sys.memoryUsageMB;
        document.getElementById('availableMemoryMB').textContent = sys.availableMemoryMB;
        document.getElementById('memoryUsagePercent').textContent = sys.memoryUsagePercent.toFixed(3) + '%';
        document.getElementById('processCount').textContent = sys.processCount;
        document.getElementById('threadCount').textContent = sys.threadCount;
        document.getElementById('workingSetMB').textContent = sys.workingSetMB;
        document.getElementById('lastUpdated').textContent = new Date(sys.lastUpdated).toLocaleString();
        document.getElementById('machineName').textContent = sys.machineName;

        cpuMemoryChart.data.datasets[0].data = [sys.cpuUsagePercent, sys.memoryUsagePercent];
        cpuMemoryChart.update();

    } catch (err) {
        console.error(err);
        alert('Error fetching stats: ' + err.message);
    }
}

// fetch every 5 seconds
fetchStats();
setInterval(fetchStats, 5000);
