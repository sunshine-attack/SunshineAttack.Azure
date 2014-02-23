using System;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SunshineAttack.Azure.TypedQueueMessage
{
    public class TypedMessage<T> where T : class
    {
        private CloudQueueMessage _message;

        public T AsObject
        {
            get { return _message == null ? null : _message.Deserialize<T>(); }
        }

        public String AsJson
        {
            get { return _message == null ? null : JsonConvert.SerializeObject(_message); }
        }

        public TypedMessage(object obj)
        {
            object objectToAdd = obj;

            var message = new CloudQueueMessage(objectToAdd.Serialize());
            Advanced = message;
            _message = message;
        }


        public TypedMessage(CloudQueueMessage message)
        {
            Advanced = message;
            _message = message;
        }

        public CloudQueueMessage Advanced { get; set; }
    }
}