using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Threading.Tasks;
using static ProxyCacheServeur.JCDecaux_Json_Model;

namespace ProxyCacheServeur


{
    public class ProxyCacheServeurService : IProxyCacheServeurService
    {
        
        private string API_KEY = "20a2981654e7fa9b4d8e824cf363d15dc3e629d6";
        private string API_URL = "https://api.jcdecaux.com/vls/v1/";
        //We could have created as many cache as we want, but since we only need a cache for the list of stations, we only created this one. 
        private static GenericProxyCache<List<BikeStation>> cacheListBikeStation = new GenericProxyCache<List<BikeStation>>();
        

        public async Task<BikeStation> getClosestStation(GeoCoordinate geoCoordinate, string contractName, bool checkAvailabity, bool checkCapacity)
        {
            List<BikeStation> stations = await getStationsFromAContract(contractName);
            BikeStation closestStation = null;
            double minDistance = double.MaxValue;

            //if our list of stations is empty or null, it means that the contractName is not valid. In that case, we simply retrieve all the stations
            if (stations.Count() == 0 || stations == null)
            {
                stations = await getStations();
            }
            
            foreach (BikeStation station in stations)
            {
                //Usually, we put the availability var at true if we want to retrieve the closest station from the origin. Then, if this variable is true, we check that the station has at least one bike. If it has none, we skip this station
                if (checkAvailabity && station.available_bikes == 0)
                {
                    continue;
                }
                //Usually, we put the capacity var at true if we want to retrieve the closest station from the destination. Then, if this variable is true, we check that the station has at least one stand. If it has none, we skip this station
                if (checkCapacity && station.available_bike_stands == 0)
                {
                    continue;
                }


                GeoCoordinate stationGeoCoordinate = new GeoCoordinate(station.position.lat, station.position.lng);
                double distance = geoCoordinate.GetDistanceTo(stationGeoCoordinate);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestStation = station;
                }
                
            }
            return closestStation;
        }

        public async Task<List<BikeStation>> getStationsFromAContract(string contractName)
        {
            var complete_url = API_URL + "stations?contract=" + contractName + "&apiKey=" + API_KEY;
            try
            {
                var selectedStations = await cacheListBikeStation.Get(complete_url, 300);
                return selectedStations;
            }
            //if the contractName is not valid, we return an empty list. The best for this is to throw an exception, even if we don't want to stop the program.
            catch (Exception e)
            {
                return new List<BikeStation>();
            } 
        }

        //We retrieve all the stations from all the contracts
        public async Task<List<BikeStation>> getStations()
        {
            var complete_url = API_URL + "stations?apiKey=" + API_KEY;
            var selectedStation = await cacheListBikeStation.Get(complete_url, 300);
            return selectedStation;
        }
    }
}
