using System;
namespace ServerlessFuncs
{
    public static class Mappings
    {
        public static TodoTableEntity ToTableEntity(Todo todo)
        {
            return new TodoTableEntity
            {
                PartitionKey = "TODO",
                RowKey = todo.Id,
                CreateedTime = todo.CreateedTime,
                IsCompleted = todo.IsCompleted,
                TaskDescription = todo.TaskDescription
            };
        }


        public static Todo ToTodo(TodoTableEntity tableEntity)
        {
            return new Todo
            {
                Id = tableEntity.RowKey,
                CreateedTime = tableEntity.CreateedTime,
                IsCompleted = tableEntity.IsCompleted,
                TaskDescription = tableEntity.TaskDescription
            };
        }
    }
}
