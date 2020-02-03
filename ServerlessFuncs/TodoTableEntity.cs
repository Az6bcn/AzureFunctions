using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace ServerlessFuncs
{
    public class TodoTableEntity : TableEntity
    {
        public TodoTableEntity()
        {

        }

        public TodoTableEntity(string partitionKey, string rowKey, string eTag)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
            ETag = eTag; // in case of delete, delete whatever the current verion is
        }

        public DateTime CreateedTime { get; set; }
        public string TaskDescription { get; set; }
        public bool IsCompleted { get; set; }
    }

    
}
