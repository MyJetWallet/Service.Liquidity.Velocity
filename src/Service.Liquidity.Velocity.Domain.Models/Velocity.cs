using System;
using System.Runtime.Serialization;

namespace Service.Liquidity.Velocity.Domain.Models
{
    [DataContract]
    public class Velocity
    {
        [DataMember(Order = 1)] public string Asset { get; set; }
        [DataMember(Order = 2)] public decimal LowOpenAverage { get; set; }
        [DataMember(Order = 3)] public decimal HighOpenAverage { get; set; }
        [DataMember(Order = 4)] public DateTime CalcDate { get; set; }
    }
   
}
