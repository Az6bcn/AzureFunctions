using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http.Internal;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;
using System.Security.Cryptography.X509Certificates;
using Microsoft.WindowsAzure.Storage;

namespace ServerlessFuncs
{
    public static class TodoApi
    {

        [FunctionName("CreateTodo")]
        public static async Task<IActionResult> CreateTodo([HttpTrigger(AuthorizationLevel.Anonymous,
            "post", Route = "todo")] HttpRequest req,ILogger log,
            [Table("todos", Connection = "AzureWebJobsStorage")] IAsyncCollector<TodoTableEntity> todoTable)
        {
            log.LogInformation("Creating a new todo list item.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<TodoCreateModel>(requestBody);

            var todo = new Todo { TaskDescription = data.TaskDescription };

            // map this to-do to to-doTableEntity
            var todoTableEntity = Mappings.ToTableEntity(todo);

            // bind/add to table
            await todoTable.AddAsync(todoTableEntity);

            return new OkObjectResult(todo);
        }

        [FunctionName("GetAllTodos")]
        public static async Task<IActionResult> GetAllTodos([HttpTrigger(AuthorizationLevel.Anonymous,
            "get", Route = "todo")] HttpRequest req, ILogger log,
            [Table("todos", Connection = "AzureWebJobsStorage")] CloudTable cloudTable)
        {
            log.LogInformation("Getting all todo list item.");

            var query = new TableQuery<TodoTableEntity>();

            var items = await cloudTable.ExecuteQuerySegmentedAsync(query, null);

            // map to To-do
            var itemsList = items.Select(x => Mappings.ToTodo(x));

            return new OkObjectResult(itemsList);
        }

        [FunctionName("GetTodoById")]
        public static async Task<IActionResult> GetTodoById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo/{id}")] HttpRequest req, ILogger log, string id,
            [Table("todos", "TODO", "{id}", Connection = "AzureStorageConnectionString")] TodoTableEntity todo)
        {
            log.LogInformation("Getting a specific todo list item by its Id.");

            if(id == Guid.Empty.ToString() || id == string.Empty)
            {
                return new BadRequestObjectResult("invalid Id");
            }

            if(todo is null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(Mappings.ToTodo(todo));
        }

        [FunctionName("UpdateTodo")]
        public static async Task<IActionResult> UpdateTodo([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todo/{id}")] HttpRequest req, ILogger log, string id,
            [Table("todos", Connection = "AzureWebJobsStorage")] CloudTable cloudTable)
        {
            log.LogInformation("Updating a specific todo list item by its Id.");

            if (id == Guid.Empty.ToString() || id == string.Empty)
            {
                return new BadRequestObjectResult("invalid Id");
            }

            // find item in table storage
            var findOperation = TableOperation.Retrieve<TodoTableEntity>("TODO", id);

            var item = await cloudTable.ExecuteAsync(findOperation);

            if (item.Result is null)
            {
                return new NotFoundResult();
            }

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updatedTodo = JsonConvert.DeserializeObject<TodoUpdateModel>(requestBody);

            // map to to-do
            var foundItem = Mappings.ToTodo(item.Result as TodoTableEntity);

            foundItem.IsCompleted = updatedTodo.IsCompleted;

            if (!string.IsNullOrEmpty(updatedTodo.TaskDescription))
            {
                foundItem.TaskDescription = updatedTodo.TaskDescription;
            }

            // update item in table storage
            var replaceOperation = TableOperation.Replace(Mappings.ToTableEntity(foundItem));
            await cloudTable.ExecuteAsync(replaceOperation);

            return new OkObjectResult(foundItem);
        }

        [FunctionName("DeleteTodo")]
        public static async Task<IActionResult> DeleteTodo([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todo/{id}")] HttpRequest req, ILogger log, string id,
           [Table("todos", Connection = "AzureWebJobsStorage")] CloudTable cloudTable)
        {
            log.LogInformation("Deleting a specific todo list item by its Id.");

            if (id == Guid.Empty.ToString() || id == string.Empty)
            {
                return new BadRequestObjectResult("invalid Id");
            }

            if (id == null)
            {
                return new NotFoundResult();
            }

            var deleteOperation = TableOperation.Delete(new TodoTableEntity("TODO", id, "*"));

            try
            {
                var deleteResult = await cloudTable.ExecuteAsync(deleteOperation);
            }
            catch (StorageException ex) when (ex.RequestInformation.HttpStatusCode == 404)
            {
                return new NotFoundResult();
            }

            return new OkResult();
        }
    }
}
