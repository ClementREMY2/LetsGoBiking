using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Caching;
using System.Text.Json;
using System.Threading.Tasks;

public class GenericProxyCache<T> where T : class
{
    //Our cache is a dictionary with a string (a request from an URL usually) as a key and a tuple as a value. For instance, if we want to retrieve all the station from a contract, the string will be the right URL and the tuple will be the list of stations that matches the contract and the expiration date of the cache.
    private Dictionary<string, Tuple<T, DateTimeOffset>> cache = new Dictionary<string, Tuple<T, DateTimeOffset>>();
    private readonly HttpClient httpClient = new HttpClient();


    public async Task<T> Get(string cacheItemName)
    {
        return await GetAsync(cacheItemName, ObjectCache.InfiniteAbsoluteExpiration);
    }

    public async Task<T> Get(string cacheItemName, double dt_seconds)
    {
        var expirationTime = DateTimeOffset.Now.AddSeconds(dt_seconds);
        return await GetAsync(cacheItemName, expirationTime);
    }

    public async Task<T> GetAsync(string cacheItemName, DateTimeOffset dt)
    {

        //Step1 : clear the value that have an expired date < actual date
        cache = cache.Where(x => x.Value.Item2 >= DateTimeOffset.Now).ToDictionary(x => x.Key, x => x.Value);

        //Step2 : check if the cacheItemName is in the cache
        if (cache.ContainsKey(cacheItemName) && cache[cacheItemName].Item1 != null)
        {
            //Our value is in the cache, we just have to return it
            return cache[cacheItemName].Item1;
        }
        //Step3 : if the value is not in the cache, we have to add it
        var cachedItem = await httpClient.GetStringAsync(cacheItemName);
        cache.Add(cacheItemName, new Tuple<T, DateTimeOffset>(JsonSerializer.Deserialize<T>(cachedItem), dt));
        return cache[cacheItemName].Item1;

    }
}