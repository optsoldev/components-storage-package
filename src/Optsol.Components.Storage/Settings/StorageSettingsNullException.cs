using Microsoft.Extensions.Logging;
using System;
using System.Runtime.Serialization;

namespace Optsol.Components.Storage.Settings
{
    [Serializable]
    public class StorageSettingsNullException : System.Exception
    {
        public StorageSettingsNullException(ILoggerFactory logger)
            : base("A configuração do STORAGE não foi encontrada no appsettings")
        {
            var _logger = logger?.CreateLogger(nameof(StorageSettingsNullException));
            _logger?.LogCritical(
                    @$"{nameof(StorageSettingsNullException)}:
                    ""StorageSettings"": {{
                        {{
                            ""ConnectionString"": ""{{UseDevelopmentStorage=true}}""
                        }}
                    }}"
            );
        }

        protected StorageSettingsNullException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}