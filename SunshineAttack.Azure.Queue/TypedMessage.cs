using System;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SunshineAttack.Azure.TypedQueueMessage
{
    public class TypedMessage<T> where T : class
    {
        protected CloudQueueMessage Message;

        public T AsObject
        {
            get { return Message == null ? null : Message.SerializeToObject<T>(); }
        }

        public String AsJson
        {
            get { return Message == null ? null : JsonConvert.SerializeObject(Message); }
        }

        public TypedMessage(object obj)
        {
            var message = new CloudQueueMessage(obj.Serialize());
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