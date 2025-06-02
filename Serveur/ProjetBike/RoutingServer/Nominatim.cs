using Newtonsoft.Json.Linq;
using System;
using System.Device.Location;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace RoutingServer
{
    //This class provide useful methods to interact with the Nominatim API
    internal class Nominatim
    {
        public static HttpClient httpClient = new HttpClient();
        public const string NominatimSearchRequest = "https://nominatim.openstreetmap.org/search";
        public const string NominatimReverseRequest = "https://nominatim.openstreetmap.org/reverse";

        public static async Task<string> GetCityFromCoordinatesAsync(GeoCoordinate coordinates)
        {
            //The CultureInfo thing is to format the numbers correctly
            string url = $"{NominatimReverseRequest}?lat={coordinates.Latitude.ToString(CultureInfo.InvariantCulture)}&lon={coordinates.Longitude.ToString(CultureInfo.InvariantCulture)}&format=json&addressdetails=1&limit=1";
            var response = await httpClient.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();
            var parsedJson = JToken.Parse(json);

            if (parsedJson is JArray)
                parsedJson = parsedJson.FirstOrDefault();
            else if (parsedJson is JObject)
                //yes, this is just not to throw an Exception in the case where the parseJson is a JObject. It's not a good practice, but it works.
                parsedJson = parsedJson;
            else
                throw new NoPlaceFoundException("No place found for these coordinates: " + coordinates);

            var address = parsedJson["address"];

            //The ?? operator is to check if the first value is null, if it is, it takes the second one, etc.
            var city = (string)address?["city"] ?? (string)address?["town"] ?? (string)address?["village"];
            if (city == null)
                throw new NoPlaceFoundException(
                    "Please enter a place or city.");
            return city;
        }

        public static async Task<GeoCoordinate> getCoordinateFromAPlace(string place)
        {
            //We add an header to avoid the API to refuse the connection
            httpClient.DefaultRequestHeaders.Add("User-Agent", "C# App");
            //Allow us to encode the special characters (ex: " " => "%20")
            place = HttpUtility.UrlEncode(place);
            string url = $"{NominatimSearchRequest}?q={place}&format=json&addressdetails=1&limit=1";
            try
            {
                //Retrieve the response from the API
                var response = await httpClient.GetAsync(url);
                //We convert it into a string
                var responseString = await response.Content.ReadAsStringAsync();
                //We deserialize it
                var parsedJson = JToken.Parse(responseString);
                //We make conditions because the same address can be duplicated in France, we then need to be more precise
                if (parsedJson is JArray)
                {
                    return new GeoCoordinate((double)parsedJson.FirstOrDefault()["lat"], (double)parsedJson.FirstOrDefault()["lon"]);
                }
                else if (parsedJson is JObject)
                {
                    return new GeoCoordinate((double)parsedJson["lat"], (double)parsedJson["lon"]);
                }
                else
                {
                    return null;
                }
            } 
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }

        public static async Task<string> getAddressFromACoordinate(GeoCoordinate coordinate)
        {
            //We add an header to avoid the API to refuse the connection
            httpClient.DefaultRequestHeaders.Add("User-Agent", "C# App");
            //We used the cultureInfo thing to format the numbers correctly
            string url = $"{NominatimReverseRequest}?lat={coordinate.Latitude.ToString(CultureInfo.InvariantCulture)}&lon={coordinate.Longitude.ToString(CultureInfo.InvariantCulture)}&format=json&addressdetails=1&limit=1";
            try
            {
                //Retrieve the response from the API
                var response = await httpClient.GetAsync(url);
                //We convert it into a string
                var responseString = await response.Content.ReadAsStringAsync();
                //We deserialize it
                var parsedJson = JToken.Parse(responseString);
                //We make conditions because the same address can be duplicated in France, we then need to be more precise
                if (parsedJson is JArray)
                {
                    return (string)parsedJson.FirstOrDefault()["display_name"];
                }
                else if (parsedJson is JObject)
                {
                    return (string)parsedJson["display_name"];
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }       

    }
}
