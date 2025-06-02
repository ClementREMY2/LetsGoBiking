using System.Collections.Generic;
using System.Device.Location;
using System.ServiceModel;
using System.Threading.Tasks;
using static ProxyCacheServeur.JCDecaux_Json_Model;

namespace ProxyCacheServeur
{
    [ServiceContract]
    public interface IProxyCacheServeurService
    {
        [OperationContract]
        Task<List<BikeStation>> getStations();

        [OperationContract]
        Task<BikeStation> getClosestStation(GeoCoordinate geoCoordinate, string contractName, bool checkAvailabity, bool checkCapacity);

        [OperationContract]
        Task<List<BikeStation>> getStationsFromAContract(string contractName);
    }
}
