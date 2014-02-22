SunshineAttack.Azure
----------------------------

Provides an easy to use API to access the Azure Queue API using strongly-typed message.

* Each message on the queue is represented by strongly-typed object
* Async support


### Usage
```csharp
_appleQueue = QueueFactory.GetTypedQueue<Apple>();

var messagge = _queue.GetMessage();

var apple = messagge.AsObject;

var color = apple.color;

await _appleQueue.DeleteMessageAsync(messageRequest);
```


Nuget package to come...

	


