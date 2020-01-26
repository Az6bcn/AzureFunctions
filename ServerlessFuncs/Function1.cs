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

namespace ServerlessFuncs
{
    public static class TodoApi
    {
        static List<Todo> items = new List<Todo>();

        [FunctionName("CreateTodo")]
        public static async Task<IActionResult> CreateTodo([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo")] HttpRequest req,ILogger log)
        {
            log.LogInformation("Creating a new todo list item.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<TodoCreateModel>(requestBody);

            var todo = new Todo { TaskDescription = data.TaskDescription };

            items.Add(todo);

            return new OkObjectResult(todo);
        }

        [FunctionName("GetAllTodos")]
        public static async Task<IActionResult> GetAllTodos([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo")] HttpRequest req, ILogger log)
        {
            log.LogInformation("Getting all todo list item.");

            return new OkObjectResult(items);
        }

        [FunctionName("GetTodoById")]
        public static async Task<IActionResult> GetTodoById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo/{id}")] HttpRequest req, ILogger log, string id)
        {
            log.LogInformation("Getting a specific todo list item by its Id.");

            if(id == Guid.Empty.ToString() || id == string.Empty)
            {
                return new BadRequestObjectResult("invalid Id");
            }

            if(id == null)
            {
                return new NotFoundResult();
            }

            var item = items.Where(x => x.Id == id).FirstOrDefault();

            return new OkObjectResult(items);
        }

        [FunctionName("UpdateTodo")]
        public static async Task<IActionResult> UpdateTodo([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todo/{id}")] HttpRequest req, ILogger log, string id)
        {
            log.LogInformation("Updating a specific todo list item by its Id.");

            if (id == Guid.Empty.ToString() || id == string.Empty)
            {
                return new BadRequestObjectResult("invalid Id");
            }

            if (id == null)
            {
                return new NotFoundResult();
            }

            var item = items.Where(x => x.Id == id).FirstOrDefault();
            if (item is null)
            {
                return new NotFoundResult();
            }

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updatedTodo = JsonConvert.DeserializeObject<TodoUpdateModel>(requestBody);


            item.IsCompleted = updatedTodo.IsCompleted;

            if (!string.IsNullOrEmpty(updatedTodo.TaskDescription))
            {
                item.TaskDescription = updatedTodo.TaskDescription;
            }

            return new OkObjectResult(items);
        }

        [FunctionName("DeleteTodo")]
        public static async Task<IActionResult> DeleteTodo([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todo/{id}")] HttpRequest req, ILogger log, string id)
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

            var item = items.Where(x => x.Id == id).FirstOrDefault();

            if (item is null)
            {
                return new NotFoundResult();
            }

            items.Remove(item);

            return new OkResult();
        }
    }
}
