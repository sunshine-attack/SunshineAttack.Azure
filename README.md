SunshineAttack.Azure
----------------------------

Provides an easy to use API to access the Azure Cloud Queue API using strongly-typed messages.

* A queue manager to simplify creating and initializing queues
* Messages are automatically serialized and deserialized onto the queue as typed objects
* Each message on the queue is represented by a strongly-typed object
* Async support


### Usage
```csharp
var _appleQueue = new TypedQueue<Apple>("Apple");

var messagge = _queue.GetMessage();

Apple apple = messagge.AsObject;

var color = apple.color;

await _appleQueue.DeleteMessageAsync(messageRequest);
```


Nuget package to come...

	


