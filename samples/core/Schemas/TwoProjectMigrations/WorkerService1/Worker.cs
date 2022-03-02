using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WorkerService1;

internal class Worker : IHostedService, IDisposable
{
    private readonly IServiceProvider _services;

    private Timer _timer;

    public Worker(IServiceProvider services)
        => _services = services;

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _timer = new Timer(
            state => Execute(),
            state: null,
            dueTime: TimeSpan.Zero,
            period: TimeSpan.FromDays(1));

        return Task.CompletedTask;
    }

    private void Execute()
    {
        using var scope = _services.CreateScope();
        var services = scope.ServiceProvider;

        var db = services.GetService<BlogContext>();

        // TODO: Something interesting
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
        => _timer?.Dispose();
}