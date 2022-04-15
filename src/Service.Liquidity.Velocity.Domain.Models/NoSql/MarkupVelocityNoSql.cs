using MyNoSqlServer.Abstractions;

namespace Service.Liquidity.Velocity.Domain.Models.NoSql
{
    public class MarkupVelocityNoSql : MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-liquidity-markup-velocity";
        public static string GeneratePartitionKey(string brokerId)  =>
            $"{brokerId}";
        public static string GenerateRowKey(string assetId) =>
            $"{assetId}";

        public MarkupVelocity Velocity { get; set; }

        public static MarkupVelocityNoSql Create(string brokerId, MarkupVelocity item) =>
            new MarkupVelocityNoSql()
            {
                PartitionKey = GeneratePartitionKey(brokerId),
                RowKey = GenerateRowKey(item.Asset),
                Velocity = item,
            };        
    }
}