using System.Runtime.Serialization;
using Service.Liquidity.Velocity.Domain.Models;

namespace Service.Liquidity.Velocity.Grpc.Models
{
    [DataContract]
    public class HelloMessage : IHelloMessage
    {
        [DataMember(Order = 1)]
        public string Message { get; set; }
    }
}