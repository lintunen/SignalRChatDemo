using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace SignalRChat.Domain
{
    /// <summary>
    /// Azure table storage chat message entity
    /// </summary>
    public class ChatMessageEntity : TableEntity
    {
        public ChatMessageEntity() { }

        public ChatMessageEntity(string displayName, string connectionID)
        {
            this.PartitionKey = displayName;
            this.RowKey = connectionID;
        }

        public string Message { get; set; }
    }
}
