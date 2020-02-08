# AzureFunctions
Implement a Rest API that manages a todo list using Azure Functions 

The default route for an azure function: **localhost/api/functionName**

We can use Azure Functions to implement a Rest API that will have the desired routh patterns for a rest api e.g:
* get: localhost/api/todo
* post: localhost/api/todo
* getById: localhost/ap/todo/{id}
* update: localhost/api/todo/{id}
* delete: localhost/api/todo/{id}

We can customise function route by using the **route** and **specifying the http verb** in the functions parameters. 
When the route parameter is specified the route of the function will change to match:
**localhost/api/routeParamValue**

That's how we can implement REST API with azure functions.

### Triggers and Bindings 
https://docs.microsoft.com/en-us/azure/azure-functions/functions-triggers-bindings

## Triggers: 
Triggers are what cause a function to run. A trigger defines how a function is invoked and a function must have exactly one trigger.
* **Https Trigger**: makes the function respond to an http request.
* **Timer Trigger**: for scheduled functions.
* **Blob Trigger**: responds to blobs been created in blob storage.
* **Queue Trigger**: responds to message on a storage queue.

## Bindings:
Binding to a function is a way of declaratively connecting another resource to the function (allows you to connect to external bindings. e.g post message to a queue or write to a blob/ table storage.); bindings may be connected as input bindings, output bindings, or both. Data from bindings is provided to the function as parameters.
Bindings are optional and a function might have one or multiple input and/or output bindings.
* **Input Bindings**: brings data into your function from an external servic, e.g blob binding (can read the content of a blob from a blobs storage container), cosmosDb binding (can find a document in the cosmosDb database) etc
* **Output Bindings**: allows you to send data to an external service e.g queue binding (you can send message to a queue), sendGrid binding (with the sendgrid output binding extension you can send an email) 

Triggers and Bindings(input/ouput) are defined in the function.json file, generated automaticlly when we build.

Additional bindings are added as an extension through nuget packages e.g cosmosDb, sendGrid etc.
https://docs.microsoft.com/en-us/azure/azure-functions/functions-triggers-bindings#supported-bindings
https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-table?tabs=csharp

## Summary:
* To implement Rest API with azure functions we use Http Trigger attribute to customise the **route** for each function.
* Multiple functions can share the same route as long has they respond to different Http methods(verbs).
* We can use the route template syntax e.g {id} to pass in the itemId that we are interested in, in a route for **HttpGet(Id), HttpUpdate(Id) and HttpDeleted(Id)** 
* We can serilise objects in the response of our methods by using the **OkObjectResult**
