using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Optsol.Components.Storage.Extensions;
using Optsol.Components.Storage.Queue.Models;
using Optsol.Components.Storage.Settings;
using Optsol.Components.Storage.Test.Utils.Queue;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Optsol.Components.Storage.Test
{
    public class QueueStorageSpec
    {
        [Trait("Table Storage", "Queue")]
#if DEBUG
        [Fact(DisplayName = "Deve registrar o serviço IQueueStorage na injeção de dependência")]
#elif RELEASE
        [Fact(DisplayName = "Deve registrar o serviço IQueueStorage na injeção de dependência", Skip ="Testes realizados localmente")]
#endif
        public void Deve_Registrar_Servico_Storage_Queue_Na_Injecao_De_Dependencia()
        {
            //Given
            var configuration = new ConfigurationBuilder()
                .AddJsonFile($@"Settings/appsettings.storage.json")
                .Build();

            var services = new ServiceCollection();

            services.AddLogging();
            services.AddStorage(configuration, options =>
            {
                options.ConfigureQueue<IQueueStorageTest, QueueStorageTest>("Optsol.Components.Storage.Test.Utils");
            });

            var provider = services.BuildServiceProvider();

            //When
            Action blobStorage = () => provider.GetRequiredService<IQueueStorageTest>();
            Action blobStorageDois = () => provider.GetRequiredService<IQueueStorageTestDois>();

            //Then
            blobStorage.Should().NotThrow();
            blobStorageDois.Should().NotThrow();
        }

        [Trait("Table Storage", "Queue")]
#if DEBUG
        [Fact(DisplayName = "Deve enviar uma mensagem para fila")]
#elif RELEASE
        [Fact(Skip ="Testes realizados localmente")]
#endif
        public async Task Deve_Enviar_Mensagem_Queue_No_Azure_Storage()
        {
            //Given
            var configuration = new ConfigurationBuilder()
                .AddJsonFile($@"Settings/appsettings.storage.json")
                .Build();

            var services = new ServiceCollection();

            services.AddLogging();
            services.AddStorage(configuration, options =>
            {
                options.ConfigureQueue<IQueueStorageTest, QueueStorageTest>("Optsol.Components.Storage.Test.Utils");
            });

            var viewModel = new TestResponseDto();
            viewModel.Id = Guid.NewGuid();
            viewModel.Nome = "Weslley Carneiro";
            viewModel.Contato = "weslley.carneiro@optsol.com.br";

            var provider = services.BuildServiceProvider();
            var blobSettings = provider.GetRequiredService<StorageSettings>();
            var queueStorage = provider.GetRequiredService<IQueueStorageTest>();
            var queueStorageDois = provider.GetRequiredService<IQueueStorageTestDois>();

            var mensagemGerada = string.Format("Olá {0}. Sua consulta foi agendada para o dia {1:dd/MM/yyyy 'às' HH:mm:ss}, quando chegar o dia acesse o portal através deste link: {2}", "Weslley Carneiro", DateTime.Now, "https://wwww.optsol.com.br");
            viewModel.Nome = mensagemGerada;

            var messageModel = new SendMessageModel<TestResponseDto>(viewModel);

            //When
            var containerExiteNoAzureStorage = await queueStorage.SendMessageBase64Async(messageModel);
            var containerExiteNoAzureStorageDois = await queueStorageDois.SendMessageBase64Async(messageModel);

            //Then
            blobSettings.Should().NotBeNull();
            queueStorage.Should().NotBeNull();

            containerExiteNoAzureStorage.Should().NotBeNull();
            containerExiteNoAzureStorage.GetRawResponse().Status.Should().Be(201);

            containerExiteNoAzureStorageDois.Should().NotBeNull();
            containerExiteNoAzureStorageDois.GetRawResponse().Status.Should().Be(201);
        }

        [Trait("Table Storage", "Queue")]
#if DEBUG
        [Fact(DisplayName = "Deve atualizar uma mensagem da fila")]
#elif RELEASE
        [Fact(Skip ="Testes realizados localmente")]
#endif
        public async Task Deve_Atualizar_Mensagem_Queue_No_Azure_Storage()
        {
            //Given
            var configuration = new ConfigurationBuilder()
                .AddJsonFile($@"Settings/appsettings.storage.json")
                .Build();

            var services = new ServiceCollection();

            services.AddLogging();
            services.AddStorage(configuration, options =>
            {
                options.ConfigureQueue<IQueueStorageTest, QueueStorageTest>("Optsol.Components.Storage.Test.Utils");
            });

            var viewModel = new TestResponseDto();
            viewModel.Id = Guid.NewGuid();
            viewModel.Contato = "weslley.carneiro@optsol.com.br";

            var provider = services.BuildServiceProvider();
            var blobSettings = provider.GetRequiredService<StorageSettings>();
            var queueStorage = provider.GetRequiredService<IQueueStorageTest>();

            var mensagemGerada = string.Format("Olá {0}. Sua consulta foi agendada para o dia {1:dd/MM/yyyy 'às' HH:mm:ss}, quando chegar o dia acesse o portal através deste link: {2}", "Weslley Carneiro", DateTime.Now, "https://wwww.optsol.com.br");
            viewModel.Nome = mensagemGerada;

            var messageModel = new SendMessageModel<TestResponseDto>(viewModel);

            //When
            var containerExisteNoAzureStorage = await queueStorage.SendMessageBase64Async(messageModel);

            viewModel.Nome = "Mensagem atualizada";
            var updatedMessage = new UpdateMessageModel<TestResponseDto>(containerExisteNoAzureStorage.Value.MessageId, containerExisteNoAzureStorage.Value.PopReceipt, viewModel);
            var updatedContainer = await queueStorage.UpdateMessageAsync(updatedMessage);

            //Then
            blobSettings.Should().NotBeNull();
            queueStorage.Should().NotBeNull();

            updatedContainer.Should().NotBeNull();
            updatedContainer.GetRawResponse().Status.Should().Be(204);
        }

        [Trait("Table Storage", "Queue")]
#if DEBUG
        [Fact(DisplayName = "Deve recuperar mensagens da fila")]
#elif RELEASE
        [Fact(Skip ="Testes realizados localmente")]
#endif
        public async Task Deve_Recuperar_Mensagens_Do_Queue_No_Azure_Storage()
        {
            //Given
            var configuration = new ConfigurationBuilder()
                .AddJsonFile($@"Settings/appsettings.storage.json")
                .Build();

            var services = new ServiceCollection();

            services.AddLogging();
            services.AddStorage(configuration, options =>
            {
                options.ConfigureQueue<IQueueStorageTest, QueueStorageTest>("Optsol.Components.Storage.Test.Utils");
            });
            var provider = services.BuildServiceProvider();
            var blobSettings = provider.GetRequiredService<StorageSettings>();
            var queueStorage = provider.GetRequiredService<IQueueStorageTest>();

            var viewModel = new TestResponseDto();
            viewModel.Id = Guid.NewGuid();
            viewModel.Contato = "weslley.carneiro@optsol.com.br";
            var mensagemGerada = string.Format("Olá {0}. Sua consulta foi agendada para o dia {1:dd/MM/yyyy 'às' HH:mm:ss}, quando chegar o dia acesse o portal através deste link: {2}", "Weslley Carneiro", DateTime.Now, "https://wwww.optsol.com.br");
            viewModel.Nome = mensagemGerada;
            var messageModel = new SendMessageModel<TestResponseDto>(viewModel);
            var containerExisteNoAzureStorage = await queueStorage.SendMessageBase64Async(messageModel);

            //When
            var recoveredMessages = await queueStorage.ReceiveMessageAsync();

            //Then
            blobSettings.Should().NotBeNull();
            queueStorage.Should().NotBeNull();

            recoveredMessages.Should().NotBeNull();
            recoveredMessages.Value.Should().NotBeEmpty();
        }

        [Trait("Table Storage", "Queue")]
#if DEBUG
        [Fact(DisplayName = "Deve deletar uma mensagem da fila")]
#elif RELEASE
        [Fact(Skip ="Testes realizados localmente")]
#endif
        public async Task Deve_Deletar_Mensagem_Do_Queue_No_Azure_Storage()
        {
            //Given
            var configuration = new ConfigurationBuilder()
                .AddJsonFile($@"Settings/appsettings.storage.json")
                .Build();

            var services = new ServiceCollection();

            services.AddLogging();
            services.AddStorage(configuration, options =>
            {
                options.ConfigureQueue<IQueueStorageTest, QueueStorageTest>("Optsol.Components.Storage.Test.Utils");
            });

            var viewModel = new TestResponseDto();
            viewModel.Id = Guid.NewGuid();
            viewModel.Contato = "weslley.carneiro@optsol.com.br";

            var provider = services.BuildServiceProvider();
            var blobSettings = provider.GetRequiredService<StorageSettings>();
            var queueStorage = provider.GetRequiredService<IQueueStorageTest>();

            var mensagemGerada = string.Format("Olá {0}. Sua consulta foi agendada para o dia {1:dd/MM/yyyy 'às' HH:mm:ss}, quando chegar o dia acesse o portal através deste link: {2}", "Weslley Carneiro", DateTime.Now, "https://wwww.optsol.com.br");
            viewModel.Nome = mensagemGerada;

            var messageModel = new SendMessageModel<TestResponseDto>(viewModel);

            //When
            var containerExisteNoAzureStorage = await queueStorage.SendMessageBase64Async(messageModel);

            var messageToDelete = new DeleteMessageModel(containerExisteNoAzureStorage.Value.MessageId, containerExisteNoAzureStorage.Value.PopReceipt);
            var responseContainer = await queueStorage.DeleteMessageAsync(messageToDelete);

            //Then
            blobSettings.Should().NotBeNull();
            queueStorage.Should().NotBeNull();

            responseContainer.Should().NotBeNull();
            responseContainer.Status.Should().Be(204);
        }
    }
}

public class TestResponseDto
{
    public Guid Id { get; set; }

    public string Nome { get; set; }

    public string Contato { get; set; }

    public string Ativo { get; set; }
}