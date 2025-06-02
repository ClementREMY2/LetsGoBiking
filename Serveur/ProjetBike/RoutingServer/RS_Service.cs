using Newtonsoft.Json.Linq;
using ProxyCacheServeur;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using static ProxyCacheServeur.JCDecaux_Json_Model;

namespace RoutingServer
{
    // REMARQUE : vous pouvez utiliser la commande Renommer du menu Refactoriser pour changer le nom de classe "Service1" à la fois dans le code et le fichier de configuration.
    public class RS_Service : RS_IService
    {
        public static HttpClient httpClient = new HttpClient();
        //It's better to use the service to instantiate the proxy
        //public ProxyCacheServeurService proxy = new ProxyCacheServeurService();
        ProxyCache.IProxyCacheServeurService proxy = new ProxyCache.ProxyCacheServeurServiceClient();
        public string ORS_TOKEN = "5b3ce3597851110001cf624810e17b6d3bbd45acbd9fa50adea18300";
        public string ORS_URL = "api.openrouteservice.org/v2/directions/";


        public Itinerary convertOSRRequestToItinerary(JObject jsonData)
        {
            var itinerary = new Itinerary();
            itinerary.Segments = new List<Segment>();
            var segments = jsonData["features"][0]?["properties"]?["segments"];
            var coordinates = jsonData["features"]?[0]?["geometry"]?["coordinates"];
            foreach (var seg in segments)
            {
                var segment = new Segment
                {
                    Distance = (double)seg["distance"],
                    Duration = (double)seg["duration"],
                    Steps = new List<Step>()
                };

                var steps = seg["steps"];
                foreach (var st in steps)
                {
                  
                    var step = new Step
                    {
                        Distance = (double)st["distance"],
                        Duration = (double)st["duration"],
                        Instruction = (string)st["instruction"],
                    };

                    segment.Steps.Add(step);
                }
                itinerary.Segments.Add(segment);
            }

            itinerary.Geometry = new Geometry();
            itinerary.Geometry.Coordinates = new List<double[]>();
            foreach (var coordinate in coordinates)
            {
                var coord = new double[2];
                coord[0] = (double)coordinate[1];
                coord[1] = (double)coordinate[0];
                itinerary.Geometry.Coordinates.Add(coord);
            }

            return itinerary;
        }

        // if the cycling is false then we'll use foot-walking
        public async Task<JObject> requestToOSR(GeoCoordinate departure, GeoCoordinate arrival, bool cycling = false)
        {
            string wayToGo;
            if (cycling)
            {
                wayToGo = "cycling-regular";
            }
            else
            {
                wayToGo = "foot-walking";
            }
            //request itineraries from OSR

            //We use the invariant culture thing because of the format of the coordinates
            var complete_url = "https://" + ORS_URL + wayToGo + "?api_key=" + ORS_TOKEN + "&start=" + departure.Longitude.ToString(CultureInfo.InvariantCulture) + "," + departure.Latitude.ToString(CultureInfo.InvariantCulture) + "&end=" + arrival.Longitude.ToString(CultureInfo.InvariantCulture) + "," + arrival.Latitude.ToString(CultureInfo.InvariantCulture);
            //Retrieve the response from OSR
            var response = await httpClient.GetAsync(complete_url);
            //Convert the response into a string
            var responseString = await response.Content.ReadAsStringAsync();
            //Convert the string into a JObject
            var parsedJson = JObject.Parse(responseString);
            return parsedJson;
        }

        public Itineraries createActiveMQMessage(ActiveMQ activeMQQueue, List<Itinerary> itineraryList) 
        {
            var queueMessage = new List<Step>();
            var queueName = "queueName";
            //adding all steps to queueMessage
            foreach (var itinerary in itineraryList)
            {
                foreach (var segment in itinerary.Segments)
                {
                    foreach (var step in segment.Steps)
                    {
                        queueMessage.Add(step);
                    }
                }
            }
            //serialization of queueMessage
            var queueMessageSerialized = queueMessage.Select(s => JsonSerializer.Serialize(s)).ToList();
            activeMQQueue.Send(queueName, queueMessageSerialized);
            //We return an empty list of itineraries because we don't want to send the itineraries to the client since we are using activeMQ to do it.
            Itineraries result = new Itineraries();
            result.ItineraryList = new List<Itinerary>();
            return result;

        }



        //Le format des string departure et arrival doit être le suivant : soit un nom de ville / village, soit un lieu précis (ex : Campus SophiaTech), soit une adresse complète (ex : 833 Chemin des Combes, 06600, Antibes)

        public async Task<Itineraries> getItinerary(string departure, string arrival)
        {

            //Step 1: convert string departure and arrival into GeoCoordinate
            GeoCoordinate departure_coordo = await Nominatim.getCoordinateFromAPlace(departure);
            GeoCoordinate arrival_coordo = await Nominatim.getCoordinateFromAPlace(arrival);

            //Step 2: convert GeoCoordinate into string city so now we can link the cities with OSR API
            string city_departure = await Nominatim.getAddressFromACoordinate(departure_coordo);
            string city_arrival = await Nominatim.getAddressFromACoordinate(arrival_coordo);

            //Step 3: get the closest station from the departure. We will use the city of the departure to retrieve the stations from the contract
            //We put the first bool at true because we want to check that the station has at least one bike.
            BikeStation closestStation = proxy.getClosestStation(departure_coordo, await Nominatim.GetCityFromCoordinatesAsync(departure_coordo),true,false);

            //Step 4: get the closest station from the arrival. We will use the city of the arrival to retrieve the stations from the contract
            //We put the second bool at true because we want to check that the station has at least one stand to drop the bike.
            BikeStation closestStationArrival = proxy.getClosestStation(arrival_coordo, await Nominatim.GetCityFromCoordinatesAsync(arrival_coordo),false,true);

            //Step 5: retrieve coordinates from the two aimed stations
            GeoCoordinate departureStation_coordo = new GeoCoordinate(closestStation.position.lat, closestStation.position.lng);
            GeoCoordinate arrivalStation_coordo = new GeoCoordinate(closestStationArrival.position.lat, closestStationArrival.position.lng);


            //Step 6: compute the walking route and retrieve the duration
            Itinerary itineraryWalk = convertOSRRequestToItinerary(await requestToOSR(departure_coordo, arrival_coordo, false));
            Itineraries itinerariesWalk = new Itineraries();
            itinerariesWalk.ItineraryList = new List<Itinerary>();
            itinerariesWalk.ItineraryList.Add(itineraryWalk);
            var durationWalk = itinerariesWalk.ItineraryList[0].Segments[0].Duration;

            //Step 7: compute the biking route and retrieve the duration
            Itinerary itinerary1Bike = convertOSRRequestToItinerary(await requestToOSR(departure_coordo, departureStation_coordo, false));
            Itinerary itinerary2Bike = convertOSRRequestToItinerary(await requestToOSR(departureStation_coordo, arrivalStation_coordo, true));
            Itinerary itinerary3Bike = convertOSRRequestToItinerary(await requestToOSR(arrivalStation_coordo, arrival_coordo, false));
            Itineraries itinerariesBike = new Itineraries();
            itinerariesBike.ItineraryList = new List<Itinerary>();
            itinerariesBike.ItineraryList.Add(itinerary1Bike);
            itinerariesBike.ItineraryList.Add(itinerary2Bike);
            itinerariesBike.ItineraryList.Add(itinerary3Bike);
            var durationBike = itinerariesBike.ItineraryList[0].Segments[0].Duration + itinerariesBike.ItineraryList[1].Segments[0].Duration + itinerariesBike.ItineraryList[2].Segments[0].Duration;

            //Step 8: check which distance is the lowest, and return the itineraries from it via activeMQ
            
            if (durationWalk <= durationBike)
            {
                //return itinerariesWalk, with activeMQ if possible
                try
                {
                    return this.createActiveMQMessage(new ActiveMQ(), itinerariesWalk.ItineraryList);
                }
                catch (ActiveMQNotAvailableException e)
                {
                    Console.WriteLine(e.Message);
                    return itinerariesWalk;
                }
                
            }
            //return itinerariesBike, with activeMQ if possible
            try
            {
                return this.createActiveMQMessage(new ActiveMQ(), itinerariesBike.ItineraryList);
            }
            catch (ActiveMQNotAvailableException e)
            {
                Console.WriteLine(e.Message);
                return itinerariesBike;
            }
        }
    }



}
