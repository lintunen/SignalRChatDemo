using Microsoft.WindowsAzure.Storage.Table;

namespace SignalRChat.Domain
{
    public interface ITableStorageWriter
    {
        void Execute();
        void Insert<TEntity>(TEntity entity) where TEntity : TableEntity;
    }
}