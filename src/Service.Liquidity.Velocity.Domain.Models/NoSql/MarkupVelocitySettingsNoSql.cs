using MyNoSqlServer.Abstractions;

namespace Service.Liquidity.Velocity.Domain.Models.NoSql
{
    public class MarkupVelocitySettingsNoSql : MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-markup-velocity-settings";
        public static string GeneratePartitionKey(string brokerId)  =>
            $"{brokerId}";
        public static string GenerateRowKey(string assetId) =>
            $"{assetId}";

        public MarkupVelocitySettings Settings { get; set; }

        public static MarkupVelocitySettingsNoSql Create(string brokerId, MarkupVelocitySettings item) =>
            new MarkupVelocitySettingsNoSql()
            {
                PartitionKey = GeneratePartitionKey(brokerId),
                RowKey = GenerateRowKey(item.Asset),
                Settings = item,
            };        
    }
}