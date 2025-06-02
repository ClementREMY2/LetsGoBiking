using System.ServiceModel;
using System.Threading.Tasks;

namespace RoutingServer
{
    [ServiceContract]
    public interface RS_IService
    {
        [OperationContract]
        Task<Itineraries> getItinerary(string departure, string arrival);

    }
}
