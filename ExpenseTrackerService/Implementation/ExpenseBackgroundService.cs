using ExpenseTrackerEntity.Models;
using ExpenseTrackerService.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
namespace ExpenseTrackerService.Implimentation
{
    public class ExpenseBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        public ExpenseBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {


                using (IServiceScope scope = _serviceProvider.CreateScope())
                {
                    IExpenseService scopedProcessingService =
                        scope.ServiceProvider.GetRequiredService<IExpenseService>();
                    Recurrence recurrence = scopedProcessingService.CheckDueDate();
                    if (recurrence != null)
                    {
                        scopedProcessingService.TriggerAlert(recurrence);
                    }
                }
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}

