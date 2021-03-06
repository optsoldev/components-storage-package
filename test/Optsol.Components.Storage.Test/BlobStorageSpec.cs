using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Optsol.Components.Storage.Extensions;
using Optsol.Components.Storage.Settings;
using Optsol.Components.Storage.Test.Utils.Blob;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Optsol.Components.Storage.Test
{
    public class BlobStorageSpec
    {
        [Trait("Table Storage", "Blob")]
#if DEBUG
        [Fact(DisplayName = "Deve registrar o serviço que implementa IBlobStorage na injeção de dependência")]
#elif RELEASE
        [Fact(DisplayName = "Deve registrar o serviço que implementa IBlobStorage na injeção de dependência", Skip ="Testes realizados localmente")]
#endif
        public void Deve_Registrar_Servico_Storage_Na_Injecao_De_Dependencia()
        {
            //Given
            var configuration = new ConfigurationBuilder()
                .AddJsonFile($@"Settings/appsettings.storage.json")
                .Build();

            var services = new ServiceCollection();

            services.AddLogging();
            services.AddStorage(configuration, options =>
            {
                options.ConfigureBlob<IBlobStorageTest, BlobStorageTest>("Optsol.Components.Storage.Test.Utils");
            });

            var provider = services.BuildServiceProvider();

            //When
            Action blobStorage = () => provider.GetRequiredService<IBlobStorageTest>();
            Action blobStorageDois = () => provider.GetRequiredService<IBlobStorageTestDois>();

            //Then
            blobStorage.Should().NotThrow();
            blobStorageDois.Should().NotThrow();
        }

        [Trait("Table Storage", "Blob")]
#if DEBUG
        [Fact(DisplayName = "Deve criar container blob no Azure Storage")]
#elif RELEASE
        [Fact(DisplayName = "Deve criar container blob no Azure Storage", Skip ="Testes realizados localmente")]
#endif
        public async Task Deve_Criar_Container_Blob_No_Azure_Storage()
        {
            //Given
            var configuration = new ConfigurationBuilder()
                .AddJsonFile($@"Settings/appsettings.storage.json")
                .Build();

            var services = new ServiceCollection();

            services.AddLogging();
            services.AddStorage(configuration, options =>
            {
                options.ConfigureBlob<IBlobStorageTest, BlobStorageTest>("Optsol.Components.Storage.Test.Utils");
            });

            var provider = services.BuildServiceProvider();
            var blobSettings = provider.GetRequiredService<StorageSettings>();
            var blobStorage = (BlobStorageTest)provider.GetRequiredService<IBlobStorageTest>();
            var blobStorageDois = (BlobStorageTestDois)provider.GetRequiredService<IBlobStorageTestDois>();

            //When
            var containerExisteNoAzureStorage = await blobStorage.ContainerExistsAsync();
            var containerDoisExiteNoAzureStorage = await blobStorageDois.ContainerExistsAsync();

            //Then
            blobSettings.Should().NotBeNull();
            blobStorage.Should().NotBeNull();
            blobStorageDois.Should().NotBeNull();

            containerExisteNoAzureStorage.Should().NotBeNull();
            containerExisteNoAzureStorage.GetRawResponse().Status.Should().Be(200);
            containerExisteNoAzureStorage.Value.Should().BeTrue();

            containerDoisExiteNoAzureStorage.Should().NotBeNull();
            containerDoisExiteNoAzureStorage.GetRawResponse().Status.Should().Be(200);
            containerDoisExiteNoAzureStorage.Value.Should().BeTrue();
        }

        [Trait("Table Storage", "Blob")]
#if DEBUG
        [Fact(DisplayName = "Deve fazer upload de arquivo no blob pelo stream")]
#elif RELEASE
        [Fact(DisplayName = "Deve fazer upload de arquivo no blob pelo stream", Skip ="Testes realizados localmente")]
#endif
        public void Deve_Fazer_Upload_De_Arquivo_No_Blob_Pelo_Stream()
        {
            //Given
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(@"Settings/appsettings.storage.json")
                .Build();

            var services = new ServiceCollection();

            services.AddLogging();
            services.AddStorage(configuration, options =>
            {
                options.ConfigureBlob<IBlobStorageTest, BlobStorageTest>("Optsol.Components.Storage.Test.Utils");
            });

            var provider = services.BuildServiceProvider();
            var blobStorage = (BlobStorageTest)provider.GetRequiredService<IBlobStorageTest>();
            var blobStorageDois = (BlobStorageTestDois)provider.GetRequiredService<IBlobStorageTestDois>();

            //When
            Action action = () => blobStorage.UploadAsync($"{Guid.NewGuid()}.jpg", File.OpenRead(@"Attachments/attachment.jpg"));
            Action actionDois = () => blobStorageDois.UploadAsync($"{Guid.NewGuid()}.jpg", File.OpenRead(@"Attachments/attachment.jpg"));

            //Then
            blobStorage.Should().NotBeNull();
            blobStorageDois.Should().NotBeNull();

            action.Should().NotThrow();
            actionDois.Should().NotThrow();
        }

        [Trait("Table Storage", "Blob")]
#if DEBUG
        [Fact(DisplayName = "Deve fazer upload de arquivo no blob pelo path")]
#elif RELEASE
        [Fact(DisplayName = "Deve fazer upload de arquivo no blob pelo path", Skip ="Testes realizados localmente")]
#endif
        public void Deve_Fazer_Upload_De_Arquivo_No_Blob_Pelo_Path()
        {
            //Given
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(@"Settings/appsettings.storage.json")
                .Build();

            var services = new ServiceCollection();

            services.AddLogging();
            services.AddStorage(configuration, options =>
            {
                options.ConfigureBlob<IBlobStorageTest, BlobStorageTest>("Optsol.Components.Storage.Test.Utils");
            });

            var provider = services.BuildServiceProvider();
            var blobStorage = (BlobStorageTest)provider.GetRequiredService<IBlobStorageTest>();
            var blobStorageDois = (BlobStorageTestDois)provider.GetRequiredService<IBlobStorageTestDois>();

            //When
            Action action = () => blobStorage.UploadAsync($"{Guid.NewGuid()}.jpg", @"Attachments/attachment.jpg");
            Action actionDois = () => blobStorageDois.UploadAsync($"{Guid.NewGuid()}.jpg", @"Attachments/attachment.jpg");

            //Then
            blobStorage.Should().NotBeNull();
            blobStorageDois.Should().NotBeNull();

            action.Should().NotThrow();
            actionDois.Should().NotThrow();
        }

        [Trait("Table Storage", "Blob")]
#if DEBUG
        [Fact(DisplayName = "Deve apagar arquivo no blob pelo nome")]
#elif RELEASE
        [Fact(DisplayName = "Deve apagar arquivo no blob pelo nome", Skip ="Testes realizados localmente")]
#endif
        public async Task Deve_Apagar_Arquivo_No_Blob_Pelo_Nome()
        {
            //Given
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(@"Settings/appsettings.storage.json")
                .Build();

            var services = new ServiceCollection();

            services.AddLogging();
            services.AddStorage(configuration, options =>
            {
                options.ConfigureBlob<IBlobStorageTest, BlobStorageTest>("Optsol.Components.Storage.Test.Utils");
            });

            var provider = services.BuildServiceProvider();
            var blobStorage = (BlobStorageTest)provider.GetRequiredService<IBlobStorageTest>();
            var blobStorageDois = (BlobStorageTestDois)provider.GetRequiredService<IBlobStorageTestDois>();

            var name = $"{Guid.NewGuid()}.jpg";
            await blobStorage.UploadAsync(name, @"Attachments/attachment.jpg");

            //When
            Action action = () => blobStorage.DeleteAsync(name);

            //Then
            blobStorage.Should().NotBeNull();

            action.Should().NotThrow();
        }

        [Trait("Table Storage", "Blob")]
#if DEBUG
        [Fact(DisplayName = "Deve fazer download do arquivo no blob pelo nome")]
#elif RELEASE
        [Fact(DisplayName = "Deve fazer download do arquivo no blob pelo nome", Skip ="Testes realizados localmente")]
#endif
        public async Task Deve_Fazer_Download_Do_Arquivo_No_Blob_Pelo_Nome()
        {
            //Given
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(@"Settings/appsettings.storage.json")
                .Build();

            var services = new ServiceCollection();

            services.AddLogging();
            services.AddStorage(configuration, options =>
            {
                options.ConfigureBlob<IBlobStorageTest, BlobStorageTest>("Optsol.Components.Storage.Test.Utils");
            });

            var provider = services.BuildServiceProvider();
            var blobStorage = (BlobStorageTestDois)provider.GetRequiredService<IBlobStorageTestDois>();

            var name = $"{Guid.NewGuid()}.jpg";
            await blobStorage.UploadAsync(name, @"Attachments/attachment.jpg");

            //When
            var arquivoDoBlob = await blobStorage.DownloadAsync(name);

            //Then
            blobStorage.Should().NotBeNull();

            arquivoDoBlob.Should().NotBeNull();
            arquivoDoBlob.GetRawResponse().Status.Should().Be(200);
            arquivoDoBlob.Value.ContentLength.Should().BeGreaterThan(0);
        }

        [Trait("Table Storage", "Blob")]
#if DEBUG
        [Fact(DisplayName = "Deve retornar a url do arquivo no blob pelo nome")]
#elif RELEASE
        [Fact(DisplayName = "Deve retornar a url do arquivo no blob pelo nome", Skip ="Testes realizados localmente")]
#endif
        public async Task Deve_Retornar_Url_Do_Arquivo_No_Blob_Pelo_Nome()
        {
            //Given
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(@"Settings/appsettings.storage.json")
                .Build();

            var services = new ServiceCollection();

            services.AddLogging();
            services.AddStorage(configuration, options =>
            {
                options.ConfigureBlob<IBlobStorageTest, BlobStorageTest>("Optsol.Components.Storage.Test.Utils");
            });

            var provider = services.BuildServiceProvider();
            var blobStorage = (BlobStorageTestDois)provider.GetRequiredService<IBlobStorageTestDois>();

            var name = $"{Guid.NewGuid()}.jpg";
            await blobStorage.UploadAsync(name, @"Attachments/attachment.jpg");

            //When
            var arquivoDoBlob = await blobStorage.GetUriAsync(name);

            //Then
            blobStorage.Should().NotBeNull();

            arquivoDoBlob.Should().NotBeNull();
            arquivoDoBlob.AbsoluteUri.Contains(name).Should().BeTrue();
        }


        [Trait("Table Storage", "Blob")]
#if DEBUG
        [Fact(DisplayName = "Deve obter todos os blobs")]
#elif RELEASE
        [Fact(DisplayName = "Deve obter todos os blobs", Skip ="Testes realizados localmente")]
#endif
        public async Task Deve_Obter_Todos_Blobs()
        {
            //Given
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(@"Settings/appsettings.storage.json")
                .Build();

            var services = new ServiceCollection();

            services.AddLogging();
            services.AddStorage(configuration, options =>
            {
                options.ConfigureBlob<IBlobStorageTest, BlobStorageTest>("Optsol.Components.Storage.Test.Utils");
            });

            var provider = services.BuildServiceProvider();
            var blobStorage = (BlobStorageTestDois)provider.GetRequiredService<IBlobStorageTestDois>();

            var name = $"{Guid.NewGuid()}.jpg";
            await blobStorage.UploadAsync(name, @"Attachments/attachment.jpg");

            //When
            var arquivos = await blobStorage.GetAllAsync();

            //Then
            blobStorage.Should().NotBeNull();

            arquivos.Should().NotBeNull();
            arquivos.Count().Should().BeGreaterThanOrEqualTo(1);
        }
    }
}