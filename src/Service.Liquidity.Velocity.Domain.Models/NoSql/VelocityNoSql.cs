using MyNoSqlServer.Abstractions;

namespace Service.Liquidity.Velocity.Domain.Models.NoSql
{
    public class VelocityNoSql : MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-liquitity-velocity";
        public static string GeneratePartitionKey() => "PortfolioVelocity";
        public static string GenerateRowKey(string assetId) =>
            $"{assetId}";

        public Velocity Velocity { get; set; }

        public static VelocityNoSql Create(Velocity item) =>
            new VelocityNoSql()
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(item.Asset),
                Velocity = item,
            };        
    }
}