using System.ServiceModel;
using System.Threading.Tasks;
using Service.Liquidity.Velocity.Grpc.Models;

namespace Service.Liquidity.Velocity.Grpc
{
    [ServiceContract]
    public interface IMarkupVelocityService
    {
        [OperationContract]
        Task<SetHistoryDepthResponse> SetPeriodAsync(SetHistoryDepthRequest historyDepthRequest);
[OperationContract]	        
Task<GetVelocityResponse> GetAllVelocitiesAsync(GetVelocityRequest historyDepthRequest);
    }
}