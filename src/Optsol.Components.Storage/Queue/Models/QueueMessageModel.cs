using Azure.Storage.Queues.Models;

namespace Optsol.Components.Storage.Queue.Models
{
    public class SendMessageModel<TData>
    {
        public TData Data { get; set; }

        public QueueMessage Message { get; set; }

        public SendMessageModel(TData data)
        {
            Data = data;
        }
    }

    public class UpdateMessageModel<TData> : SendMessageModel<TData>
    {
        public string MessageId { get; }
        public string PopReceipt { get; }

        public UpdateMessageModel(string messageId, string popReceipt, TData data) : base(data)
        {
            MessageId = messageId;
            PopReceipt = popReceipt;
        }
    }

    public class ReceiveMessageModel<TData> : SendMessageModel<TData>
    {
        public ReceiveMessageModel(TData data) : base(data)
        {
        }
    }

    public class DeleteMessageModel
    {
        public string MessageId { get; }
        public string PopReceipt { get; }

        public DeleteMessageModel(string messageId, string popReceipt)
        {
            MessageId = messageId;
            PopReceipt = popReceipt;
        }
    }
}