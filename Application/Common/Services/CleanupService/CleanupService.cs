using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Services.CleanupService
{
    public class CleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromMinutes(5);

        public CleanupService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                await CleanUpExpiredRecordsAsync();

                await Task.Delay(_cleanupInterval, stoppingToken);
            }
        }

        private async Task CleanUpExpiredRecordsAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

                var expiredEntries = dbContext.PendingVerifications
                    .Where(pv => pv.Expiration < DateTime.UtcNow);

                dbContext.PendingVerifications.RemoveRange(expiredEntries);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
