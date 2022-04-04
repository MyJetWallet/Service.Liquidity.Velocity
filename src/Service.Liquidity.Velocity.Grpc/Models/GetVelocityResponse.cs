using System.Collections.Generic;
using System.Runtime.Serialization;
using Service.Liquidity.Velocity.Domain.Models;

namespace Service.Liquidity.Velocity.Grpc.Models;

[DataContract]
public class GetVelocityResponse
{
    [DataMember(Order = 1)] public List<MarkupVelocity> Items { get; set; }
    [DataMember(Order = 2)] public bool IsError { get; set; }
    [DataMember(Order = 3)] public string ErrorMessage { get; set; }
}