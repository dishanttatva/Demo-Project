using ExpenseTrackerEntity.Models;
using ExpenseTrackerRepository.Interface;
using ExpenseTrackerService.Interface;
using iText.Bouncycastle.Crypto;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                    if(recurrence != null)
                    {
                    scopedProcessingService.TriggerAlert(recurrence);
                    }
                }
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}

