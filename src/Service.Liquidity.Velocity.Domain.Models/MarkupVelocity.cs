using System;
using System.Runtime.Serialization;

namespace Service.Liquidity.Velocity.Domain.Models
{
    [DataContract]
    public class MarkupVelocity
    {
        [DataMember(Order = 1)] public string Asset { get; set; }
        [DataMember(Order = 2)] public decimal Velocity { get; set; }
        [DataMember(Order = 3)] public uint  Period { get; set; }
        [DataMember(Order = 4)] public DateTime CalcDateTime { get; set; }
        [DataMember(Order = 5)] public bool CanTrust { get; set; }

    }
}