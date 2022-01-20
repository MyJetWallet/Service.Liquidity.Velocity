using System.ServiceModel;
using System.Threading.Tasks;
using Service.Liquidity.Velocity.Grpc.Models;

namespace Service.Liquidity.Velocity.Grpc
{
    [ServiceContract]
    public interface IHelloService
    {
        [OperationContract]
        Task<HelloMessage> SayHelloAsync(HelloRequest request);
    }
}