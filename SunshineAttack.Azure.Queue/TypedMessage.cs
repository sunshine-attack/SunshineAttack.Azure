using Microsoft.WindowsAzure.Storage.Queue;
using SunshineAttack.Azure.Common;

namespace SunshineAttack.Azure.TypedQueueMessage
{
    public class TypedMessage<T> where T : class
    {
        protected CloudQueueMessage Message;

        public T AsObject
        {
            get { return Message == null ? null : Message.AsBytes.DecompressToObject<T>(); }
        }

        public TypedMessage(object obj)
        {
            var message = new CloudQueueMessage(obj.Compress());
            Advanced = message;
            Message = message;
        }


        public TypedMessage(CloudQueueMessage message)
        {
            Advanced = message;
            Message = message;
        }

        public CloudQueueMessage Advanced { get; set; }
    }
}