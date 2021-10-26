using Microsoft.Extensions.Logging;
using Optsol.Components.Storage.Queue;
using Optsol.Components.Storage.Settings;

namespace Optsol.Components.Storage.Test.Utils.Queue
{
    public class QueueStorageTest : QueueStorageBase, IQueueStorageTest
    {
        public QueueStorageTest(StorageSettings settings, ILoggerFactory logger) : base(settings, logger)
        {
        }

        public override string QueueName { get => "teste-queue"; }
    }
}