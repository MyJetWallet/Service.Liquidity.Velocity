using System.Runtime.Serialization;

namespace Service.Liquidity.Velocity.Grpc.Models;

[DataContract]
public class GetVelocityRequest
{
    [DataMember(Order = 1)] public string BrokerId { get; set; }
}