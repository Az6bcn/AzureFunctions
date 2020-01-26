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

