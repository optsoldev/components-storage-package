﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Optsol.Components.Storage.Blob;
using Optsol.Components.Storage.Queue;
using Optsol.Components.Storage.Settings;
using System;

namespace Optsol.Components.Storage.Extensions
{
    public static class StorageExtensions
    {
        public static IServiceCollection AddStorage(this IServiceCollection services, IConfiguration configuration, Action<StorageOptions> options)
        {
            var servicesProvider = services.BuildServiceProvider();

            var storageSettings = configuration.GetSection(nameof(StorageSettings)).Get<StorageSettings>()
                ?? throw new StorageSettingsNullException(servicesProvider.GetRequiredService<ILoggerFactory>());
            storageSettings.Validate();

            services.AddSingleton(storageSettings);

            var storageOptions = new StorageOptions(services);
            options?.Invoke(storageOptions);

            return services;
        }
    }

    public class StorageOptions
    {
        private readonly IServiceCollection _services;

        public StorageOptions(IServiceCollection services)
        {
            _services = services;
        }

        public StorageOptions ConfigureBlob<TInterface, TImplementation>(params string[] namespaces)
            where TInterface : IBlobStorage
            where TImplementation : BlobStorageBase

        {
            _services.RegisterTransient<TInterface, TImplementation>(namespaces);

            return this;
        }

        public StorageOptions ConfigureQueue<TInterface, TImplementation>(params string[] namespaces)
            where TInterface : IQueueStorage
            where TImplementation : QueueStorageBase
        {
            _services.RegisterTransient<TInterface, TImplementation>(namespaces);

            return this;
        }
    }
}