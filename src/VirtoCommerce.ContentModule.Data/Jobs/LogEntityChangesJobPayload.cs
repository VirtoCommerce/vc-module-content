using VirtoCommerce.Platform.Core.ChangeLog;

namespace VirtoCommerce.ContentModule.Data.Jobs
{
    /// <summary>
    /// Payload of the background job that persists menu link list change-log entries.
    /// </summary>
    public class LogEntityChangesJobPayload
    {
        public OperationLog[] OperationLogs { get; set; }
    }
}
