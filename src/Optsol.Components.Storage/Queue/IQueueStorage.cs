using Azure;
using Azure.Storage.Queues.Models;
using Optsol.Components.Storage.Queue.Models;
using System.Threading.Tasks;

namespace Optsol.Components.Storage.Queue
{
    public interface IQueueStorage
    {
        string QueueName { get; }

        Task<Response<SendReceipt>> SendMessageAsync<TData>(SendMessageModel<TData> message)
            where TData : class;

        Task<Response<SendReceipt>> SendMessageBase64Async<TData>(SendMessageModel<TData> message)
            where TData : class;

        Task<Response<UpdateReceipt>> UpdateMessageAsync<TData>(UpdateMessageModel<TData> message)
            where TData : class;

        Task<Response> DeleteMessageAsync<TData>(DeleteMessageModel message)
            where TData : class;

        Task<Response<QueueMessage[]>> ReceiveMessageAsync<TData>(string queueName) where TData : class;
    }
}