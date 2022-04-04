using System;
using System.Runtime.Serialization;

namespace Service.Liquidity.Velocity.Domain.Models
{
    [DataContract]
    public class MarkupVelocitySettings
    {
        [DataMember(Order = 1)] public string Asset { get; set; }
        [DataMember(Order = 2)] public uint  Period { get; set; }

    }
}