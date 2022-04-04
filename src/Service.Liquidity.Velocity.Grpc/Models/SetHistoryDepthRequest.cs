using System.Runtime.Serialization;

namespace Service.Liquidity.Velocity.Grpc.Models;

[DataContract]
public class SetHistoryDepthRequest
{
    [DataMember(Order = 1)] public string Asset { get; set; }
    [DataMember(Order = 2)] public uint Period { get; set; }
    [DataMember(Order = 3)] public string BrokerId { get; set; }
}