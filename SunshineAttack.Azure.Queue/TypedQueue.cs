using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using SunshineAttack.Common;

namespace SunshineAttack.Azure.TypedQueueMessage
{
    public class TypedQueue<T>
        where T : class, new()
    {
        protected CloudQueue Queue;

        public CloudQueue Advanced { get; set; }

        public TypedQueue(CloudQueue queue)
        {
            Advanced = queue;
            Queue = queue;
        }


        public Task DeleteMessageAsync(TypedMessage<T> message)
        {
            Func<CloudQueueMessage, AsyncCallback, object, IAsyncResult> deleteMessage =
                (queueMessage, callback, state) =>
                    Queue.BeginDeleteMessage(queueMessage, null, null, callback, state);

            return Task.Factory.FromAsync(deleteMessage, Queue.EndDeleteMessage, message.Advanced, null);
        }

        public async Task<IEnumerable<TypedMessage<T>>> GetMessagesAsync(int count)
        {

            IEnumerable<CloudQueueMessage> cloudMessages = await
                Queue.GetMessagesAsync(count);

            return cloudMessages == null ? await Empty<IEnumerable<TypedMessage<T>>>.Task : cloudMessages.Select(cloudMessage => new TypedMessage<T>(cloudMessage)).ToList();

        }

        public TypedMessage<T> GetMessage()
        {
            CloudQueueMessage message = Queue.GetMessage(TimeSpan.FromMinutes(5));

            Byte[] byteMessage = null;

            if (message != null)
            {
                byteMessage = message.AsBytes;
            }

            return byteMessage == null ? null : CreateTypedMessage(message.AsBytes.DecompressToObject<T>());
        }

        public Task AddMessageAsync(TypedMessage<T> message)
        {
            return Task.Factory.FromAsync(Queue.BeginAddMessage, Queue.EndAddMessage, message.Advanced, new { });
        }

        public void DeleteMessage(TypedMessage<T> message)
        {
            Queue.DeleteMessage(message.Advanced);
        }

        public TypedMessage<T> CreateTypedMessage(object obj)
        {
            return new TypedMessage<T>(obj);
        }

        public Task CreateTypedMessageAndAddToQueueAsync(object obj)
        {
            return AddMessageAsync(new TypedMessage<T>(obj));
        }
    }
}
