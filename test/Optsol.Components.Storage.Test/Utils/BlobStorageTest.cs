using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using Optsol.Components.Storage.Blob;
using Optsol.Components.Storage.Settings;

namespace Optsol.Components.Storage.Test.Utils
{
    public class BlobStorageTest : BlobStorageBase, IBlobStorageTest
    {
        public BlobStorageTest(StorageSettings settings, ILoggerFactory logger)
            : base(settings, logger)
        {
        }

        public override string ContainerName { get => "teste-container"; }

        public override PublicAccessType AccessPolicy { get => PublicAccessType.Blob; }
    }
}