
## NetRabbit
High throughput RMQ client. Client concurrently processes **N** messages where **N** is the `prefetch` count for the channel.


# Setup

Install the package via NuGet: `Install-Package NetRabbit`. This is a `NET 9` library

Register subscribers/publisher for DI.
For easy registration, it is recommended to install `NetRabbit.Extensions`
To use a specific subscriber, use the **Add{ProcessorName}** extension method(s).

```csharp
//add infrastructure 
var rabbit = services.AddRabbit();

//add subscribers
rabbit.AddBasicAsyncMessageHandler(); // Assuming your Processor is named BasicAsyncMessageHandler

//register publisher if need be
rabbit.AddPublisherService();
```


## Creating a subscriber

Each subscriber is responsible for supplying its own settings. This includes the connection settings as well as subscriber settings i.e. queue name to consume from and prefetch count etc.

Implement interface `IMessageHandlerAsync` e.g.

```csharp
public class BasicAsyncMessageHandler : IMessageHandlerAsync
{
    public IEnumerable<SubscriberSettings> GetSubscriberSettings()
    {
       //one suscriber can bind to multiple queues hence IEnumerable
        yield return new SubscriberSettings("TAG Name", "Queue name", 1000 /*prefetch*/);
    }
    
    public Task<IBasicConnectionSettings> GetBasicConnectionSettingsAsync(CancellationToken cancellationToken = default)
    {
	    return Task.FromResult(new BasicConnectionSettings
        {
	        ConnectionString = "amqp://your.rmq.domain",
	        Username = "username",
	        Password = "password",
	        VirtualHost = "/"
	     });
    }
 
    public async Task<bool> ProcessAsync(SubscriberBrokeredMessage message, CancellationToken cancellationToken = default)
    {
        //this will deserialize using System.Text.Json
        //or if you want to use your own deserializer, implement IJsonSerializer
        //and supply instance to overload.
        var payload = message.JsonDeserialize<TestMessage>();
        //simulate work   
        await Task.Delay(8000, cancellationToken);
        //true results in an Ack, false/exception thrown in a Nack. RequeueOnNack
        //is set up top in SubscriberSettings
        return true;
    }
}
```

You can also use a generic `IMessageHandlerAsync<T>` that will deliver your deserialized rmq payload as part of the method signature

```csharp
public class BasicAsyncMessageHandler2 : IMessageHandlerAsync<TestMessage>
{
   public async Task<bool> ProcessAsync(TestMessage? payload, BasicMessageProperties? messageProperties, CancellationToken cancellationToken)
   {
       await Task.Delay(1000, cancellationToken);
       return true;
   }
}
```

The default deserializer getting used here is System.Text.Json with the following settings:
```csharp
var options = new JsonSerializerOptions
{
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
};
options.Converters.Add(new JsonStringEnumConverter());
```

If you need more customization or have the need for a different serializer, you can implement `GetSerializer` method for the subscriber e.g.

```csharp
public ISerializer GetSerializer()
{
    //return your custom serializer that implements ISerializer
}
```

For most use cases, we don't need or care about the rmq message properties that usually flow from rmq. These values are wrapped in `BasicMessageProperties` that is part of the `ProcessAsync` signature. If you don't need that object populated, you can implement 
```csharp
public bool PopulateBasicMessageProperties()
{
    return false;
}

```
inside the processor. This will result in a `null` value for `BasicMessageProperties`

## Publishing messages
There are 2 different ways of publishing messages:

`PublishAsync` - Fastest. No confirms needed from cluster. Fire and forget.

`PublishConfirmAsync` - When a message is absolutely critical and you need rabbit to (n)ack that it (not)received it.

```csharp
public class PublishTest
{
    private readonly IPublisherService _publisherService;
    
    public PublishTest(IPublisherService publisherService)
    {
        _publisherService = publisherService;
    }
	
    public async Task Test()
    {
        var message = new SomeTestMessageDto();
        var brokeredMessage = new PublisherBrokeredMessage(message); //WithHeaders allows you to specify exchange headers
        
        //This overload assumes you have already setup your basic connection settings during
        //the process startup phase. If you have not done that, you will get a NPE. Use the
        //other overload where it takes BasicConnectionSettings argument.
	var response = await _publisherService.PublishAsync
	(
	    brokeredMessage,
	    "myexchange" //exchange name
	);
	//call this if you need an exception thrown if an error occurred during publishing
	response.EnsureSuccess();
/*
	var response = await _publisherService.PublishAsync
	(
    	    new BasicConnectionSettings
            {
                ConnectionString = "amqp://rmq.domain",
                Username = "username",
                Password = "password",
                VirtualHost = "/"
            },
	    brokeredMessage,
	    "myexchange" //exchange name
	);
	response.EnsureSuccess();
*/
	
}

```
`PublishConfirmAsync` works exactly the same as above.


## Miscellaneous
Included is a `IChannelPreWarmer` that allows you to establish a connection to rabbit and create the channel that will be used for publishing so that the first request for publishing does not take that performance hit. If you have the concept of startup tasks in your application that runs before Kestrel serves requests, you can prewarm e.g.


```csharp
//connectionsettings, exchange name, true/false whether it's going to use publisher confirms or not
await _channelPreWarmer.WarmUpForPublish(_basicConnectionSettings, "myexchange", false);
```

There is also an `IConnectionProbe` that you can use to for your applcation's healthcheck to see if your connection to rmq is still alive.
