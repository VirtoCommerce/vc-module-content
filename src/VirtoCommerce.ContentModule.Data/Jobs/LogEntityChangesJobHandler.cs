using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Core.Jobs;

namespace VirtoCommerce.ContentModule.Data.Jobs
{
    /// <summary>
    /// Persists the change-log entries collected by <see cref="Handlers.LogChangesChangedEventHandler"/>, off the request thread.
    /// </summary>
    public class LogEntityChangesJobHandler(IChangeLogService changeLogService) : IBackgroundJobHandler<LogEntityChangesJobPayload>
    {
        public virtual Task Execute(LogEntityChangesJobPayload payload, IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            return changeLogService.SaveChangesAsync(payload.OperationLogs);
        }
    }
}
