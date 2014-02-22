using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Queue.Protocol;

namespace SunshineAttack.Azure.TypedQueueMessage
{
    public class QueueManager
    {
        private static Dictionary<string, CloudQueue> _queues;
        private static CloudQueueClient _blobClient;

        public static CloudQueue GetQueues(string name)
        {
            CloudQueue queue;

            _queues.TryGetValue(name, out queue);

            return queue;
        }

        public static void Initialize(CloudQueueClient blobClient, String[] queues )
        {
            _blobClient = blobClient;

            _queues = new Dictionary<string, CloudQueue>();

            foreach (var queue in queues)
            {
                CreateQueue(queue);
            }

        }

        private static void CreateQueue(string name)
        {
            var queue = _blobClient.GetQueueReference(name);

            queue.CreateIfNotExists();

            SetSharedAccessControl(queue);

            _queues.Add(name, queue);
        }

        private static void SetSharedAccessControl(CloudQueue queue)
        {
            // Create blob queue permissions, consisting of a shared access policy 
            // and a public access setting. 
            var queuePermissions = new QueuePermissions();

            // The shared access policy provides 
            // read/write access to the queue for 10 hours.
            queuePermissions.SharedAccessPolicies.Add("mypolicy", new SharedAccessQueuePolicy()
                {
                    //If valid immediately, don’t set SharedAccessStartTime,
                    //to avoid failures caused by small clock differences.
                    // This policy can be used one hour from now.
                    //SharedAccessStartTime = DateTime.UtcNow,
                    SharedAccessExpiryTime = DateTime.UtcNow.AddHours(11),
                    Permissions = SharedAccessQueuePermissions.Add //|
                    //SharedAccessBlobPermissions.Read
                });

            // Set the permission policy on the queue.
            queue.SetPermissions(queuePermissions);
        }
    }
}

