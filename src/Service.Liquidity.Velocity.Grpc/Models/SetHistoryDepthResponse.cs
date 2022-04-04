using System.Runtime.Serialization;

namespace Service.Liquidity.Velocity.Grpc.Models;

[DataContract]
public class SetHistoryDepthResponse
{
    [DataMember(Order = 1)] public bool IsError{ get; set; }
    [DataMember(Order = 2)] public string ErrorMessage { get; set; }
}